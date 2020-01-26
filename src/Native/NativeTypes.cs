using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ejdb2.Native
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal delegate ulong jbl_json_printer(
        [MarshalAs(UnmanagedType.LPStr)] string data, int size,
        char ch, int count, IntPtr op);

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
}
