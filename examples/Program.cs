using System;
using System.IO;
using Ejdb2;

namespace Ejdb2.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new EJDB2Options("example.db");
            options.IWKVOptions.Truncate = true;

            using var db = new EJDB2(options);

            long id = db.Put("parrots", "{\"name\":\"Bianca\", \"age\": 4}");
            Console.WriteLine("Bianca record: {0}", id);

            id = db.Put("parrots", "{\"name\":\"Darko\", \"age\": 8}");
            Console.WriteLine("Darko record: {0}", id);

            string data = db.Get("parrots", id);
            Console.WriteLine("Record {0} data: {1}", id, data);

            string info = db.GetInfo();
            Console.WriteLine("DB info: {0}", info);
        }
    }
}
