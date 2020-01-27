# EJDB2 .NET binding

Embeddable JSON Database engine http://ejdb.org .NET binding (.NET Standard 2.0).

See https://github.com/Softmotions/ejdb/blob/master/README.md

For API usage examples take a look into [examples project](https://github.com/kmvi/ejdb2-csharp/blob/master/examples/Program.cs).

## Minimal example

``` csharp
static void Main(string[] args)
{
    var options = new EJDB2Options("example.db");
    options.IWKVOptions.Truncate = true;

    using var db = new EJDB2(options);

    long id = db.Put("parrots", "{\"name\":\"Bianca\", \"age\": 4}");
    Console.WriteLine("Bianca record: {0}", id);

    id = db.Put("parrots", "{\"name\":\"Darko\", \"age\": 8}");
    Console.WriteLine("Darko record: {0}", id);
    
    db.CreateQuery("@parrots/[age > :age]").SetLong("age", 3)
        .Execute((docId, doc) => {
            Console.WriteLine("Found {0}: {1}", docId, doc);
            return 1;
        });
}
```

## Supported platforms

* Windows x64
* TODO: Linux x64

## How to build it manually

- Build EJDB2 as a shared library ([guide](https://github.com/Softmotions/ejdb/blob/master/WINDOWS.md))
- Build iowow as a shared library ([guide](http://iowow.io/iw/win))

``` cmd
git clone https://github.com/kmvi/ejdb2-csharp
cd ejdb2-csharp
dotnet build
```

## Run example

Copy `libejdb2.dll` and `libiowow.dll` in `examples\bin\{Configuration}\netcoreapp2.1` directory.

``` cmd
cd examples
dotnet run
```
