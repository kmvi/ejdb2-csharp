using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ejdb2.Native;

namespace Ejdb2
{
    /// <summary>
    /// EJDB2 database
    /// </summary>
    /// <remarks>
    /// In order to release memory resources and avoiding data lost every opened
    /// database instance should be closed with <c>Dispose</c>
    /// </remarks>
    public sealed class EJDB2 : IDisposable
    {
        private bool _disposed;
        
        private readonly Lazy<EJDB2Handle> _handle;        

        public EJDB2(EJDB2Options options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _handle = new Lazy<EJDB2Handle>(() => EJDB2Facade.Instance.Open(Options));
        }

        internal EJDB2Handle Handle => _handle.Value;

        /// <summary>
        /// Returns options used to build this database instance
        /// </summary>
        public EJDB2Options Options { get; }

        /// <summary>
        /// Create a query instance. Query can be reused multiple times with various
        /// placeholder parameters. See JQL specification:
        /// https://github.com/Softmotions/ejdb/blob/master/README.md#jql
        /// </summary>
        /// <remarks>Note: collection name must be encoded in query, eg: <c>@mycoll/[foo=bar]</c></remarks>
        /// <param name="query">JQL query</param>
        /// <returns>JQL query object</returns>
        /// <exception cref="EJDB2Exception"></exception>
        public JQL CreateQuery(string query) => CreateQuery(query, null);

        /// <summary>
        /// Create a query instance. Query can be reused multiple times with various
        /// placeholder parameters. See JQL specification:
        /// https://github.com/Softmotions/ejdb/blob/master/README.md#jql
        /// </summary>
        /// <remarks>
        /// If <c>collection</c> is not null it will be used for query. In this case
        /// collection name encoded in query will not be taken into account.
        /// </remarks>
        /// <param name="query">JQL query</param>
        /// <param name="collection">Optional collection name</param>
        /// <returns>JQL query object</returns>
        /// <exception cref="EJDB2Exception"></exception>
        public JQL CreateQuery(string query, string collection)
        {
            EnsureNotDisposed();
            return JQL.Create(this, query, collection);
        }

        /// <summary>
        /// Persists <c>json</c> document into <c>collection</c>.
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="json">JSON document</param>
        /// <returns>Generated identifier for document</returns>
        /// <exception cref="EJDB2Exception"></exception>
        public long Put(string collection, string json) => Put(collection, json, 0);

        /// <summary>
        /// Persists <c>json</c> document under specified <c>id</c>.
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="json">JSON document</param>
        /// <param name="id">Document id. If zero a new identifier will be genareted</param>
        /// <returns>Generated identifier for document</returns>
        /// <exception cref="EJDB2Exception"></exception>
        public long Put(string collection, string json, long id)
        {
            EnsureNotDisposed();
            return EJDB2Facade.Instance.Put(_handle.Value, collection, json, id);
        }

        /// <summary>
        /// Removes a document identified by given {@code id} from collection {@code coll}.
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="id">Document id</param>
        /// <exception cref="EJDB2Exception">
        /// If document is not found EJDB2Exception will be thrown: code: 75001,
        /// message: Key not found. (IWKV_ERROR_NOTFOUND)
        /// </exception>
        public void Delete(string collection, long id)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.Delete(_handle.Value, collection, id);
        }

        public void RenameCollection(string oldCollectionName, string newCollectionName)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.RenameCollection(_handle.Value, oldCollectionName, newCollectionName);
        }

        /// <summary>
        /// Apply rfc6902/rfc7386 JSON patch to the document identified by <c>id</c>
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="patch">JSON patch</param>
        /// <param name="id">Document id</param>
        public void Patch(string collection, string patch, long id)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.Patch(_handle.Value, collection, patch, id, false);
        }

        /// <summary>
        /// Apply JSON merge patch (rfc7396) to the document identified by `id` or
        /// insert new document under specified `id`.
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="patch">JSON patch</param>
        /// <param name="id">Document id</param>
        public void PatchOrPut(string collection, string patch, long id)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.Patch(_handle.Value, collection, patch, id, true);
        }

        /// <summary>
        /// Returns document identified by <c>id</c> as into <c>writer</c>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <param name="stream"></param>
        /// <param name="prettify"></param>
        /// <returns>Document</returns>
        /// <exception cref="EJDB2Exception">
        /// If document is not found EJDB2Exception will be thrown: code: 75001,
        /// message: Key not found. (IWKV_ERROR_NOTFOUND)
        /// </exception>
        public void Get(string collection, long id, Stream stream, bool prettify = false)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.Get(_handle.Value, collection, id, stream, prettify);
        }

        /// <summary>
        /// Returns document identified by <c>id</c> as into <c>writer</c>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <param name="writer"></param>
        /// <param name="prettify"></param>
        /// <returns>Document</returns>
        /// <exception cref="EJDB2Exception">
        /// If document is not found EJDB2Exception will be thrown: code: 75001,
        /// message: Key not found. (IWKV_ERROR_NOTFOUND)
        /// </exception>
        public string this[string collection, long id] => Get(collection, id);

        /// <summary>
        /// Returns document identified by <c>id</c> as string.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id"></param>
        /// <returns>Document</returns>
        /// <exception cref="EJDB2Exception">
        /// If document is not found EJDB2Exception will be thrown: code: 75001,
        /// message: Key not found. (IWKV_ERROR_NOTFOUND)
        /// </exception>
        public string Get(string collection, long id)
        {
            EnsureNotDisposed();

            using var stream = new MemoryStream();
            EJDB2Facade.Instance.Get(_handle.Value, collection, id, stream, false);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Returns JSON document describind database structure.
        /// </summary>
        /// <remarks>
        /// Example database metadata:
        /// <code>
        ///   {
        ///    "version": "2.0.0", // EJDB engine version
        ///    "file": "db.jb",    // Path to storage file
        ///    "size": 16384,      // Storage file size in bytes
        ///    "collections": [    // List of collections
        ///     {
        ///      "name": "c1",     // Collection name
        ///      "dbid": 3,        // Collection database ID
        ///      "rnum": 2,        // Number of documents in collection
        ///      "indexes": [      // List of collections indexes
        ///       {
        ///        "ptr": "/n",    // rfc6901 JSON pointer to indexed field
        ///        "mode": 8,      // Index mode. Here is EJDB_IDX_I64
        ///        "idbf": 96,     // Index database flags. See iwdb_flags_t
        ///        "dbid": 4,      // Index database ID
        ///        "rnum": 2       // Number records stored in index database
        ///       }
        ///      ]
        ///     }
        ///    ]
        ///   }
        /// </code>
        /// </remarks>
        /// <param name="stream">Stream to write the document</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void GetInfo(Stream stream)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.GetInfo(_handle.Value, stream, false);
        }

        /// <summary>
        /// Returns JSON document describind database structure. Same as
        /// <see cref="GetInfo(TextWriter)"/> but returns JSON data as string.
        /// </summary>
        /// <returns>JSON document</returns>
        /// <exception cref="EJDB2Exception"></exception>
        public string GetInfo()
        {
            EnsureNotDisposed();

            using var stream = new MemoryStream();
            EJDB2Facade.Instance.GetInfo(_handle.Value, stream, false);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Removes collection from database and all its documents.
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void RemoveCollection(string collection)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.RemoveCollection(_handle.Value, collection);
        }

        /// <summary>
        /// Create string index with specified parameters if it has not existed before.
        /// </summary>
        /// <remarks>
        /// <para>Where <c>path</c> must be fully specified as rfc6901 JSON pointer.</para>
        /// <para>Example document:</para>
        /// <para>
        /// <code>
        /// "address" : {
        ///     "street": "High Street"
        /// }
        /// </code>
        /// </para>
        /// <para>
        /// Call <c>EnsureStringIndex("mycoll", "/address/street", true)}</c> in order to
        /// create unique index over all street names in nested address object.
        /// </para>
        /// </remarks>
        /// <param name="collection">Collection name</param>
        /// <param name="path">JSON pointer path to indexed field</param>
        /// <param name="unique"><c>true</c> for unique index</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void EnsureStringIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_STR | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.EnsureIndex(_handle.Value, collection, path, flags);
        }

        /// <summary>
        /// Removes collection index for JSON document field pointed by <c>path</c>
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="path">JSON pointer path to indexed field</param>
        /// <param name="unique"><c>true</c> for unique index</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void RemoveStringIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_STR | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.RemoveIndex(_handle.Value, collection, path, flags);
        }

        /// <summary>
        /// Create integer number with specified parameters if it has not existed before.
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="path">JSON pointer path to indexed field</param>
        /// <param name="unique"><c>true</c> for unique index</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void EnsureIntIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_I64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.EnsureIndex(_handle.Value, collection, path, flags);
        }

        /// <summary>
        /// Removes collection index for JSON document field pointed by <c>path</c>
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="path">JSON pointer path to indexed field</param>
        /// <param name="unique"><c>true</c> for unique index</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void RemoveIntIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_I64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.RemoveIndex(_handle.Value, collection, path, flags);
        }

        /// <summary>
        /// Create floating point number index with specified parameters if it has not existed before.
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="path">JSON pointer path to indexed field</param>
        /// <param name="unique"><c>true</c> for unique index</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void EnsureFloatIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_F64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.EnsureIndex(_handle.Value, collection, path, flags);
        }

        /// <summary>
        /// Removes collection index for JSON document field pointed by <c>path</c>
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="path">JSON pointer path to indexed field</param>
        /// <param name="unique"><c>true</c> for unique index</param>
        /// <exception cref="EJDB2Exception"></exception>
        public void RemoveFloatIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_F64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.RemoveIndex(_handle.Value, collection, path, flags);
        }

        /// <summary>
        /// Creates an online database backup image and copies it into the specified `targetFile`.
        /// During online backup phase read/write database operations are allowed and not
        /// blocked for significant amount of time.Returns backup finish time as number
        /// of milliseconds since epoch.
        /// Online backup guaranties what all records before finish timestamp will
        /// be stored in backup image. Later, online backup image can be
        /// opened as ordinary database file.
        /// </summary>
        /// <remarks>
        /// In order to avoid deadlocks: close all opened database cursors
        /// before calling this method or do call in separate thread.
        /// </remarks>
        /// <param name="targetFile">Backup file path</param>
        /// <returns></returns>
        public ulong OnlineBackup(string targetFile)
        {
            EnsureNotDisposed();
            return EJDB2Facade.Instance.OnlineBackup(_handle.Value, targetFile);
        }

        /// <summary>
        /// Closes database instance and releases all resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_handle.IsValueCreated)
                    _handle.Value.Dispose();

                _disposed = true;
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
