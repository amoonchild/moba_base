using System;
using System.Runtime.InteropServices;


namespace LiaoZhai.FP
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VLine
    {
        public VInt2 point;
        public VInt2 direction;
    }
}