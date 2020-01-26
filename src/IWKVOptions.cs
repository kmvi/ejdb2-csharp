using System;
using System.Collections.Generic;
using System.Text;
using Ejdb2.Native;

namespace Ejdb2
{
    [Serializable]
    public sealed class IWKVOptions
    {
        public IWKVOptions(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be empty.", nameof(path));

            Path = path;
            WALOptions = new WALOptions();
        }

        public WALOptions WALOptions { get; }
        public string Path { get; set; }
        public uint RandomSeed { get; set; }
        public bool FileLockFailFast { get; set; }

        internal iwkv_openflags OpenFlags { get; set; }

        public bool Truncate
        {
            get => (OpenFlags & iwkv_openflags.IWKV_TRUNC) != 0;
            set => OpenFlags = value
                ? OpenFlags | iwkv_openflags.IWKV_TRUNC
                : OpenFlags & ~iwkv_openflags.IWKV_TRUNC;
        }

        public bool Readonly
        {
            get => (OpenFlags & iwkv_openflags.IWKV_RDONLY) != 0;
            set => OpenFlags = value
                ? OpenFlags | iwkv_openflags.IWKV_RDONLY
                : OpenFlags & ~iwkv_openflags.IWKV_RDONLY;
        }

        public override string ToString() =>
            new StringBuilder(GetType().Name)
                .Append('[')
                .Append("path='").Append(Path).Append('\'').Append(", ")
                .Append("oflags=").Append(OpenFlags).Append(", ")
                .Append("file_lock_fail_fast=").Append(FileLockFailFast).Append(", ")
                .Append("random_seed=").Append(RandomSeed).Append(", ")
                .Append("wal=").Append(WALOptions.ToString())
                .Append(']')
                .ToString();
    }
}
