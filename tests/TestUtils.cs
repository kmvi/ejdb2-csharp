using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ejdb2.Tests
{
    internal static class TestUtils
    {
        public const string Json = @"{""foo"":""bar""}";
        public const string Patch = @"[{""op"":""add"", ""path"":""/baz"", ""value"":""qux""}]";

        public static void Cleanup(string name)
        {
            if (File.Exists(name))
                File.Delete(name);
        }

        public static EJDB2 CreateTestDb(string name)
        {
            var options = new EJDB2OptionsBuilder(name)
                .WithWAL().Truncate()
                .GetOptions();

            return new EJDB2(options);
        }
    }
}
