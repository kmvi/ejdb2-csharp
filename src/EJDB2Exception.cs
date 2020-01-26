using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ejdb2
{
    [Serializable]
    public class EJDB2Exception : Exception
    {
        public EJDB2Exception(ulong code)
        {
            Code = code;
        }

        public EJDB2Exception(ulong code, string message)
            : base(message)
        {
            Code = code;
        }

        public EJDB2Exception(ulong code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        protected EJDB2Exception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public ulong Code { get; }
    }
}
