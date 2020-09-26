using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ejdb2.Native
{
    internal sealed class EJDB2Facade
    {
        private static readonly Lazy<EJDB2Facade> _instance = new Lazy<EJDB2Facade>(Initialize);

        private readonly INativeHelper _helper;
        private readonly ExceptionHelper _e;

        private EJDB2Facade(INativeHelper helper)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _e = new ExceptionHelper(helper);
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

            IntPtr handle;
            ulong rc;

            using (var pool = new Utf8StringPool())
            {
                EJDB_OPTS opts = MapOptions(options, pool);
                rc = _helper.ejdb_open(ref opts, out handle);
            }

            if (rc != 0)
                throw _e.CreateException(rc);

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

            ulong rc;
            using (var col = new Utf8String(collection))
            {
                rc = _helper.ejdb_del(handle.DangerousGetHandle(), col, id);
            }

            if (rc != 0)
                throw _e.CreateException(rc);
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

            ulong rc;
            using (var pool = new Utf8StringPool())
            {
                rc = upsert
                    ? _helper.ejdb_merge_or_put(db, pool.GetString(collection), pool.GetString(patch), id)
                    : _helper.ejdb_patch(db, pool.GetString(collection), pool.GetString(patch), id);
            }

            if (rc != 0)
                throw _e.CreateException(rc);
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

            ulong rc;
            using (var pool = new Utf8StringPool())
            {
                rc = _helper.ejdb_rename_collection(handle.DangerousGetHandle(),
                    pool.GetString(oldCollectionName), pool.GetString(newCollectionName));
            }

            if (rc != 0)
                throw _e.CreateException(rc);
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

            ulong rc;
            using (var pool = new Utf8StringPool())
            {
                rc = _helper.ejdb_remove_index(handle.DangerousGetHandle(),
                        pool.GetString(collection), pool.GetString(path), mode);
            }

            if (rc != 0)
                throw _e.CreateException(rc);
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

            ulong rc;
            using (var pool = new Utf8StringPool())
            {
                rc = _helper.ejdb_ensure_index(handle.DangerousGetHandle(),
                        pool.GetString(collection), pool.GetString(path), mode);
            }

            if (rc != 0)
                throw _e.CreateException(rc);
        }

        public void Get(EJDB2Handle handle, string collection, long id, Stream stream, bool pretty)
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
                ulong rc;
                using (var col = new Utf8String(collection))
                {
                    rc = _helper.ejdb_get(db, col, id, out jbl);
                }

                if (rc != 0)
                    throw _e.CreateException(rc);

                var printer = new Printer(stream);

                rc = _helper.jbl_as_json(jbl, printer.JbnJsonPrinter, IntPtr.Zero,
                    pretty ? jbl_print_flags_t.JBL_PRINT_PRETTY : jbl_print_flags_t.JBL_PRINT_NONE);

                if (rc != 0)
                    throw _e.CreateException(rc);

                stream.Flush();
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

            ulong rc, ts;
            using (var tf = new Utf8String(targetFile))
            {
                rc = _helper.ejdb_online_backup(handle.DangerousGetHandle(), out ts, tf);
            }

            if (rc != 0)
                throw _e.CreateException(rc);

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

            ulong rc;
            using (var col = new Utf8String(collection))
            {
                rc = _helper.ejdb_remove_collection(handle.DangerousGetHandle(), col);
            }

            if (rc != 0)
                throw _e.CreateException(rc);
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
                using (var pool = new Utf8StringPool())
                {
                    ulong rc = _helper.jbl_from_json(out jbl, pool.GetString(json));
                    if (rc != 0)
                        throw _e.CreateException(rc);

                    rc = id > 0
                        ? _helper.ejdb_put(db, pool.GetString(collection), jbl, id)
                        : _helper.ejdb_put_new(db, pool.GetString(collection), jbl, out ret);

                    if (rc != 0)
                        throw _e.CreateException(rc);
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

        public void GetInfo(EJDB2Handle handle, Stream stream, bool pretty)
        {
            if (handle.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (handle.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            IntPtr db = handle.DangerousGetHandle(), jbl = IntPtr.Zero;

            try
            {
                ulong rc = _helper.ejdb_get_meta(db, out jbl);
                if (rc != 0)
                    throw _e.CreateException(rc);

                var printer = new Printer(stream);

                rc = _helper.jbl_as_json(jbl, printer.JbnJsonPrinter, IntPtr.Zero,
                    pretty ? jbl_print_flags_t.JBL_PRINT_PRETTY : jbl_print_flags_t.JBL_PRINT_NONE);

                if (rc != 0)
                    throw _e.CreateException(rc);

                stream.Flush();
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
            var e = new ExceptionHelper(helper);

            ulong rc = helper.ejdb_init();
            if (rc != 0)
                throw new InvalidOperationException("ejdb_init failed. Error code: " + rc);

            return new EJDB2Facade(helper);
        }

        private static EJDB_OPTS MapOptions(EJDB2Options options, Utf8StringPool pool)
        {
            var result = new EJDB_OPTS();

            result.no_wal = !options.UseWAL;
            result.sort_buffer_sz = options.SortBufferSize;
            result.document_buffer_sz = options.DocumentBufferSize;
            result.kv.random_seed = options.IWKVOptions.RandomSeed;
            result.kv.oflags = options.IWKVOptions.OpenFlags;
            result.kv.file_lock_fail_fast = options.IWKVOptions.FileLockFailFast;
            result.kv.path = pool.GetString(options.IWKVOptions.Path);
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
            private readonly Stream _stream;

            public Printer(Stream stream)
            {
                _stream = stream;
            }

            public unsafe ulong JbnJsonPrinter(IntPtr data, int size, byte ch, int count, IntPtr op)
            {
                if (data == IntPtr.Zero)
                {
                    for (int i = 0; i < count; ++i)
                        _stream.WriteByte(ch);
                }
                else
                {
                    if (count <= 0)
                        count = 1;

                    for (int i = 0; i < count; ++i)
                    {
                        var len = Utf8String.StrLen(data);
                        if (len > 0)
                        {
                            using (var ms = new UnmanagedMemoryStream((byte*)data, len))
                                ms.CopyTo(_stream);
                        }
                    }
                }

                return 0;
            }
        }
    }
}
