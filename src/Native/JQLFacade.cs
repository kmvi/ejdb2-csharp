using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ejdb2.Native
{
    internal sealed class JQLFacade
    {
        private static readonly Lazy<JQLFacade> _instance =
            new Lazy<JQLFacade>(() => new JQLFacade(NativeHelper.Create()));

        private readonly INativeHelper _helper;
        private readonly ExceptionHelper _e;

        private JQLFacade(INativeHelper helper)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _e = new ExceptionHelper(helper);
        }

        public EJDB2Handle Init(EJDB2Handle db, string query, ref string collection)
        {
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            if (query == null)
                throw new ArgumentNullException(nameof(query));

            IntPtr q = IntPtr.Zero;

            try
            {
                ulong rc = _helper.jql_create2(out q, collection, query,
                    jql_create_mode_t.JQL_KEEP_QUERY_ON_PARSE_ERROR | jql_create_mode_t.JQL_SILENT_ON_PARSE_ERROR);

                if (rc != 0)
                {
                    string message = null;
                    if (rc == (ulong)jql_ecode_t.JQL_ERROR_QUERY_PARSE)
                        message = $"Query parse error: {Marshal.PtrToStringAnsi(_helper.jql_error(q))}";
                    
                    throw _e.CreateException(rc, message);
                }

                if (collection == null)
                {
                    IntPtr col = _helper.jql_collection(q);
                    collection = Marshal.PtrToStringAnsi(col);
                }

                return new EJDB2Handle(q);
            }
            catch (Exception)
            {
                if (q != IntPtr.Zero)
                    _helper.jql_destroy(ref q);

                throw;
            }
        }

        public long GetSkip(EJDB2Handle jql)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            ulong rc = _helper.jql_get_skip(jql.DangerousGetHandle(), out long skip);
            if (rc != 0)
                throw _e.CreateException(rc);

            return skip;
        }

        public long GetLimit(EJDB2Handle jql)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            ulong rc = _helper.jql_get_limit(jql.DangerousGetHandle(), out long limit);
            if (rc != 0)
                throw _e.CreateException(rc);

            return limit;
        }

        public void SetString(EJDB2Handle jql, int pos, string placeholder, string val, SetStringType type)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            if (val == null)
                throw new ArgumentNullException(nameof(val));

            IntPtr q = jql.DangerousGetHandle();

            if (type == SetStringType.Json)
            {
                IntPtr pool = IntPtr.Zero;
                
                try
                {
                    pool = _helper.iwpool_create((UIntPtr)1024);
                    if (pool == IntPtr.Zero)
                        throw new InvalidOperationException("iwpool_create failed.");

                    ulong rc = _helper.jbl_node_from_json(val, out JBL_NODE node, ref pool);
                    if (rc != 0)
                        throw _e.CreateException(rc);

                    rc = _helper.jql_set_json2(q, placeholder, pos, ref node, FreePool, pool);
                    if (rc != 0)
                        throw _e.CreateException(rc);
                }
                catch (Exception)
                {
                    FreePool(pool, IntPtr.Zero);
                }
            }
            else if (type == SetStringType.Regexp)
            {
                IntPtr str = IntPtr.Zero;

                try
                {
                    str = Marshal.StringToHGlobalAnsi(val);
                    ulong rc = _helper.jql_set_regexp2(q, placeholder, pos, str, FreeStringMem, IntPtr.Zero);
                    if (rc != 0)
                        throw _e.CreateException(rc);
                }
                catch (Exception)
                {
                    FreeStringMem(str, IntPtr.Zero);
                }
            }
            else
            {
                IntPtr str = IntPtr.Zero;

                try
                {
                    str = Marshal.StringToHGlobalAnsi(val);
                    ulong rc = _helper.jql_set_str2(q, placeholder, pos, str, FreeStringMem, IntPtr.Zero);
                    if (rc != 0)
                        throw _e.CreateException(rc);
                }
                catch (Exception)
                {
                    FreeStringMem(str, IntPtr.Zero);
                }
            }
        }

        private void FreePool(IntPtr ptr, IntPtr op)
        {
            if (ptr != IntPtr.Zero)
                _helper.iwpool_destroy(ptr);
        }

        private static void FreeStringMem(IntPtr ptr, IntPtr op)
        {
            if (ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr);
        }

        public void SetLong(EJDB2Handle jql, int pos, string placeholder, long val)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            ulong rc = _helper.jql_set_i64(jql.DangerousGetHandle(), placeholder, pos, val);
            if (rc != 0)
                throw _e.CreateException(rc);
        }

        public void SetDouble(EJDB2Handle jql, int pos, string placeholder, double val)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            ulong rc = _helper.jql_set_f64(jql.DangerousGetHandle(), placeholder, pos, val);
            if (rc != 0)
                throw _e.CreateException(rc);
        }

        public void SetBoolean(EJDB2Handle jql, int pos, string placeholder, bool val)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            ulong rc = _helper.jql_set_bool(jql.DangerousGetHandle(), placeholder, pos, val);
            if (rc != 0)
                throw _e.CreateException(rc);
        }

        public void SetNull(EJDB2Handle jql, int pos, string placeholder)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            ulong rc = _helper.jql_set_null(jql.DangerousGetHandle(), placeholder, pos);
            if (rc != 0)
                throw _e.CreateException(rc);
        }

        public void Execute(EJDB2Handle db, EJDB2Handle jql, long skip, long limit,
            JQLCallback cb, StringWriter explain)
        {
            if (db.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (db.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            IntPtr log = IntPtr.Zero;

            try
            {
                if (explain != null)
                {
                    log = _helper.iwxstr_new();
                    if (log == IntPtr.Zero)
                        throw new InvalidOperationException("iwxstr_new failed.");
                }

                var ux = new EJDB_EXEC
                {
                    db = db.DangerousGetHandle(),
                    q = jql.DangerousGetHandle(),
                    skip = skip > 0 ? skip : 0,
                    limit = limit > 0 ? limit : 0,
                    opaque = IntPtr.Zero,
                    visitor = null,
                    log = log,
                };

                if (cb != null)
                {
                    var visitor = new ExecuteVisitor(_helper, cb);
                    ux.visitor = visitor.Visitor;
                }

                ulong rc = _helper.ejdb_exec(ref ux);
                if (rc != 0)
                    throw _e.CreateException(rc);

                if (log != IntPtr.Zero)
                {
                    string slog = Marshal.PtrToStringAnsi(_helper.iwxstr_ptr(log));
                    explain.Write(slog);
                }
            }
            finally
            {
                if (log != IntPtr.Zero)
                    _helper.iwxstr_destroy(log);
            }
        }

        public void Reset(EJDB2Handle jql)
        {
            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            _helper.jql_reset(jql.DangerousGetHandle(), true, true);
        }

        public void Destroy(EJDB2Handle handle)
        {
            if (!handle.IsInvalid)
            {
                IntPtr jql = handle.DangerousGetHandle();
                _helper.jql_destroy(ref jql);
            }
        }

        public long ExecuteScalarInt64(EJDB2Handle db, EJDB2Handle jql,
            long skip, long limit, StringWriter explain)
        {
            if (db.IsInvalid)
                throw new ArgumentException("Invalid DB handle.");

            if (db.IsClosed)
                throw new ArgumentException("DB handle is closed.");

            if (jql.IsInvalid)
                throw new ArgumentException("Invalid JQL handle.");

            if (jql.IsClosed)
                throw new ArgumentException("JQL handle is closed.");

            IntPtr log = IntPtr.Zero;

            try
            {
                if (explain != null)
                {
                    log = _helper.iwxstr_new();
                    if (log == IntPtr.Zero)
                        throw new InvalidOperationException("iwxstr_new failed.");
                }

                var ux = new EJDB_EXEC
                {
                    db = db.DangerousGetHandle(),
                    q = jql.DangerousGetHandle(),
                    skip = skip > 0 ? skip : 0,
                    limit = limit > 0 ? limit : 0,
                    opaque = IntPtr.Zero,
                    visitor = null,
                    log = log,
                };

                ulong rc = _helper.ejdb_exec(ref ux);
                if (rc != 0)
                    throw _e.CreateException(rc);

                if (log != IntPtr.Zero)
                {
                    string slog = Marshal.PtrToStringAnsi(_helper.iwxstr_ptr(log));
                    explain.Write(slog);
                }

                return ux.cnt;
            }
            finally
            {
                if (log != IntPtr.Zero)
                    _helper.iwxstr_destroy(log);
            }
        }

        public static JQLFacade Instance => _instance.Value;

        public enum SetStringType
        {
            Json,
            Regexp,
            Other,
        }

        private class ExecuteVisitor
        {
            private readonly INativeHelper _helper;
            private readonly JQLCallback _callback;
            private readonly ExceptionHelper _e;

            public ExecuteVisitor(INativeHelper helper, JQLCallback callback)
            {
                _helper = helper;
                _callback = callback;
                _e = new ExceptionHelper(helper);
            }

            public ulong Visitor(ref EJDB_EXEC ctx, ref EJDB_DOC doc, out long step)
            {
                ulong rc = 0;
                IntPtr xstr = IntPtr.Zero;
                step = 0;

                try
                {
                    uint sz = _helper.jbl_size(doc.raw).ToUInt32() * 2;
                    xstr = _helper.iwxstr_new2(new UIntPtr(sz));

                    if (xstr == IntPtr.Zero)
                        throw new InvalidOperationException("iwxstr_new2 failed.");

                    if (doc.node != IntPtr.Zero)
                    {
                        rc = _helper.jbl_node_as_json(doc.node, _helper.jbl_xstr_json_printer,
                            xstr, jbl_print_flags_t.JBL_PRINT_NONE);

                        if (rc != 0)
                            throw _e.CreateException(rc);
                    }
                    else
                    {
                        rc = _helper.jbl_as_json(doc.raw, _helper.jbl_xstr_json_printer,
                            xstr, jbl_print_flags_t.JBL_PRINT_NONE);
                        
                        if (rc != 0)
                            throw _e.CreateException(rc);
                    }

                    string json = Marshal.PtrToStringAnsi(_helper.iwxstr_ptr(xstr));
                    long llv = _callback(doc.id, json);
                    step = llv < -2 ? 0 : llv;
                }
                finally
                {
                    if (xstr != IntPtr.Zero)
                        _helper.iwxstr_destroy(xstr);
                }

                return rc;
            }
        }
    }
}
