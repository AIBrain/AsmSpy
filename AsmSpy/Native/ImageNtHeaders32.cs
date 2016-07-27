using System.Runtime.InteropServices;

namespace AsmSpy.Native
{
    using System;

    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageNtHeaders32
    {
        [FieldOffset(0)]
        public readonly UInt32 Signature;
        [FieldOffset(4)]
        public ImageFileHeader FileHeader;
        [FieldOffset(24)]
        public ImageOptionalHeader32 OptionalHeader;
    }
}