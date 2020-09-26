using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ejdb2.Native.Linux
{
    internal static class Interop
    {
        public const string EjdbLibName = "libejdb2";

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

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jbl_as_json(
            IntPtr jbl, jbl_json_printer pt, IntPtr op, jbl_print_flags_t pf);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jbn_as_json(
            IntPtr node, jbl_json_printer pt, IntPtr op, jbl_print_flags_t pf);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jbl_from_json([Out] out IntPtr jblp, IntPtr jsonstr);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jbl_xstr_json_printer(
            IntPtr data, int size, byte ch, int count, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_put(IntPtr db, IntPtr coll, IntPtr jbl, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_put_new(
            IntPtr db, IntPtr coll, IntPtr jbl, [Out] out long oid);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jbl_destroy([In, Out] ref IntPtr jblp);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jql_destroy([In, Out] ref IntPtr qptr);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_del(IntPtr db, IntPtr coll, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_get(
            IntPtr db, IntPtr coll, long id, [Out] out IntPtr jblp);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_rename_collection(
            IntPtr db, IntPtr coll, IntPtr new_coll);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_merge_or_put(
            IntPtr db, IntPtr coll, IntPtr patchjson, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_patch(
            IntPtr db, IntPtr coll, IntPtr patchjson, long id);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_ensure_index(
            IntPtr db, IntPtr coll, IntPtr path, ejdb_idx_mode_t mode);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_remove_index(
            IntPtr db, IntPtr coll, IntPtr path, ejdb_idx_mode_t mode);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_online_backup(
            IntPtr db, [Out] out ulong ts, IntPtr target_file);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_remove_collection(IntPtr db, IntPtr coll);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_create2([Out] out IntPtr qptr, IntPtr coll,
            IntPtr query, jql_create_mode_t mode);
        
        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jql_error(IntPtr q);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_get_skip(IntPtr q, [Out] out long skip);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void jql_reset(
            IntPtr q, bool reset_match_cache, bool reset_placeholders);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_get_limit(IntPtr q, [Out] out long limit);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_set_regexp2(IntPtr q, IntPtr placeholder,
            int index, IntPtr expr, freefn freefn, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_set_str2(IntPtr q, IntPtr placeholder,
            int index, IntPtr val, freefn freefn, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_set_json2(IntPtr q, IntPtr placeholder,
            int index, ref JBL_NODE val, freefn freefn, IntPtr op);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_set_i64(IntPtr q,
            IntPtr placeholder, int index, long val);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_set_f64(IntPtr q,
            IntPtr placeholder, int index, double val);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_set_bool(IntPtr q,
            IntPtr placeholder, int index, bool val);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jql_set_null(IntPtr q,
            IntPtr placeholder, int index);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong jbn_from_json(IntPtr json,
            [Out] out JBL_NODE node, [In] ref IntPtr pool);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr jql_collection(IntPtr q);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ejdb_exec([In, Out] ref EJDB_EXEC ux);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr jbl_size(IntPtr jbl);

        #endregion

        #region libiowow

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong iwlog_init();

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwlog_ecode_explained(ulong ecode);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint iwrc_strip_errno([In, Out] ref ulong rc);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwxstr_new();

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwxstr_new2(UIntPtr siz);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void iwxstr_destroy(IntPtr xstr);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr iwxstr_size(IntPtr xstr);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwxstr_ptr(IntPtr xstr);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr iwpool_create(UIntPtr siz);

        [DllImport(EjdbLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void iwpool_destroy(IntPtr pool);

        #endregion
    }
}
