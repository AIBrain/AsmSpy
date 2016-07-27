using System.Runtime.InteropServices;

namespace AsmSpy.Native
{
    using System;

    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageDosHeader
    {
        [FieldOffset(60)]
        public readonly Int32 FileAddressOfNewExeHeader;
    }
}