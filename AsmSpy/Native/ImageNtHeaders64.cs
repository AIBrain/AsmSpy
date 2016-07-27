namespace AsmSpy.Native
{
    using System;

    internal struct ImageNtHeaders64
    {
        public UInt32 Signature;
        public ImageFileHeader FileHeader;
        public ImageOptionalHeader64 OptionalHeader;
    }
}