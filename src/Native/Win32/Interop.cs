using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ejdb2.Native.Win32
{
    internal static class Interop
    {
        public const string LibName = "libejdb2";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ejdb_init();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ejdb_version_major();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ejdb_version_minor();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ejdb_version_patch();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_open([In] ref EJDB_OPTS opts, [Out] out IntPtr ejdbp);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_close([In, Out] ref IntPtr ejdbp);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_get_meta([In] IntPtr db, [Out] out IntPtr jblp);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jbl_as_json(IntPtr jbl,
            [MarshalAs(UnmanagedType.FunctionPtr)] jbl_json_printer pt,
            IntPtr op, jbl_print_flags_t pf);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jbl_from_json([Out] out IntPtr jblp,
            [MarshalAs(UnmanagedType.LPStr)] string jsonstr);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_put(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, IntPtr jbl, long id);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_put_new(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, IntPtr jbl, [Out] out long oid);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jbl_destroy([In, Out] ref IntPtr jblp);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_del(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, long id);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_get(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, long id, [Out] out IntPtr jblp);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_rename_collection(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, [MarshalAs(UnmanagedType.LPStr)] string new_coll);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_merge_or_put(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string patchjson, long id);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_patch(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string patchjson, long id);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_ensure_index(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string path, ejdb_idx_mode_t mode);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_remove_index(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string path, ejdb_idx_mode_t mode);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_online_backup(IntPtr db,
            [Out] out ulong ts, [MarshalAs(UnmanagedType.LPStr)] string target_file);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_remove_collection(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string coll);
    }
}
