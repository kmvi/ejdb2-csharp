using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Ejdb2.Native.Win32;

namespace Ejdb2.Native
{
    internal sealed class EJDB2Facade
    {
        private static readonly Lazy<EJDB2Facade> _instance = new Lazy<EJDB2Facade>(Initialize);

        private readonly INativeHelper _helper;

        private EJDB2Facade(INativeHelper helper)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public Version GetVersion() => new Version(
            _helper.ejdb_version_major(),
            _helper.ejdb_version_minor(),
            _helper.ejdb_version_patch()
        );

        public EJDB2Handle Open(EJDB2Options options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            EJDB_OPTS opts = MapOptions(options);

            ulong rc = _helper.ejdb_open(ref opts, out IntPtr handle);
            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_open failed.");

            return new EJDB2Handle(handle);
        }

        public void Delete(EJDB2Handle handle, string collection, long id)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            ulong rc = _helper.ejdb_del(handle.DangerousGetHandle(), collection, id);
            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_del failed.");
        }

        public void Patch(EJDB2Handle handle, string collection, string patch, long id, bool upsert)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            IntPtr db = handle.DangerousGetHandle();

            if (upsert)
            {
                ulong rc = _helper.ejdb_merge_or_put(db, collection, patch, id);
                if (rc != 0)
                    throw new EJDB2Exception(rc, "ejdb_merge_or_put failed.");
            }
            else
            {
                ulong rc = _helper.ejdb_patch(db, collection, patch, id);
                if (rc != 0)
                    throw new EJDB2Exception(rc, "ejdb_patch failed.");
            }
        }

        public void RenameCollection(EJDB2Handle handle, string oldCollectionName, string newCollectionName)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (oldCollectionName == null)
                throw new ArgumentNullException(nameof(oldCollectionName));

            if (newCollectionName == null)
                throw new ArgumentNullException(nameof(newCollectionName));

            ulong rc = _helper.ejdb_rename_collection(handle.DangerousGetHandle(),
                oldCollectionName, newCollectionName);

            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_rename_collection failed.");
        }

        public void RemoveIndex(EJDB2Handle handle, string collection, string path, ejdb_idx_mode_t mode)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            ulong rc = _helper.ejdb_remove_index(handle.DangerousGetHandle(),
                collection, path, mode);

            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_remove_index failed.");
        }

        public void EnsureIndex(EJDB2Handle handle, string collection, string path, ejdb_idx_mode_t mode)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            ulong rc = _helper.ejdb_ensure_index(handle.DangerousGetHandle(),
                collection, path, mode);

            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_ensure_index failed.");
        }

        public void Get(EJDB2Handle handle, string collection, long id, TextWriter writer, bool pretty)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            IntPtr db = handle.DangerousGetHandle(), jbl = IntPtr.Zero;

            try
            {
                ulong rc = _helper.ejdb_get(db, collection, id, out jbl);
                if (rc != 0)
                    throw new EJDB2Exception(rc, "ejdb_get failed.");

                var printer = new Printer(writer);

                rc = _helper.jbl_as_json(jbl, printer.JbnJsonPrinter, IntPtr.Zero,
                    pretty ? jbl_print_flags_t.JBL_PRINT_PRETTY : jbl_print_flags_t.JBL_PRINT_NONE);

                if (rc != 0)
                    throw new EJDB2Exception(rc, "jbl_as_json failed.");

                writer.Flush();
            }
            finally
            {
                if (jbl != IntPtr.Zero)
                    _helper.jbl_destroy(ref jbl);
            }
        }

        public ulong OnlineBackup(EJDB2Handle handle, string targetFile)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (targetFile == null)
                throw new ArgumentNullException(nameof(targetFile));

            ulong rc = _helper.ejdb_online_backup(handle.DangerousGetHandle(),
                out ulong ts, targetFile);

            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_online_backup failed.");

            return ts;
        }

        public void RemoveCollection(EJDB2Handle handle, string collection)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            ulong rc = _helper.ejdb_remove_collection(handle.DangerousGetHandle(), collection);
            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_remove_collection failed.");
        }

        public long Put(EJDB2Handle handle, string collection, string json, long id)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (json == null)
                throw new ArgumentNullException(nameof(collection));

            long ret = id;
            IntPtr db = handle.DangerousGetHandle(), jbl = IntPtr.Zero;

            try
            {
                ulong rc = _helper.jbl_from_json(out jbl, json);
                if (rc != 0)
                    throw new EJDB2Exception(rc, "jbl_from_json failed.");

                if (id > 0)
                {
                    rc = _helper.ejdb_put(db, collection, jbl, id);
                    if (rc != 0)
                        throw new EJDB2Exception(rc, "ejdb_put failed.");
                }
                else
                {
                    rc = _helper.ejdb_put_new(db, collection, jbl, out ret);
                    if (rc != 0)
                        throw new EJDB2Exception(rc, "ejdb_put_new failed.");
                }

                return ret;
            }
            finally
            {
                if (jbl != IntPtr.Zero)
                    _helper.jbl_destroy(ref jbl);
            }
        }

        public ulong Close(EJDB2Handle handle)
        {
            if (!handle.IsInvalid)
            {
                IntPtr db = handle.DangerousGetHandle();
                return _helper.ejdb_close(ref db);
            }

            return 0;
        }

        public void GetInfo(EJDB2Handle handle, TextWriter writer, bool pretty)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            IntPtr db = handle.DangerousGetHandle(), jbl = IntPtr.Zero;

            try
            {
                ulong rc = _helper.ejdb_get_meta(db, out jbl);
                if (rc != 0)
                    throw new EJDB2Exception(rc, "jbl_as_json failed.");

                var printer = new Printer(writer);

                rc = _helper.jbl_as_json(jbl, printer.JbnJsonPrinter, IntPtr.Zero,
                    pretty ? jbl_print_flags_t.JBL_PRINT_PRETTY : jbl_print_flags_t.JBL_PRINT_NONE);

                if (rc != 0)
                    throw new EJDB2Exception(rc, "jbl_as_json failed.");

                writer.Flush();
            }
            finally
            {
                if (jbl != IntPtr.Zero)
                    _helper.jbl_destroy(ref jbl);
            }
        }

        public static EJDB2Facade Instance => _instance.Value;

        private static EJDB2Facade Initialize()
        {
            INativeHelper helper = NativeHelper.Create();

            ulong rc = helper.ejdb_init();
            if (rc != 0)
                throw new EJDB2Exception(rc, "ejdb_init failed.");

            return new EJDB2Facade(helper);
        }

        private static EJDB_OPTS MapOptions(EJDB2Options options)
        {
            var result = new EJDB_OPTS();

            result.no_wal = !options.UseWAL;
            result.sort_buffer_sz = options.SortBufferSize;
            result.document_buffer_sz = options.DocumentBufferSize;
            result.kv.random_seed = options.IWKVOptions.RandomSeed;
            result.kv.oflags = options.IWKVOptions.OpenFlags;
            result.kv.file_lock_fail_fast = options.IWKVOptions.FileLockFailFast;
            result.kv.path = options.IWKVOptions.Path;
            result.kv.wal.check_crc_on_checkpoint = options.IWKVOptions.WALOptions.CheckCRCOnCheckpoint;
            result.kv.wal.savepoint_timeout_sec = options.IWKVOptions.WALOptions.SavePointTimeoutSec;
            result.kv.wal.checkpoint_timeout_sec = options.IWKVOptions.WALOptions.CheckPointTimeoutSec;
            result.kv.wal.wal_buffer_sz = new UIntPtr(options.IWKVOptions.WALOptions.BufferSize);
            result.kv.wal.checkpoint_buffer_sz = options.IWKVOptions.WALOptions.CheckpointBufferSize;
            result.http.enabled = options.HttpOptions.Enabled;
            result.http.port = options.HttpOptions.Port;
            result.http.bind = options.HttpOptions.Bind;
            result.http.access_token = options.HttpOptions.AccessToken;
            result.http.access_token_len = new UIntPtr((uint)(options.HttpOptions.AccessToken?.Length ?? 0));
            result.http.read_anon = options.HttpOptions.ReadAnon;
            result.http.max_body_size = new UIntPtr(options.HttpOptions.MaxBodySize);

            return result;
        }

        private sealed class Printer
        {
            private readonly TextWriter _writer;

            public Printer(TextWriter writer)
            {
                _writer = writer;
            }

            public ulong JbnJsonPrinter(string data, int size, char ch, int count, IntPtr op)
            {
                if (data == null)
                {
                    for (int i = 0; i < count; ++i)
                        _writer.Write(ch);
                }
                else
                {
                    if (count <= 0)
                        count = 1;

                    for (int i = 0; i < count; ++i)
                        _writer.Write(data);
                }

                return 0;
            }
        }
    }
}
