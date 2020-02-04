using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Ejdb2.Native
{
    internal sealed class Utf8String : IDisposable
    {
        private readonly bool _isNull;

        private GCHandle _handle;
        private bool _disposed;

        public Utf8String(string str)
        {
            if (str == null)
            {
                _isNull = true;
            }
            else
            {
                _isNull = false;
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                _handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (!_isNull)
                    _handle.Free();

                _disposed = true;
            }
        }

        public override string ToString()
        {
            EnsureNotDisposed();
            return _isNull ? null : Encoding.UTF8.GetString((byte[])_handle.Target);
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        public static implicit operator IntPtr(Utf8String str)
        {
            str.EnsureNotDisposed();
            return str._isNull ? IntPtr.Zero : str._handle.AddrOfPinnedObject();
        }

        public static IntPtr CreateUnmanaged(string str)
        {
            if (str == null)
                return IntPtr.Zero;

            byte[] buffer = Encoding.UTF8.GetBytes(str);
            IntPtr result = Marshal.AllocHGlobal(buffer.Length + 1);
            Marshal.Copy(buffer, 0, result, buffer.Length);
            Marshal.WriteByte(result, buffer.Length, 0);

            return result;
        }

        public static unsafe string FromIntPtr(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            return new String((sbyte*)ptr, 0, StrLen(ptr), Encoding.UTF8);
        }

        public static unsafe int StrLen(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return 0;

            int len = 0;
            var p = (byte*)ptr;
            while (*p++ != 0)
                ++len;

            return len;
        }
    }

    internal sealed class Utf8StringPool : IDisposable
    {
        private bool _disposed;

        private readonly List<Utf8String> _strings = new List<Utf8String>();

        public Utf8StringPool()
        {
        }

        public Utf8String GetString(string str)
        {
            EnsureNotDisposed();

            var result = new Utf8String(str);
            _strings.Add(result);

            return result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (Utf8String str in _strings)
                    str.Dispose();
                _strings.Clear();
                _disposed = true;
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
