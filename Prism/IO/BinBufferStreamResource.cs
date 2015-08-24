using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Prism.IO
{
    public class BinBufferStreamResource : BinBufferResource
    {
        Stream stream;

        public override int Position
        {
            get
            {
                return (int)stream.Position;
            }
            set
            {
                stream.Position = value;
            }
        }
        public override bool IsEmpty
        {
            get
            {
                return stream.Length == 0L;
            }
        }
        public override int Size
        {
            get
            {
                return (int)stream.Length;
            }
        }
        public override int BufferSize
        {
            get
            {
                return (int)stream.Length;
            }
        }

        public BinBufferStreamResource(int initialCapacity)
        {
            stream = new MemoryStream(initialCapacity);
        }
        public BinBufferStreamResource(byte[] buffer)
        {
            stream = new MemoryStream(buffer);
        }
        public BinBufferStreamResource(Stream s, bool copy = false)
        {
            if (copy)
            {
                stream = new MemoryStream((int)s.Length);
                s.CopyTo(stream, (int)s.Length);
            }
            else
                stream = s;
        }

        public override void Clear(bool wipeData = false)
        {
            if (stream.CanSeek)
                stream.Position = 0L;

            if (wipeData)
                stream.SetLength(0L);
        }

        public override void Write(Union v, int size)
        {
            for (int i = 0; i < size; i++)
                stream.WriteByte(v[i]);
        }

        public override void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }
        public override void Write(byte[] data, int startIndex, int count)
        {
            stream.Write(data, startIndex, count);
        }

        public override Union ReadUnion(int size)
        {
            var u = new Union();

            for (int i = 0; i < size; i++)
            {
                var v = stream.ReadByte();

                if (v == -1)
                    throw new EndOfStreamException();

                u[i] = (byte)v;
            }

            return u;
        }

        public override byte ReadByte()
        {
            var r = stream.ReadByte();

            if (r == -1)
                throw new EndOfStreamException();

            return (byte)r;
        }
        public override int Read(byte[] data, int startIndex, int count)
        {
            return stream.Read(data, startIndex, count);
        }

        public override Stream AsStream()
        {
            return stream;
        }
        public override byte[] AsByteArray()
        {
            if (stream is MemoryStream)
                return ((MemoryStream)stream).ToArray();

            //if (stream is UnmanagedMemoryStream)
            //{
            //    byte[] uret = new byte[stream.Length];
            //    Marshal.Copy(new IntPtr(((UnmanagedMemoryStream)stream).PositionPointer), uret, 0, (int)stream.Length);
            //    return uret;
            //}

            byte[] ret = new byte[stream.Length];

            long p = stream.Position;
            stream.Seek(0L, SeekOrigin.Begin);

            stream.Read(ret, 0, ret.Length);

            stream.Seek(p, SeekOrigin.Begin);

            return ret;
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            stream.Dispose();
            stream = null;
        }

        public static BinBufferStreamResource FromFile(string path, bool canWrite = true)
        {
            return new BinBufferStreamResource(canWrite ? File.OpenWrite(path) : File.OpenRead(path));
        }
    }
}
