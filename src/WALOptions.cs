using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2
{
    [Serializable]
    public sealed class WALOptions
    {
        public WALOptions()
        {

        }

        public bool CheckCRCOnCheckpoint { get; set; }
        public uint SavePointTimeoutSec { get; set; }
        public uint CheckPointTimeoutSec { get; set; }
        public uint BufferSize { get; set; }
        public uint CheckpointBufferSize { get; set; }

        public override string ToString() =>
            new StringBuilder(GetType().Name)
                .Append('[')
                .Append("check_crc_on_checkpoint=").Append(CheckCRCOnCheckpoint).Append(", ")
                .Append("savepoint_timeout_sec=").Append(SavePointTimeoutSec).Append(", ")
                .Append("checkpoint_timeout_sec=").Append(CheckPointTimeoutSec).Append(", ")
                .Append("buffer_sz=").Append(BufferSize).Append(", ")
                .Append("checkpoint_buffer_sz=").Append(CheckpointBufferSize)
                .Append(']')
                .ToString();
    }
}
