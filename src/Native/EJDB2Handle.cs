using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Ejdb2.Native
{
    internal sealed class EJDB2Handle : SafeHandle
    {
        private EJDB2Handle()
            : base(IntPtr.Zero, true)
        {

        }

        public EJDB2Handle(IntPtr value)
            : this()
        {
            SetHandle(value);
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
                return EJDB2Facade.Instance.Close(this) != 0;
            
            return false;
        }
    }
}
