using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;

namespace Prism.IO
{
    public class BinBuffer : BinBufferResource
    {
        BinBufferResource res;

        public override int Position
        {
            get
            {
                return res.Position;
            }
            set
            {
                res.Position = value;
            }
        }
        public override bool IsEmpty
        {
            get
            {
                return res.IsEmpty;
            }
        }
        public override int Size
        {
            get
            {
                return res.Size;
            }
        }
        public override int BufferSize
        {
            get
            {
                return res.BufferSize;
            }
        }

        public BinBuffer(BinBufferResource resource)
        {
            res = resource;
        }
        public BinBuffer()
            : this(new BinBufferByteResource())
        {

        }
        public BinBuffer(int initialCapacity)
            : this(new BinBufferByteResource(initialCapacity))
        {

        }
        public BinBuffer(byte[] data, bool copy = true, bool startAtEnd = false)
            : this(new BinBufferByteResource(data, copy, startAtEnd))
        {

        }
        public BinBuffer(Stream s, bool copy = false, bool dispose = true)
            : this(new BinBufferStreamResource(s, copy, dispose))
        {

        }

        public override void Clear(bool wipeData = false)
        {
            res.Clear(wipeData);
        }

        public override void Write(Union v, int size)
        {
            res.Write(v, size);
        }

        public override void WriteByte(byte value)
        {
            res.WriteByte(value);
        }
        public override void Write(byte[] data, int startIndex, int count)
        {
            res.Write(data, startIndex, count);
        }

        public override Union ReadUnion(int size)
        {
            return res.ReadUnion(size);
        }

        public override byte ReadByte()
        {
            return res.ReadByte();
        }
        public override int Read(byte[] data, int startIndex, int count)
        {
            return res.Read(data, startIndex, count);
        }

        public override Stream AsStream()
        {
            return res.AsStream();
        }
        public override byte[] AsByteArray()
        {
            return res.AsByteArray();
        }

        protected override void Dispose(bool disposing)
        {
            res.Dispose();
            res = null;
        }

        #region Write methods
        public void Write(bool v)
        {
            Write((byte)(v ? 1 : 0));
        }

        public void Write(sbyte v)
        {
            Write(unchecked((byte)v));
        }
        // using the Union struct is much faster than splitting up in bytes (only have to check position/size once) or using the bitconverter (it allocates a new array every call)
        // I know, this is ugly as fuck, but performance might be needed here (e.g. world loading time...)
        public void Write(ushort v)
        {
            Write(new Union() { Short1 = v }, sizeof(ushort));
        }
        public void Write(short v)
        {
            Write(new Union() { Short1 = unchecked((ushort)v) }, sizeof(short));
        }
        public void Write(uint v)
        {
            Write(new Union() { Int1 = v }, sizeof(uint));
        }
        public void Write(int v)
        {
            Write(new Union() { Int1 = unchecked((uint)v) }, sizeof(uint));
        }
        public void Write(ulong v)
        {
            Write(new Union() { Long1 = v }, sizeof(ulong));
        }
        public void Write(long v)
        {
            Write(new Union() { Long1 = unchecked((ulong)v) }, sizeof(ulong));
        }

        public void Write(float v)
        {
            Write(new Union() { Float1 = v }, sizeof(float));
        }
        public void Write(double v)
        {
            Write(new Union() { Double1 = v }, sizeof(double));
        }
        public void Write(decimal v)
        {
            Write(new Union() { Decimal = v }, sizeof(decimal));
        }

        public void Write(TimeSpan span)
        {
            Write(new Union() { Long1 = unchecked((ulong)span.Ticks) }, sizeof(ulong));
        }
        public void Write(DateTime date)
        {
            Write(new Union() { Long1 = unchecked((ulong)date.Ticks), Byte9 = (byte)date.Kind }, sizeof(ulong) + sizeof(byte));
        }
        public void Write(Complex c)
        {
            Write(new Union() { Double1 = c.Real, Double2 = c.Imaginary }, sizeof(double) * 2);
        }
        public void Write(Point p)
        {
            Write(unchecked(new Union() { Int1 = (uint)p.X, Int2 = (uint)p.Y }), sizeof(int) * 2);
        }
        public void Write(Rectangle r)
        {
            Write(unchecked(new Union() { Int1 = (uint)r.X, Int2 = (uint)r.Y, Int3 = (uint)r.Width, Int4 = (uint)r.Height }), sizeof(int) * 4);
        }
        public void Write(Vector2 v)
        {
            Write(new Union() { Float1 = v.X, Float2 = v.Y }, sizeof(float) * 2);
        }
        public void Write(Vector3 v)
        {
            Write(new Union() { Float1 = v.X, Float2 = v.Y, Float3 = v.Z }, sizeof(float) * 3);
        }
        public void Write(Vector4 v)
        {
            Write(new Union() { Float1 = v.X, Float2 = v.Y, Float3 = v.Z, Float4 = v.W }, sizeof(float) * 4);
        }
        public void Write(Color c)
        {
            Write(new Union() { Int1 = c.PackedValue }, sizeof(int));
        }
        public void WriteRgb(Color c)
        {
            Write(new Union() { Byte1 = c.R, Byte2 = c.G, Byte3 = c.B }, sizeof(byte) * 3);
        }

        public void Write(byte[] data)
        {
            Write(data, 0, data.Length);
        }

        public void WriteVlq(uint v)
        {
            do
            {
                unchecked
                {
                    byte b = (byte)(v & SByte.MaxValue);

                    v >>= 7;

                    if (v != 0)
                        b |= 128;

                    WriteByte(b);
                }
            } while (v != 0);
        }
        public void WriteVlq(BigInteger v)
        {
            do
            {
                unchecked
                {
                    byte b = (byte)(v & SByte.MaxValue);

                    v >>= 7;

                    if (v != 0)
                        b |= 128;

                    WriteByte(b);
                }
            } while (v != 0);
        }

        public void Write(string v)
        {
            var d = Encoding.UTF8.GetBytes(v);

            Write/*Vlq*/(/*(uint)*/d.Length);
            Write(d);
        }

        public void Write(BigInteger i)
        {
            var d = i.ToByteArray();
            Write(d.Length);
            Write(d);
        }

        public void Write(BinBuffer bb)
        {
            Write(bb, bb.BytesLeft);
        }
        public void Write(BinBuffer bb, int count)
        {
            for (int i = 0; i < count; i++)
                Write(bb.ReadByte());
        }
        #endregion

        #region Read methods
        public bool ReadBoolean()
        {
            return ReadByte() == 0 ? false : true;
        }

        public sbyte ReadSByte()
        {
            return unchecked((sbyte)ReadByte());
        }
        public ushort ReadUInt16()
        {
            return ReadUnion(sizeof(ushort)).Short1;
        }
        public short ReadInt16()
        {
            return unchecked((short)ReadUnion(sizeof(ushort)).Short1);
        }
        public uint ReadUInt32()
        {
            return ReadUnion(sizeof(uint)).Int1;
        }
        public int ReadInt32()
        {
            return unchecked((int)ReadUnion(sizeof(uint)).Int1);
        }
        public ulong ReadUInt64()
        {
            return ReadUnion(sizeof(ulong)).Long1;
        }
        public long ReadInt64()
        {
            return unchecked((long)ReadUnion(sizeof(ulong)).Long1);
        }

        public float ReadSingle()
        {
            return ReadUnion(sizeof(float)).Float1;
        }
        public double ReadDouble()
        {
            return ReadUnion(sizeof(double)).Double1;
        }
        public decimal ReadDecimal()
        {
            return ReadUnion(sizeof(decimal)).Decimal;
        }

        public TimeSpan ReadTimeSpan()
        {
            return TimeSpan.FromTicks(unchecked((long)ReadUnion(sizeof(ulong)).Long1));
        }
        public DateTime ReadDateTime()
        {
            var u = ReadUnion(sizeof(ulong) + sizeof(byte));
            return new DateTime(unchecked((long)ReadUnion(sizeof(ulong)).Long1), (DateTimeKind)u.Byte1);
        }
        public Complex ReadComplex()
        {
            var u = ReadUnion(sizeof(double) * 2);
            return new Complex(u.Double1, u.Double2);
        }
        public Point ReadPoint()
        {
            var u = ReadUnion(sizeof(int) * 2);
            return unchecked(new Point((int)u.Int1, (int)u.Int2));
        }
        public Rectangle ReadRectangle()
        {
            var u = ReadUnion(sizeof(int) * 4);
            return unchecked(new Rectangle((int)u.Int1, (int)u.Int2, (int)u.Int3, (int)u.Int4));
        }
        public Vector2 ReadVector2()
        {
            var u = ReadUnion(sizeof(float) * 2);
            return new Vector2(u.Float1, u.Float2);
        }
        public Vector3 ReadVector3()
        {
            var u = ReadUnion(sizeof(float) * 3);
            return new Vector3(u.Float1, u.Float2, u.Float3);
        }
        public Vector4 ReadVector4()
        {
            var u = ReadUnion(sizeof(float) * 4);
            return new Vector4(u.Float1, u.Float2, u.Float3, u.Float4);
        }
        public Color ReadColour()
        {
            var u = ReadUnion(sizeof(byte) * 4);
            return new Color(u.Byte1, u.Byte2, u.Byte3, u.Byte4);
        }
        public Color ReadColourRgb()
        {
            var u = ReadUnion(sizeof(byte) * 3);
            return new Color(u.Byte1, u.Byte2, u.Byte3);
        }

        public byte[] ReadBytes(int amount)
        {
            byte[] data = new byte[amount];
            Read(data, 0, amount);
            return data;
        }
        public byte[] ReadBytesLeft()
        {
            return ReadBytes(BytesLeft);
        }

        public uint ReadVlqUInt32()
        {
            uint r = 0;

            for (int s = 0; ; s += 7)
            {
                var b = ReadByte();

                r |= (uint)((b & SByte.MaxValue) << s);

                if ((b & 128) == 0)
                    break;
            }

            return r;
        }
        public BigInteger ReadVlqBigInteger()
        {
            BigInteger r = 0;

            for (int s = 0; ; s += 7)
            {
                var b = ReadByte();

                r |= (b & SByte.MaxValue) << s;

                if ((b & 128) == 0)
                    break;
            }

            return r;
        }

        public string ReadString()
        {
            return Encoding.UTF8.GetString(ReadBytes(/*(int)*/ReadInt32/*VlqUInt32*/()));
        }

        public BigInteger ReadBigInteger()
        {
            return new BigInteger(ReadBytes(unchecked((int)ReadUnion(sizeof(uint)).Int1)));
        }
        #endregion
    }
}
