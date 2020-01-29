using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ejdb2.Native
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal delegate string IWLOG_ECODE_FN(IntPtr locale, uint ecode);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal delegate ulong jbl_json_printer(
        [MarshalAs(UnmanagedType.LPStr)] string data, int size,
        char ch, int count, IntPtr op);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void freefn(IntPtr ptr, IntPtr op);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate ulong EJDB_EXEC_VISITOR(ref EJDB_EXEC ctx, ref EJDB_DOC doc, out long step);

    [StructLayout(LayoutKind.Sequential)]
    internal struct EJDB_OPTS
    {
        public IWKV_OPTS kv;
        public EJDB_HTTP http;
        public bool no_wal;
        public uint sort_buffer_sz;
        public uint document_buffer_sz;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EJDB_HTTP
    {
        public bool enabled;
        public int port;
        public string bind;
        public string access_token;
        public UIntPtr access_token_len;
        public bool blocking;
        public bool read_anon;
        public UIntPtr max_body_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IWKV_OPTS
    {
        public string path;
        public uint random_seed;
        public iwkv_openflags oflags;
        public bool file_lock_fail_fast;
        public IWKV_WAL_OPTS wal;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IWKV_WAL_OPTS
    {
        public bool enabled;
        public bool check_crc_on_checkpoint;
        public uint savepoint_timeout_sec;
        public uint checkpoint_timeout_sec;
        public UIntPtr wal_buffer_sz;
        public ulong checkpoint_buffer_sz;
        public IntPtr wal_lock_interceptor;
        public IntPtr wal_lock_interceptor_opaque;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct JBL_NODE
    {
        public IntPtr next;
        public IntPtr prev;
        public IntPtr parent;
        public IntPtr key;
        public int klidx;
        public uint flags;
        public IntPtr child;
        public jbl_type_t type;
        public JBL_NODE_VALUE value;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct JBL_NODE_VALUE
    {
        [FieldOffset(0)]
        public IntPtr vptr;

        [FieldOffset(0)]
        public bool vbool;

        [FieldOffset(0)]
        public long vi64;

        [FieldOffset(0)]
        public double vf64;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EJDB_EXEC
    {
        public IntPtr db;
        public IntPtr q;
        public EJDB_EXEC_VISITOR visitor;
        public IntPtr opaque;
        public long skip;
        public long limit;
        public long cnt;
        public IntPtr log;
        public IntPtr pool;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EJDB_DOC
    {
        public long id;
        public IntPtr raw;
        public IntPtr node;
        public IntPtr next;
        public IntPtr prev;
    }

    [Flags]
    internal enum iwkv_openflags : byte
    {
        IWKV_RDONLY = 0x02,
        IWKV_TRUNC = 0x04,
    }

    [Flags]
    internal enum jbl_print_flags_t : byte
    {
        JBL_PRINT_NONE = 0,
        JBL_PRINT_PRETTY = 0x01,
        JBL_PRINT_CODEPOINTS = 0x02,
    }

    [Flags]
    internal enum ejdb_idx_mode_t : byte
    {
        EJDB_IDX_UNIQUE = 0x01,
        EJDB_IDX_STR = 0x04,
        EJDB_IDX_I64 = 0x08,
        EJDB_IDX_F64 = 0x10,
    }

    [Flags]
    internal enum jql_create_mode_t : byte
    {
        JQL_KEEP_QUERY_ON_PARSE_ERROR = 0x01,
        JQL_SILENT_ON_PARSE_ERROR = 0x02,
    }

    internal enum jql_ecode_t : ulong
    {
        _JQL_ERROR_START = (NativeConstants.IW_ERROR_START + 15000UL + 2000),
        JQL_ERROR_QUERY_PARSE,
        JQL_ERROR_INVALID_PLACEHOLDER,
        JQL_ERROR_UNSET_PLACEHOLDER,
        JQL_ERROR_REGEXP_INVALID,
        JQL_ERROR_REGEXP_CHARSET,
        JQL_ERROR_REGEXP_SUBEXP,
        JQL_ERROR_REGEXP_SUBMATCH,
        JQL_ERROR_REGEXP_ENGINE,
        JQL_ERROR_SKIP_ALREADY_SET,
        JQL_ERROR_LIMIT_ALREADY_SET,
        JQL_ERROR_ORDERBY_MAX_LIMIT,
        JQL_ERROR_NO_COLLECTION,
        JQL_ERROR_INVALID_PLACEHOLDER_VALUE_TYPE,
        _JQL_ERROR_END,
        _JQL_ERROR_UNMATCHED,
    }

    internal enum jbl_type_t
    {
        JBV_NONE = 0,
        JBV_NULL,
        JBV_BOOL,
        JBV_I64,
        JBV_F64,
        JBV_STR,
        JBV_OBJECT,
        JBV_ARRAY,
    }
}
