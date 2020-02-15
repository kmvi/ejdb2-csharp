using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Ejdb2.Tests
{
    public class JQLTests
    {
        public JQLTests()
        {
            TestUtils.Cleanup("test1.db");
            TestUtils.Cleanup("test-bkp1.db");
        }

        [Fact]
        public void Db_ReturnsNotNull()
        {
            using var db = CreateTestDb();
            using var jql = db.CreateQuery("@mycoll/*");
            Assert.NotNull(jql.Db);
        }

        [Fact]
        public void Collection_EqualsToCollectionNameFromQuery()
        {
            using var db = CreateTestDb();
            using var jql = db.CreateQuery("@mycoll/*");
            Assert.Equal("mycoll", jql.Collection);
        }

        [Fact]
        public void Execute_Test()
        {
            using var db = CreateTestDb();

            db.Put("mycoll", @"{""foo"":""baz""}");
            using var query = db.CreateQuery("@mycoll/*");

            query.Execute((docId, doc) =>
            {
                Assert.True(docId > 0);
                Assert.NotNull(doc);
                return 1;
            });
        }

        [Fact]
        public void Execute_Test2()
        {
            using var db = CreateTestDb();

            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            using var query = db.CreateQuery("@mycoll/*");

            var results = new Dictionary<long, string>();
            query.Execute((docId, doc) =>
            {
                results.Add(docId, doc);
                return 1;
            });

            Assert.Equal(2, results.Count);
            Assert.Equal("{\"foo\":\"bar\"}", results[1L]);
            Assert.Equal("{\"foo\":\"baz\"}", results[2L]);
        }

        [Fact]
        public void SetString_Test()
        {
            using var db = CreateTestDb();

            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            using var query = db.CreateQuery("/[foo=:?]", "mycoll").SetString(0, "zaz");

            var results = new Dictionary<long, string>();
            query.Execute((docId, doc) =>
            {
                results.Add(docId, doc);
                return 1;
            });

            Assert.Empty(results);
        }

        [Fact]
        public void SetString_Test2()
        {
            using var db = CreateTestDb();

            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            using var query = db.CreateQuery("/[foo=:val]", "mycoll").SetString("val", "bar");

            var results = new Dictionary<long, string>();
            query.Execute((docId, doc) =>
            {
                results.Add(docId, doc);
                return 1;
            });

            Assert.Single(results);
            Assert.Equal("{\"foo\":\"bar\"}", results[1L]);
        }

        [Fact]
        public void ExecuteScalarInt64_Test()
        {
            using var db = CreateTestDb();

            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            using var query = db.CreateQuery("@mycoll/* | count");

            long count = query.ExecuteScalarInt64();

            Assert.Equal(2, count);
        }

        [Fact]
        public void GetExplainLog_Test()
        {
            using var db = CreateTestDb();

            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            using var query = db.CreateQuery("@mycoll/* | count");

            query.WithExplain().Execute();
            string log = query.GetExplainLog();

            Assert.Contains("[INDEX] NO [COLLECTOR] PLAIN", log);
        }

        [Fact]
        public void FirstJson_Test()
        {
            using var db = CreateTestDb();

            PrepareDb(db);

            string json = db.CreateQuery("@mycoll/[foo=:?] and /[baz=:?]")
              .SetString(0, "baz")
              .SetString(1, "qux")
              .FirstJson();

            Assert.NotNull(json);
            Assert.Equal("{\"foo\":\"baz\",\"baz\":\"qux\"}", json);
        }

        [Fact]
        public void FirstJson_Test2()
        {
            using var db = CreateTestDb();

            PrepareDb(db);

            string json = db.CreateQuery("@mycoll/[foo re :?]")
                .SetRegexp(0, ".*")
                .FirstJson();

            Assert.NotNull(json);
            Assert.Equal("{\"foo\":\"baz\",\"baz\":\"qux\"}", json);
        }

        [Fact]
        public void Limit_Test()
        {
            using var db = CreateTestDb();

            using var query = db.CreateQuery("@mycoll/* | limit 2 skip 3");

            Assert.Equal(2, query.Limit);
            Assert.Equal(3, query.Skip);
        }

        private void PrepareDb(EJDB2 db)
        {
            db.Put("c1", TestUtils.Json);
            db.Delete("c1", 1);
            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            db.EnsureStringIndex("mycoll", "/foo", true);
            db.Patch("mycoll", TestUtils.Patch, 2);
        }

        private EJDB2 CreateTestDb() => TestUtils.CreateTestDb("test1.db");
    }
}
