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
        ulong ejdb_put(IntPtr db, string coll, IntPtr jbl, long id);
        ulong ejdb_put_new(IntPtr db, string coll, IntPtr jbl, out long oid);
        void jbl_destroy(ref IntPtr jblp);
        ulong ejdb_del(IntPtr db, string coll, long id);
        ulong ejdb_get(IntPtr db, string coll, long id, out IntPtr jblp);
        ulong ejdb_rename_collection(IntPtr db, string coll, string new_coll);
        ulong ejdb_merge_or_put(IntPtr db, string coll, string patchjson, long id);
        ulong ejdb_patch(IntPtr db, string coll, string patchjson, long id);
        ulong ejdb_ensure_index(IntPtr db, string coll, string path, ejdb_idx_mode_t mode);
        ulong ejdb_remove_index(IntPtr db, string coll, string path, ejdb_idx_mode_t mode);
        ulong ejdb_online_backup(IntPtr db, out ulong ts, string target_file);
        ulong ejdb_remove_collection(IntPtr db, string coll);
        int ejdb_version_major();
        int ejdb_version_minor();
        int ejdb_version_patch();
    }
}
