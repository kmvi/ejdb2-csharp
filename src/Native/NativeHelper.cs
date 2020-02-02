using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Ejdb2.Native.Linux;
using Ejdb2.Native.Win32;

namespace Ejdb2.Native
{
    internal abstract class NativeHelper
    {
        public static INativeHelper Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new Win32NativeHelper();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxNativeHelper();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException();
            }

            throw new PlatformNotSupportedException(
                $"Platform {RuntimeInformation.OSDescription} is not supported.");
        }
    }
}
