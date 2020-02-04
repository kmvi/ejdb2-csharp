using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2
{
    public sealed class EJDB2OptionsBuilder
    {
        private readonly EJDB2Options _options;

        public EJDB2OptionsBuilder(string path)
        {
            _options = new EJDB2Options(path);
        }

        public EJDB2OptionsBuilder WithWAL(Action<WALOptions> configurator = null)
        {
            _options.UseWAL = true;
            configurator?.Invoke(_options.IWKVOptions.WALOptions);
            return this;
        }

        public EJDB2OptionsBuilder WithoutWAL()
        {
            _options.UseWAL = false;
            return this;
        }

        public EJDB2OptionsBuilder SetSortBufferSize(uint value)
        {
            _options.SortBufferSize = value;
            return this;
        }

        public EJDB2OptionsBuilder SetDocumentBufferSize(uint value)
        {
            _options.DocumentBufferSize = value;
            return this;
        }

        // IWKVOptions

        public EJDB2OptionsBuilder Truncate()
        {
            _options.IWKVOptions.Truncate = true;
            return this;
        }

        public EJDB2OptionsBuilder Readonly()
        {
            _options.IWKVOptions.Readonly = true;
            return this;
        }

        public EJDB2OptionsBuilder SetRandomSeed(uint value)
        {
            _options.IWKVOptions.RandomSeed = value;
            return this;
        }

        public EJDB2OptionsBuilder FileLockFailFast()
        {
            _options.IWKVOptions.FileLockFailFast = true;
            return this;
        }

        // EJDB2HttpOptions

        public EJDB2OptionsBuilder WithHttp(Action<EJDB2HttpOptions> configurator = null)
        {
            _options.HttpOptions.Enabled = true;
            configurator?.Invoke(_options.HttpOptions);
            return this;
        }

        public EJDB2Options GetOptions() => _options;
    }
}
