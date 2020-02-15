# EJDB2 .NET binding

Embeddable JSON Database engine http://ejdb.org .NET binding (.NET Standard 2.0).

See https://github.com/Softmotions/ejdb/blob/master/README.md

For API usage examples take a look into [examples](https://github.com/kmvi/ejdb2-csharp/blob/master/examples/Program.cs) and [tests](https://github.com/kmvi/ejdb2-csharp/tree/master/tests).

## Minimal example

``` csharp
static void Main(string[] args)
{
    var options = new EJDB2OptionsBuilder("example.db")
        .Truncate().GetOptions();

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

* Linux x64
* Windows x64

## How to build it manually

### Linux

- Build and/or install EJDB2: https://github.com/Softmotions/ejdb#linux
- Build ejdb2-csharp:

``` cmd
git clone https://github.com/kmvi/ejdb2-csharp
cd ejdb2-csharp
dotnet build
```

### Windows

- Clone [ejdb](https://github.com/Softmotions/ejdb) repo
- Apply the patch:

``` diff
diff --git a/cmake/Modules/AddIOWOW.cmake b/cmake/Modules/AddIOWOW.cmake
index ce674bc8..ed034e06 100644
--- a/cmake/Modules/AddIOWOW.cmake
+++ b/cmake/Modules/AddIOWOW.cmake
@@ -19,13 +19,13 @@ endif()
 if (IOS)
   set(BYPRODUCT "${CMAKE_BINARY_DIR}/lib/libiowow-1.a")
 else()
-  set(BYPRODUCT "${CMAKE_BINARY_DIR}/src/extern_iowow-build/src/libiowow-1.a")
+  set(BYPRODUCT "${CMAKE_BINARY_DIR}/src/extern_iowow-build/src/libiowow.dll.a")
 endif()

 set(CMAKE_ARGS  -DOWNER_PROJECT_NAME=${PROJECT_NAME}
                 -DCMAKE_BUILD_TYPE=${CMAKE_BUILD_TYPE}
                 -DCMAKE_INSTALL_PREFIX=${CMAKE_BINARY_DIR}
-                -DBUILD_SHARED_LIBS=OFF
+                -DBUILD_SHARED_LIBS=ON
                 -DBUILD_EXAMPLES=OFF)

 foreach(extra CMAKE_TOOLCHAIN_FILE
```

- Build both EJDB2 and iowow as shared libraries ([guide](https://github.com/Softmotions/ejdb/blob/master/WINDOWS.md))
- Build ejdb2-csharp:

``` cmd
git clone https://github.com/kmvi/ejdb2-csharp
cd ejdb2-csharp
dotnet build
```

## Run example

Windows only: copy `libejdb2.dll` and `libiowow.dll` in `examples\bin\{Configuration}\netcoreapp3.1` directory.

``` cmd
cd examples
dotnet run
```

## Run tests

Windows only: copy `libejdb2.dll` and `libiowow.dll` in `tests\bin\{Configuration}\netcoreapp3.1` directory.

``` cmd
cd tests
dotnet test
```
