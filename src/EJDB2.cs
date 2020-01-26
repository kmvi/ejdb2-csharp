using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ejdb2.Native;

namespace Ejdb2
{
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

        public EJDB2Options Options { get; }

        public JQL CreateQuery(string query) => CreateQuery(query, null);

        public JQL CreateQuery(string query, string collection)
        {
            EnsureNotDisposed();
            return new JQL(this, query, collection);
        }

        public long Put(string collection, string json) => Put(collection, json, 0);

        public long Put(string collection, string json, long id)
        {
            EnsureNotDisposed();
            return EJDB2Facade.Instance.Put(_handle.Value, collection, json, id);
        }

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

        public void Patch(string collection, string patch, long id)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.Patch(_handle.Value, collection, patch, id, false);
        }

        public void PatchOrPut(string collection, string patch, long id)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.Patch(_handle.Value, collection, patch, id, true);
        }

        public void Get(string collection, long id, TextWriter writer, bool prettify = false)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.Get(_handle.Value, collection, id, writer, prettify);
        }

        public string this[string collection, long id] => Get(collection, id);

        public string Get(string collection, long id)
        {
            EnsureNotDisposed();

            using var writer = new StringWriter();
            EJDB2Facade.Instance.Get(_handle.Value, collection, id, writer, false);

            return writer.ToString();
        }

        public void GetInfo(TextWriter writer)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.GetInfo(_handle.Value, writer, false);
        }

        public string GetInfo()
        {
            EnsureNotDisposed();

            using var writer = new StringWriter();
            EJDB2Facade.Instance.GetInfo(_handle.Value, writer, false);

            return writer.ToString();
        }

        public void RemoveCollection(string collection)
        {
            EnsureNotDisposed();
            EJDB2Facade.Instance.RemoveCollection(_handle.Value, collection);
        }

        public void EnsureStringIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_STR | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.EnsureIndex(_handle.Value, collection, path, flags);
        }

        public void RemoveStringIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_STR | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.RemoveIndex(_handle.Value, collection, path, flags);
        }

        public void EnsureIntIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_I64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.EnsureIndex(_handle.Value, collection, path, flags);
        }

        public void RemoveIntIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_I64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.RemoveIndex(_handle.Value, collection, path, flags);
        }

        public void EnsureFloatIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_F64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.EnsureIndex(_handle.Value, collection, path, flags);
        }

        public void RemoveFloatIndex(string collection, string path, bool unique)
        {
            EnsureNotDisposed();
            var flags = ejdb_idx_mode_t.EJDB_IDX_F64 | (unique ? ejdb_idx_mode_t.EJDB_IDX_UNIQUE : 0);
            EJDB2Facade.Instance.RemoveIndex(_handle.Value, collection, path, flags);
        }

        public ulong OnlineBackup(string targetFile)
        {
            EnsureNotDisposed();
            return EJDB2Facade.Instance.OnlineBackup(_handle.Value, targetFile);
        }

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
