using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2.Native
{
    internal interface INativeHelper
    {
        ulong ejdb_init();
        ulong ejdb_open(ref EJDB_OPTS opts, out IntPtr ejdbp);
        ulong ejdb_close(ref IntPtr ejdbp);
        ulong ejdb_get_meta(IntPtr handle, out IntPtr jblp);
        ulong jbl_from_json(out IntPtr jblp, string jsonstr);
        ulong jbl_as_json(IntPtr jbl, jbl_json_printer pt, IntPtr op, jbl_print_flags_t pf);
        ulong jbl_node_as_json(IntPtr node, jbl_json_printer pt, IntPtr op, jbl_print_flags_t pf);
        ulong jbl_xstr_json_printer(string data, int size, char ch, int count, IntPtr op);
        ulong ejdb_put(IntPtr db, string coll, IntPtr jbl, long id);
        ulong ejdb_put_new(IntPtr db, string coll, IntPtr jbl, out long oid);
        void jbl_destroy(ref IntPtr jblp);
        void jql_destroy(ref IntPtr qptr);
        ulong ejdb_del(IntPtr db, string coll, long id);
        ulong ejdb_get(IntPtr db, string coll, long id, out IntPtr jblp);
        ulong ejdb_rename_collection(IntPtr db, string coll, string new_coll);
        ulong ejdb_merge_or_put(IntPtr db, string coll, string patchjson, long id);
        ulong ejdb_patch(IntPtr db, string coll, string patchjson, long id);
        ulong ejdb_ensure_index(IntPtr db, string coll, string path, ejdb_idx_mode_t mode);
        ulong ejdb_remove_index(IntPtr db, string coll, string path, ejdb_idx_mode_t mode);
        ulong ejdb_online_backup(IntPtr db, out ulong ts, string target_file);
        ulong ejdb_remove_collection(IntPtr db, string coll);
        ulong jql_create2(out IntPtr qptr, string coll, string query, jql_create_mode_t mode);
        IntPtr jql_error(IntPtr q);
        ulong jql_get_skip(IntPtr q, out long skip);
        ulong jql_get_limit(IntPtr q, out long limit);
        void jql_reset(IntPtr q, bool reset_match_cache, bool reset_placeholders);
        IntPtr jql_collection(IntPtr q);
        ulong jql_set_regexp2(IntPtr q, string placeholder, int index, IntPtr expr, freefn freefn, IntPtr op);
        ulong jql_set_str2(IntPtr q, string placeholder, int index, IntPtr val, freefn freefn, IntPtr op);
        ulong jql_set_json2(IntPtr q, string placeholder, int index, ref JBL_NODE val, freefn freefn, IntPtr op);
        ulong jql_set_i64(IntPtr q, string placeholder, int index, long val);
        ulong jql_set_f64(IntPtr q, string placeholder, int index, double val);
        ulong jql_set_bool(IntPtr q, string placeholder, int index, bool val);
        ulong jql_set_null(IntPtr q, string placeholder, int index);
        ulong jbl_node_from_json(string json, out JBL_NODE node, ref IntPtr pool);
        ulong ejdb_exec(ref EJDB_EXEC ux);
        UIntPtr jbl_size(IntPtr jbl);
        int ejdb_version_major();
        int ejdb_version_minor();
        int ejdb_version_patch();

        ulong iwlog_init();
        ulong iwlog_register_ecodefn(IWLOG_ECODE_FN fp);
        IntPtr iwlog_ecode_explained(ulong ecode);
        uint iwrc_strip_errno(ref ulong rc);
        IntPtr iwxstr_new();
        IntPtr iwxstr_new2(UIntPtr siz);
        void iwxstr_destroy(IntPtr xstr);
        UIntPtr iwxstr_size(IntPtr xstr);
        IntPtr iwxstr_ptr(IntPtr xstr);
        IntPtr iwpool_create(UIntPtr siz);
        void iwpool_destroy(IntPtr pool);
    }
}
