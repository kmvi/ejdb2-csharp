using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2.Native.Win32
{
    internal sealed class Win32NativeHelper : INativeHelper
    {
        public ulong ejdb_close(ref IntPtr ejdbp) => Interop.ejdb_close(ref ejdbp);

        public ulong ejdb_del(IntPtr db, string coll, long id) => Interop.ejdb_del(db, coll, id);

        public ulong ejdb_ensure_index(IntPtr db, string coll, string path, ejdb_idx_mode_t mode) => Interop.ejdb_ensure_index(db, coll, path, mode);

        public ulong ejdb_get(IntPtr db, string coll, long id, out IntPtr jblp) => Interop.ejdb_get(db, coll, id, out jblp);

        public ulong ejdb_get_meta(IntPtr db, out IntPtr jblp) => Interop.ejdb_get_meta(db, out jblp);

        public ulong ejdb_init() => Interop.ejdb_init();

        public ulong ejdb_merge_or_put(IntPtr db, string coll, string patchjson, long id) => Interop.ejdb_merge_or_put(db, coll, patchjson, id);

        public ulong ejdb_online_backup(IntPtr db, out ulong ts, string target_file) => Interop.ejdb_online_backup(db, out ts, target_file);

        public ulong ejdb_open(ref EJDB_OPTS opts, out IntPtr ejdbp) => Interop.ejdb_open(ref opts, out ejdbp);

        public ulong ejdb_patch(IntPtr db, string coll, string patchjson, long id) => Interop.ejdb_patch(db, coll, patchjson, id);

        public ulong ejdb_put(IntPtr db, string coll, IntPtr jbl, long id) => Interop.ejdb_put(db, coll, jbl, id);

        public ulong ejdb_put_new(IntPtr db, string coll, IntPtr jbl, out long oid) => Interop.ejdb_put_new(db, coll, jbl, out oid);

        public ulong ejdb_remove_collection(IntPtr db, string coll) => Interop.ejdb_remove_collection(db, coll);

        public ulong ejdb_remove_index(IntPtr db, string coll, string path, ejdb_idx_mode_t mode) => Interop.ejdb_remove_index(db, coll, path, mode);

        public ulong ejdb_rename_collection(IntPtr db, string coll, string new_coll) => Interop.ejdb_rename_collection(db, coll, new_coll);

        public int ejdb_version_major() => Interop.ejdb_version_major();

        public int ejdb_version_minor() => Interop.ejdb_version_minor();

        public int ejdb_version_patch() => Interop.ejdb_version_patch();

        public ulong jbl_as_json(IntPtr jbl, jbl_json_printer pt, IntPtr op, jbl_print_flags_t pf) => Interop.jbl_as_json(jbl, pt, op, pf);

        public void jbl_destroy(ref IntPtr jblp) => Interop.jbl_destroy(ref jblp);

        public ulong jbl_from_json(out IntPtr jblp, string jsonstr) => Interop.jbl_from_json(out jblp, jsonstr);

        public ulong jql_create2(out IntPtr qptr, string coll, string query, jql_create_mode_t mode) => Interop.jql_create2(out qptr, coll, query, mode);

        public void jql_destroy(ref IntPtr qptr) => Interop.jql_destroy(ref qptr);

        public IntPtr jql_error(IntPtr q) => Interop.jql_error(q);

        public ulong jql_get_limit(IntPtr q, out long limit) => Interop.jql_get_limit(q, out limit);

        public ulong jql_get_skip(IntPtr q, out long skip) => Interop.jql_get_skip(q, out skip);

        public void jql_reset(IntPtr q, bool reset_match_cache, bool reset_placeholders) => Interop.jql_reset(q, reset_match_cache, reset_placeholders);
    }
}
