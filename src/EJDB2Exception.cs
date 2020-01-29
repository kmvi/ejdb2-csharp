using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ejdb2
{
    [Serializable]
    public class EJDB2Exception : Exception
    {
        public EJDB2Exception(ulong code, uint errno)
        {
            Code = code;
            Errno = errno;
        }

        public EJDB2Exception(ulong code, uint errno, string message)
            : base(message)
        {
            Code = code;
            Errno = errno;
        }

        public EJDB2Exception(ulong code, uint errno, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
            Errno = errno;
        }

        protected EJDB2Exception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public ulong Code { get; }
        public uint Errno { get; }
    }
}
