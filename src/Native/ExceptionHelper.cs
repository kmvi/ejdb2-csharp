using System;
using System.Runtime.InteropServices;

namespace Ejdb2.Native
{
    internal class ExceptionHelper
    {
        private readonly INativeHelper _helper;

        public ExceptionHelper(INativeHelper helper)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public EJDB2Exception CreateException(ulong rc, string message = null, Exception innerException = null)
        {
            if (message == null)
            {
                IntPtr pmsg = _helper.iwlog_ecode_explained(rc);
                message = pmsg != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi(pmsg)
                    : "Unknown error.";
            }

            uint errno = _helper.iwrc_strip_errno(ref rc);

            return new EJDB2Exception(rc, errno, message, innerException);
        }
    }
}
