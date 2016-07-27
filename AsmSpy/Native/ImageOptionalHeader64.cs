using System.Runtime.InteropServices;

namespace AsmSpy.Native
{
    using System;

    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageOptionalHeader64
    {
        [FieldOffset(0)]
        public UInt16 Magic;
        [FieldOffset(224)]
        public ImageDataDirectory DataDirectory;
    }
}