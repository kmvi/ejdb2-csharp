using System;
using System.Collections.Generic;
using System.Text;

namespace Ejdb2
{
    public class NextRecordEventArgs : EventArgs
    {
        public NextRecordEventArgs(long id, string json)
        {
            Id = id;
            Json = json;
            MoveTo = 1;
        }

        public long Id { get; }
        public string Json { get; }
        public uint MoveTo { get; set; }
    }
}
