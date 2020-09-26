using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2.Native.Win32
{
    internal sealed class Win32NativeHelper : INativeHelper
    {
        public ulong ejdb_close(ref IntPtr ejdbp) => Interop.ejdb_close(ref ejdbp);

        public ulong ejdb_del(IntPtr db, IntPtr coll, long id) => Interop.ejdb_del(db, coll, id);

        public ulong ejdb_ensure_index(IntPtr db, IntPtr coll, IntPtr path, ejdb_idx_mode_t mode) => Interop.ejdb_ensure_index(db, coll, path, mode);

        public ulong ejdb_exec(ref EJDB_EXEC ux) => Interop.ejdb_exec(ref ux);

        public ulong ejdb_get(IntPtr db, IntPtr coll, long id, out IntPtr jblp) => Interop.ejdb_get(db, coll, id, out jblp);

        public ulong ejdb_get_meta(IntPtr db, out IntPtr jblp) => Interop.ejdb_get_meta(db, out jblp);

        public ulong ejdb_init() => Interop.ejdb_init();

        public ulong ejdb_merge_or_put(IntPtr db, IntPtr coll, IntPtr patchjson, long id) => Interop.ejdb_merge_or_put(db, coll, patchjson, id);

        public ulong ejdb_online_backup(IntPtr db, out ulong ts, IntPtr target_file) => Interop.ejdb_online_backup(db, out ts, target_file);

        public ulong ejdb_open(ref EJDB_OPTS opts, out IntPtr ejdbp) => Interop.ejdb_open(ref opts, out ejdbp);

        public ulong ejdb_patch(IntPtr db, IntPtr coll, IntPtr patchjson, long id) => Interop.ejdb_patch(db, coll, patchjson, id);

        public ulong ejdb_put(IntPtr db, IntPtr coll, IntPtr jbl, long id) => Interop.ejdb_put(db, coll, jbl, id);

        public ulong ejdb_put_new(IntPtr db, IntPtr coll, IntPtr jbl, out long oid) => Interop.ejdb_put_new(db, coll, jbl, out oid);

        public ulong ejdb_remove_collection(IntPtr db, IntPtr coll) => Interop.ejdb_remove_collection(db, coll);

        public ulong ejdb_remove_index(IntPtr db, IntPtr coll, IntPtr path, ejdb_idx_mode_t mode) => Interop.ejdb_remove_index(db, coll, path, mode);

        public ulong ejdb_rename_collection(IntPtr db, IntPtr coll, IntPtr new_coll) => Interop.ejdb_rename_collection(db, coll, new_coll);

        public int ejdb_version_major() => Interop.ejdb_version_major();

        public int ejdb_version_minor() => Interop.ejdb_version_minor();

        public int ejdb_version_patch() => Interop.ejdb_version_patch();

        public IntPtr iwlog_ecode_explained(ulong ecode) => Interop.iwlog_ecode_explained(ecode);

        public ulong iwlog_init() => Interop.iwlog_init();

        public IntPtr iwpool_create(UIntPtr siz) => Interop.iwpool_create(siz);

        public void iwpool_destroy(IntPtr pool) => Interop.iwpool_destroy(pool);

        public uint iwrc_strip_errno(ref ulong rc) => Interop.iwrc_strip_errno(ref rc);

        public void iwxstr_destroy(IntPtr xstr) => Interop.iwxstr_destroy(xstr);

        public IntPtr iwxstr_new() => Interop.iwxstr_new();

        public IntPtr iwxstr_new2(UIntPtr siz) => Interop.iwxstr_new2(siz);

        public IntPtr iwxstr_ptr(IntPtr xstr) => Interop.iwxstr_ptr(xstr);

        public UIntPtr iwxstr_size(IntPtr xstr) => Interop.iwxstr_size(xstr);

        public ulong jbl_as_json(IntPtr jbl, jbl_json_printer pt, IntPtr op, jbl_print_flags_t pf) => Interop.jbl_as_json(jbl, pt, op, pf);

        public void jbl_destroy(ref IntPtr jblp) => Interop.jbl_destroy(ref jblp);

        public ulong jbl_from_json(out IntPtr jblp, IntPtr jsonstr) => Interop.jbl_from_json(out jblp, jsonstr);

        public ulong jbn_as_json(IntPtr node, jbl_json_printer pt, IntPtr op, jbl_print_flags_t pf) => Interop.jbn_as_json(node, pt, op, pf);

        public ulong jbn_from_json(IntPtr json, out JBL_NODE node, ref IntPtr pool) => Interop.jbn_from_json(json, out node, ref pool);

        public UIntPtr jbl_size(IntPtr jbl) => Interop.jbl_size(jbl);

        public ulong jbl_xstr_json_printer(IntPtr data, int size, byte ch, int count, IntPtr op) => Interop.jbl_xstr_json_printer(data, size, ch, count, op);

        public IntPtr jql_collection(IntPtr q) => Interop.jql_collection(q);

        public ulong jql_create2(out IntPtr qptr, IntPtr coll, IntPtr query, jql_create_mode_t mode) => Interop.jql_create2(out qptr, coll, query, mode);

        public void jql_destroy(ref IntPtr qptr) => Interop.jql_destroy(ref qptr);

        public IntPtr jql_error(IntPtr q) => Interop.jql_error(q);

        public ulong jql_get_limit(IntPtr q, out long limit) => Interop.jql_get_limit(q, out limit);

        public ulong jql_get_skip(IntPtr q, out long skip) => Interop.jql_get_skip(q, out skip);

        public void jql_reset(IntPtr q, bool reset_match_cache, bool reset_placeholders) => Interop.jql_reset(q, reset_match_cache, reset_placeholders);

        public ulong jql_set_bool(IntPtr q, IntPtr placeholder, int index, bool val) => Interop.jql_set_bool(q, placeholder, index, val);

        public ulong jql_set_f64(IntPtr q, IntPtr placeholder, int index, double val) => Interop.jql_set_f64(q, placeholder, index, val);

        public ulong jql_set_i64(IntPtr q, IntPtr placeholder, int index, long val) => Interop.jql_set_i64(q, placeholder, index, val);

        public ulong jql_set_json2(IntPtr q, IntPtr placeholder, int index, ref JBL_NODE val, freefn freefn, IntPtr op) => Interop.jql_set_json2(q, placeholder, index, ref val, freefn, op);

        public ulong jql_set_null(IntPtr q, IntPtr placeholder, int index) => Interop.jql_set_null(q, placeholder, index);

        public ulong jql_set_regexp2(IntPtr q, IntPtr placeholder, int index, IntPtr expr, freefn freefn, IntPtr op) => Interop.jql_set_regexp2(q, placeholder, index, expr, freefn, op);

        public ulong jql_set_str2(IntPtr q, IntPtr placeholder, int index, IntPtr val, freefn freefn, IntPtr op) => Interop.jql_set_str2(q, placeholder, index, val, freefn, op);
    }
}
