using System;
using System.Runtime.InteropServices;

namespace dotPatchIat
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageNTHeaders
    {
        public ImageNTSignature Signature;
        public ImageFileHeader FileHeader;
        public ImageOptionalHeader OptionalHeader;

        public void VerifySignature()
        {
            if (Signature != ImageNTSignature.PE00)
            {
                throw new BadImageFormatException();
            }
        }
    }
}