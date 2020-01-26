using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ejdb2
{
    [Serializable]
    public sealed class EJDB2HttpOptions
    {
        private bool _enabled;

        public EJDB2HttpOptions()
        {

        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                // TODO: https://github.com/Softmotions/ejdb/issues/257
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    throw new NotSupportedException("HTTP is not supported on this platform.");
                
                _enabled = value;
            }
        }

        public int Port { get; set; }
        public string Bind { get; set; }
        public string AccessToken { get; set; }
        public bool ReadAnon { get; set; }
        public uint MaxBodySize { get; set; }

        public override string ToString() =>
            new StringBuilder(GetType().Name)
            .Append('[')
            .Append("enabled=").Append(Enabled).Append(", ")
            .Append("port=").Append(Port).Append(", ")
            .Append("bind='").Append(Bind).Append('\'').Append(", ")
            .Append("access_token='").Append(AccessToken).Append('\'').Append(", ")
            .Append("read_anon=").Append(ReadAnon).Append(", ")
            .Append("max_body_size=").Append(MaxBodySize)
            .Append(']')
            .ToString();
    }
}
