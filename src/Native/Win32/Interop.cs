using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ejdb2.Native.Win32
{
    internal static class Interop
    {
        public const string EjdbLibName = "libejdb2";
        public const string IowowLibName = "libiowow";

        #region libejdb2

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ejdb_init();

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ejdb_version_major();

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ejdb_version_minor();

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ejdb_version_patch();

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_open([In] ref EJDB_OPTS opts, [Out] out IntPtr ejdbp);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_close([In, Out] ref IntPtr ejdbp);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_get_meta([In] IntPtr db, [Out] out IntPtr jblp);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jbl_as_json(IntPtr jbl,
            [MarshalAs(UnmanagedType.FunctionPtr)] jbl_json_printer pt,
            IntPtr op, jbl_print_flags_t pf);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jbl_node_as_json(IntPtr node, jbl_json_printer pt,
            IntPtr op, jbl_print_flags_t pf);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jbl_from_json([Out] out IntPtr jblp,
            [MarshalAs(UnmanagedType.LPStr)] string jsonstr);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jbl_xstr_json_printer(
            [MarshalAs(UnmanagedType.LPStr)] string data, int size, char ch, int count, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_put(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, IntPtr jbl, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_put_new(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, IntPtr jbl, [Out] out long oid);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jbl_destroy([In, Out] ref IntPtr jblp);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jql_destroy([In, Out] ref IntPtr qptr);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_del(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_get(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, long id, [Out] out IntPtr jblp);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_rename_collection(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll, [MarshalAs(UnmanagedType.LPStr)] string new_coll);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_merge_or_put(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string patchjson, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_patch(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string patchjson, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_ensure_index(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string path, ejdb_idx_mode_t mode);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_remove_index(IntPtr db,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string path, ejdb_idx_mode_t mode);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_online_backup(IntPtr db,
            [Out] out ulong ts, [MarshalAs(UnmanagedType.LPStr)] string target_file);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong ejdb_remove_collection(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string coll);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_create2([Out] out IntPtr qptr,
            [MarshalAs(UnmanagedType.LPStr)] string coll,
            [MarshalAs(UnmanagedType.LPStr)] string query, jql_create_mode_t mode);
        
        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jql_error(IntPtr q);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_get_skip(IntPtr q, [Out] out long skip);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jql_reset(IntPtr q, bool reset_match_cache, bool reset_placeholders);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_get_limit(IntPtr q, [Out] out long limit);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_set_regexp2(IntPtr q,
            [MarshalAs(UnmanagedType.LPStr)] string placeholder, int index,
            IntPtr expr, freefn freefn, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_set_str2(IntPtr q, [MarshalAs(UnmanagedType.LPStr)] string placeholder,
            int index, IntPtr val, freefn freefn, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_set_json2(IntPtr q, [MarshalAs(UnmanagedType.LPStr)] string placeholder,
            int index, ref JBL_NODE val, freefn freefn, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_set_i64(IntPtr q,
            [MarshalAs(UnmanagedType.LPStr)] string placeholder, int index, long val);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_set_f64(IntPtr q,
            [MarshalAs(UnmanagedType.LPStr)] string placeholder, int index, double val);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_set_bool(IntPtr q,
            [MarshalAs(UnmanagedType.LPStr)] string placeholder, int index, bool val);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jql_set_null(IntPtr q,
            [MarshalAs(UnmanagedType.LPStr)] string placeholder, int index);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ulong jbl_node_from_json([MarshalAs(UnmanagedType.LPStr)] string json,
            ref JBL_NODE node, ref IntPtr pool);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jql_collection(IntPtr q);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_exec([In] ref EJDB_EXEC ux);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr jbl_size(IntPtr jbl);

        #endregion

        #region libiowow

        [DllImport(IowowLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwxstr_new();

        [DllImport(IowowLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwxstr_new2(UIntPtr siz);

        [DllImport(IowowLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void iwxstr_destroy(IntPtr xstr);

        [DllImport(IowowLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr iwxstr_size(IntPtr xstr);

        [DllImport(IowowLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwxstr_ptr(IntPtr xstr);

        #endregion
    }
}
