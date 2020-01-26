using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2
{
    [Serializable]
    public class EJDB2Options
    {
        public EJDB2Options(string path)
        {
            IWKVOptions = new IWKVOptions(path);
            HttpOptions = new EJDB2HttpOptions();
            UseWAL = true;
        }

        public bool UseWAL { get; set; }
        public IWKVOptions IWKVOptions { get; }
        public EJDB2HttpOptions HttpOptions { get; }
        public uint SortBufferSize { get; set; }
        public uint DocumentBufferSize { get; internal set; }
    }
}
