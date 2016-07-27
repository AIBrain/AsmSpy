using System.Runtime.InteropServices;

namespace AsmSpy.Native
{
    using System;

    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageOptionalHeader32
    {
        [FieldOffset(0)]
        public UInt16 Magic;
        [FieldOffset(208)]
        public ImageDataDirectory DataDirectory;
    }
}