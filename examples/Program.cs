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
    }
}
