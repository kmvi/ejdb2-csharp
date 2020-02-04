using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ejdb2;
using static System.Diagnostics.Debug;

namespace Ejdb2.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            Test2();
        }

        static void Test1()
        {
            if (File.Exists("example.db"))
                File.Delete("example.db");

            var options = new EJDB2OptionsBuilder("example.db")
                .WithWAL().Truncate()
                .GetOptions();

            using var db = new EJDB2(options);

            long id = db.Put("parrots", "{\"name\":\"Bianca\", \"age\": 4}");
            Console.WriteLine("Bianca record: {0}", id);

            id = db.Put("parrots", "{\"name\":\"Darko\", \"age\": 8}");
            Console.WriteLine("Darko record: {0}", id);

            id = db.Put("parrots", "{\"name\":\"тест 迶逅透進\", \"age\": 8}");
            Console.WriteLine("UTF8 test record: {0}", id);

            string data = db.Get("parrots", id);
            Console.WriteLine("Record {0} data: {1}", id, data);

            string info = db.GetInfo();
            Console.WriteLine("DB info: {0}", info);

            db.CreateQuery("@parrots/[age > :age]").SetLong("age", 3)
                .Execute((docId, doc) => {
                    Console.WriteLine("Found {0}: {1}", docId, doc);
                    return 1;
                });
        }

        static void Test2()
        {
            if (File.Exists("test.db"))
                File.Delete("test.db");

            if (File.Exists("test-bkp.db"))
                File.Delete("test-bkp.db");

            var options = new EJDB2OptionsBuilder("test.db")
                .WithWAL().Truncate()
                .GetOptions();

            using var db = new EJDB2(options);

            EJDB2Exception exception = null;

            string json = "{'foo':'bar'}".Replace('\'', '"');
            string patch = "[{'op':'add', 'path':'/baz', 'value':'qux'}]".Replace('\'', '"');

            long id = db.Put("c1", json);
            Assert(id == 1L);

            var bos = new MemoryStream();
            db.Get("c1", 1L, bos);
            Assert(Encoding.UTF8.GetString(bos.ToArray()).Equals(json));
            bos.Dispose();

            db.Patch("c1", patch, id);
            bos = new MemoryStream();
            db.Get("c1", 1L, bos);
            Assert(Encoding.UTF8.GetString(bos.ToArray()).Equals("{'foo':'bar','baz':'qux'}".Replace('\'', '"')));
            bos.Dispose();
            bos = new MemoryStream();

            db.Delete("c1", id);

            exception = null;
            try
            {
                db.Get("c1", id, bos);
            }
            catch (EJDB2Exception e)
            {
                exception = e;
            }

            Assert(exception != null);
            Assert(exception.Message.Contains("IWKV_ERROR_NOTFOUND"));
            bos.Dispose();

            // JQL resources can be closed explicitly or garbage collected
            JQL q = db.CreateQuery("@mycoll/*");
            Assert(q.Db != null);
            Assert(q.Collection == "mycoll");

            id = db.Put("mycoll", "{'foo':'bar'}".Replace('\'', '"'));
            Assert(id == 1);

            exception = null;
            try
            {
                db.Put("mycoll", "{\"");
            }
            catch (EJDB2Exception e)
            {
                exception = e;
            }

            Assert(exception != null && exception.Message != null);
            Assert(exception.Code == 86005);
            Assert(exception.Message.Contains("JBL_ERROR_PARSE_UNQUOTED_STRING"));

            db.Put("mycoll", "{'foo':'baz'}".Replace('\'', '"'));

            var results = new Dictionary<long, string>();
            q.Execute((docId, doc) =>
            {
                Assert(docId > 0 && doc != null);
                results.Add(docId, doc);
                return 1;
            });

            Assert(results.Count == 2);
            Assert(results[1L] == "{\"foo\":\"bar\"}");
            Assert(results[2L] == "{\"foo\":\"baz\"}");
            results.Clear();

            using (JQL q2 = db.CreateQuery("/[foo=:?]", "mycoll").SetString(0, "zaz"))
            {
                q2.Execute((docId, doc) =>
                {
                    results.Add(docId, doc);
                    return 1;
                });
            }

            Assert(results.Count == 0);

            using (JQL q2 = db.CreateQuery("/[foo=:val]", "mycoll").SetString("val", "bar"))
            {
                q2.Execute((docId, doc) =>
                {
                    results.Add(docId, doc);
                    return 1;
                });
            }

            Assert(results.Count == 1);
            Assert(results[1L] == "{\"foo\":\"bar\"}");

            exception = null;
            try
            {
                db.CreateQuery("@mycoll/[");
            }
            catch (EJDB2Exception e)
            {
                exception = e;
            }
            Assert(exception != null && exception.Message != null);
            Assert(exception.Code == 87001);
            Assert(exception.Message.Contains("@mycoll/[ <---"));

            long count = db.CreateQuery("@mycoll/* | count").ExecuteScalarInt64();
            Assert(count == 2);

            q.WithExplain().Execute();
            string log = q.GetExplainLog();
            Console.WriteLine(log);
            Assert(log.Contains("[INDEX] NO [COLLECTOR] PLAIN"));

            json = db.GetInfo();
            Assert(json != null);
            Assert(json.Contains("{\"name\":\"mycoll\",\"dbid\":4,\"rnum\":2,\"indexes\":[]}"));

            // Indexes
            db.EnsureStringIndex("mycoll", "/foo", true);
            json = db.GetInfo();
            Assert(json.Contains("\"indexes\":[{\"ptr\":\"/foo\",\"mode\":5,\"idbf\":0,\"dbid\":5,\"rnum\":2}]"));

            db.Patch("mycoll", patch, 2);

            json = db.CreateQuery("@mycoll/[foo=:?] and /[baz=:?]")
              .SetString(0, "baz")
              .SetString(1, "qux")
              .FirstJson();
            Assert("{\"foo\":\"baz\",\"baz\":\"qux\"}".Equals(json));

            json = db.CreateQuery("@mycoll/[foo re :?]").SetRegexp(0, ".*").FirstJson();
            Assert("{\"foo\":\"baz\",\"baz\":\"qux\"}".Equals(json));

            db.RemoveStringIndex("mycoll", "/foo", true);
            json = db.GetInfo();
            Assert(json.Contains("{\"name\":\"mycoll\",\"dbid\":4,\"rnum\":2,\"indexes\":[]}"));

            db.RemoveCollection("mycoll");
            db.RemoveCollection("c1");
            json = db.GetInfo();
            Assert(json.Contains("\"collections\":[]"));

            // Test rename collection
            bos = new MemoryStream();
            db.Put("cc1", "{\"foo\": 1}");
            db.RenameCollection("cc1", "cc2");
            db.Get("cc2", 1, bos);
            Assert(Encoding.UTF8.GetString(bos.ToArray()).Equals("{\"foo\":1}"));
            bos.Dispose();

            // Check limit
            q = db.CreateQuery("@mycoll/* | limit 2 skip 3");
            Assert(q.Limit == 2);
            Assert(q.Skip == 3);

            ulong ts0 = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ulong ts = db.OnlineBackup("test-bkp.db");
            Assert(ts > ts0);
            Assert(File.Exists("test-bkp.db"));

            var options2 = new EJDB2OptionsBuilder("test-bkp.db")
                .WithWAL()
                .GetOptions();

            using (EJDB2 db2 = new EJDB2(options2))
            {
                string val = db2["cc2", 1];
                Assert(val.Equals("{\"foo\":1}"));
            }
        }
    }
}
