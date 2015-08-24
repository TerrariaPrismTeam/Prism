using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Prism.IO
{
    // ugly, but better than allocating new byte arrays (System.BitConverter)

    [StructLayout(LayoutKind.Explicit, Size = sizeof(decimal))]
    public struct Union
    {
        [FieldOffset( 0)]
        public byte Byte1 ;
        [FieldOffset( 1)]
        public byte Byte2 ;
        [FieldOffset( 2)]
        public byte Byte3 ;
        [FieldOffset( 3)]
        public byte Byte4 ;
        [FieldOffset( 4)]
        public byte Byte5 ;
        [FieldOffset( 5)]
        public byte Byte6 ;
        [FieldOffset( 6)]
        public byte Byte7 ;
        [FieldOffset( 7)]
        public byte Byte8 ;
        [FieldOffset( 8)]
        public byte Byte9 ;
        [FieldOffset( 9)]
        public byte Byte10;
        [FieldOffset(10)]
        public byte Byte11;
        [FieldOffset(11)]
        public byte Byte12;
        [FieldOffset(12)]
        public byte Byte13;
        [FieldOffset(13)]
        public byte Byte14;
        [FieldOffset(14)]
        public byte Byte15;
        [FieldOffset(15)]
        public byte Byte16;

        [FieldOffset( 0)]
        public ushort Short1;
        [FieldOffset( 2)]
        public ushort Short2;
        [FieldOffset( 4)]
        public ushort Short3;
        [FieldOffset( 6)]
        public ushort Short4;
        [FieldOffset( 8)]
        public ushort Short5;
        [FieldOffset(10)]
        public ushort Short6;
        [FieldOffset(12)]
        public ushort Short7;
        [FieldOffset(14)]
        public ushort Short8;

        [FieldOffset(0)]
        public uint Int1;
        [FieldOffset(4)]
        public uint Int2;
        [FieldOffset(8)]
        public uint Int3;
        [FieldOffset(12)]
        public uint Int4;

        [FieldOffset(0)]
        public ulong Long1;
        [FieldOffset(8)]
        public ulong Long2;

        [FieldOffset(0)]
        public float Float1;
        [FieldOffset(4)]
        public float Float2;
        [FieldOffset(8)]
        public float Float3;
        [FieldOffset(12)]
        public float Float4;

        [FieldOffset(0)]
        public double Double1;
        [FieldOffset(8)]
        public double Double2;

        [FieldOffset(0)]
        public decimal Decimal;

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Byte1;
                    case 1:
                        return Byte2;
                    case 2:
                        return Byte3;
                    case 3:
                        return Byte4;
                    case 4:
                        return Byte5;
                    case 5:
                        return Byte6;
                    case 6:
                        return Byte7;
                    case 7:
                        return Byte8;
                    case 8:
                        return Byte9;
                    case 9:
                        return Byte10;
                    case 10:
                        return Byte11;
                    case 11:
                        return Byte12;
                    case 12:
                        return Byte13;
                    case 13:
                        return Byte14;
                    case 14:
                        return Byte15;
                    case 15:
                        return Byte16;
                }

                throw new ArgumentOutOfRangeException("index");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Byte1 = value;
                        break;
                    case 1:
                        Byte2 = value;
                        break;
                    case 2:
                        Byte3 = value;
                        break;
                    case 3:
                        Byte4 = value;
                        break;
                    case 4:
                        Byte5 = value;
                        break;
                    case 5:
                        Byte6 = value;
                        break;
                    case 6:
                        Byte7 = value;
                        break;
                    case 7:
                        Byte8 = value;
                        break;
                    case 8:
                        Byte9 = value;
                        break;
                    case 9:
                        Byte10 = value;
                        break;
                    case 10:
                        Byte11 = value;
                        break;
                    case 11:
                        Byte12 = value;
                        break;
                    case 12:
                        Byte13 = value;
                        break;
                    case 13:
                        Byte14 = value;
                        break;
                    case 14:
                        Byte15 = value;
                        break;
                    case 15:
                        Byte16 = value;
                        break;
                }

                throw new ArgumentOutOfRangeException("index");
            }
        }
    }
}
