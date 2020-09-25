using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Ejdb2.Tests
{
    public class EJDB2Tests
    {
        public EJDB2Tests()
        {
            TestUtils.Cleanup("test.db");
            TestUtils.Cleanup("test-bkp.db");
        }

        [Fact]
        public void Ctor_NullArgument_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EJDB2(null));
        }

        [Fact]
        public void Put_PutRecordToEmptyDb_Returns1()
        {
            using var db = CreateTestDb();
            long id = db.Put("c1", TestUtils.Json);
            Assert.Equal(1, id);
        }

        [Fact]
        public void Get_Test()
        {
            using var db = CreateTestDb();

            long id = db.Put("c1", TestUtils.Json);
            string actual = db.Get("c1", id);

            Assert.Equal(TestUtils.Json, actual);
        }

        [Fact]
        public void Patch_Test()
        {
            using var db = CreateTestDb();

            long id = db.Put("c1", TestUtils.Json);
            db.Patch("c1", TestUtils.Patch, id);
            string actual = db.Get("c1", id);

            Assert.Equal(@"{""foo"":""bar"",""baz"":""qux""}", actual);
        }

        [Fact]
        public void Get_NonExistentRecord_ThrowsEJDB2Exception()
        {
            using var db = CreateTestDb();
            var e = Assert.Throws<EJDB2Exception>(() => db.Get("c1", 1));
            Assert.Contains("IW_ERROR_NOT_EXISTS", e.Message);
        }

        [Fact]
        public void CreateQuery_ReturnsNotNull()
        {
            using var db = CreateTestDb();
            using var jql = db.CreateQuery("@mycoll/*");
            Assert.NotNull(jql);
        }

        [Fact]
        public void CreateQuery_InvalidQuery_ThrowsEJDB2Exception()
        {
            using var db = CreateTestDb();

            var e = Assert.Throws<EJDB2Exception>(() => db.CreateQuery("@mycoll/["));
            Assert.Contains("@mycoll/[ <---", e.Message);
            Assert.Equal(87001ul, e.Code);
        }

        [Fact]
        public void Put_InvalidJson_ThrowsEJDB2Exception()
        {
            using var db = CreateTestDb();

            var e = Assert.Throws<EJDB2Exception>(() => db.Put("c1", "{\""));
            Assert.Contains("JBL_ERROR_PARSE_UNQUOTED_STRING", e.Message);
            Assert.Equal(86005ul, e.Code);
        }

        [Fact]
        public void GetInfo_Test()
        {
            using var db = CreateTestDb();

            db.Put("c1", TestUtils.Json);
            db.Delete("c1", 1);
            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");

            string json = db.GetInfo();

            Assert.NotNull(json);
            Assert.Contains("{\"name\":\"mycoll\",\"dbid\":4,\"rnum\":2,\"indexes\":[]}", json);
        }

        [Fact]
        public void EnsureStringIndex_Test()
        {
            using var db = CreateTestDb();

            db.Put("c1", TestUtils.Json);
            db.Delete("c1", 1);
            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            
            db.EnsureStringIndex("mycoll", "/foo", true);
            string json = db.GetInfo();

            Assert.NotNull(json);
            Assert.Contains("\"indexes\":[{\"ptr\":\"/foo\",\"mode\":5,\"idbf\":0,\"dbid\":5,\"rnum\":2}]", json);
        }

        [Fact]
        public void RemoveStringIndex_Test()
        {
            using var db = CreateTestDb();

            db.Put("c1", TestUtils.Json);
            db.Delete("c1", 1);
            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            db.EnsureStringIndex("mycoll", "/foo", true);
            db.Patch("mycoll", TestUtils.Patch, 2);

            db.RemoveStringIndex("mycoll", "/foo", true);
            string json = db.GetInfo();

            Assert.Contains("{\"name\":\"mycoll\",\"dbid\":4,\"rnum\":2,\"indexes\":[]}", json);
        }

        [Fact]
        public void RemoveCollection_Test()
        {
            using var db = CreateTestDb();

            db.Put("c1", TestUtils.Json);
            db.Delete("c1", 1);
            db.Put("mycoll", @"{""foo"":""bar""}");
            db.Put("mycoll", @"{""foo"":""baz""}");
            db.Patch("mycoll", TestUtils.Patch, 2);

            db.RemoveCollection("mycoll");
            db.RemoveCollection("c1");
            string json = db.GetInfo();

            Assert.Contains("\"collections\":[]", json);
        }

        [Fact]
        public void RenameCollection_Test()
        {
            using var db = CreateTestDb();

            db.Put("cc1", "{\"foo\": 1}");
            db.RenameCollection("cc1", "cc2");
            string result = db.Get("cc2", 1);

            Assert.Equal("{\"foo\":1}", result);
        }

        [Fact]
        public void OnlineBackup_Test()
        {
            using var db = CreateTestDb();

            ulong ts0 = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ulong ts = db.OnlineBackup("test-bkp.db");

            Assert.True(ts > ts0);
            Assert.True(File.Exists("test-bkp.db"));
        }

        [Fact]
        public void OnlineBackup_Test2()
        {
            using var db = CreateTestDb();

            db.Put("cc1", "{\"foo\": 1}");
            db.RenameCollection("cc1", "cc2");

            db.OnlineBackup("test-bkp.db");

            var options2 = new EJDB2OptionsBuilder("test-bkp.db")
                .WithWAL()
                .GetOptions();

            using var db2 = new EJDB2(options2);
            string val = db2["cc2", 1];

            Assert.Equal("{\"foo\":1}", val);
        }

        private EJDB2 CreateTestDb() => TestUtils.CreateTestDb("test.db");
    }
}
