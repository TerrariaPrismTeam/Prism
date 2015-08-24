using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Prism.IO
{
    public class BinBufferByteResource : BinBufferResource
    {
        byte[] buffer;
        int pos = 0, size = 0;

        public override int Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;

                if (pos > buffer.Length)
                    ResizeBuffer(pos);
            }
        }
        public override bool IsEmpty
        {
            get
            {
                return size == 0;
            }
        }
        public override int Size
        {
            get
            {
                return size;
            }
        }
        public override int BufferSize
        {
            get
            {
                return buffer.Length;
            }
        }

        public BinBufferByteResource(byte[] data, bool copy = true, bool startAtEnd = false)
        {
            if (copy)
            {
                buffer = new byte[data.Length];

                Array.Copy(data, buffer, data.Length);
            }
            else
                buffer = data;

            size = data.Length;
            if (startAtEnd)
                pos = data.Length;
        }
        public BinBufferByteResource(int initialCapacity)
        {
            buffer = new byte[initialCapacity];
        }
        public BinBufferByteResource()
            : this(1024)
        {

        }

        public override void Clear(bool wipeData = false)
        {
            if (wipeData)
                for (int i = 0; i < size; i++)
                    buffer[i] = 0;

            pos = size = 0;
        }

        public override void Write(Union v, int s)
        {
            if (pos > buffer.Length - s)
                ResizeBuffer(buffer.Length + s);
            if (pos > size - s)
                size++;

            for (int i = 0; i < s; i++)
                buffer[pos + i] = v[i];

            pos += s;
        }

        public override void WriteByte(byte value)
        {
            if (pos > buffer.Length - 1)
                ResizeBuffer(buffer.Length + 1);
            if (pos > size - 1)
                size++;

            buffer[pos++] = value;
        }
        public override void Write(byte[] data, int startIndex, int count)
        {
            if (pos > buffer.Length - count)
                ResizeBuffer(buffer.Length + count);
            if (pos > size - count)
                size += count;

            Array.Copy(data, startIndex, buffer, pos, count);
        }

        public override Union ReadUnion(int s)
        {
            if (pos + s > size)
                throw new EndOfStreamException();

            Union u = new Union();

            for (int i = 0; i < s; i++)
                u[i] = buffer[pos + i];

            return u;
        }

        public override byte ReadByte()
        {
            if (pos >= size)
                throw new EndOfStreamException();

            return buffer[pos++];
        }
        public override int Read(byte[] data, int startIndex, int count)
        {
            if (pos + count > size)
                count = BytesLeft;

            Array.Copy(buffer, pos, data, startIndex, count);

            pos += count;

            return count;
        }

        public override Stream AsStream()
        {
            return new MemoryStream(buffer);
        }
        public override byte[] AsByteArray()
        {
            return buffer;
        }

        // assuming n > 0
        static int FastLog2(int n)
        {
            int bits = 0;

            if (n > 0xFFFF)
            {
                n >>= 16;
                bits = 0x10;
            }
            if (n > 0xFF)
            {
                n >>= 8;
                bits |= 0x8;
            }
            if (n > 0xF)
            {
                n >>= 4;
                bits |= 0x4;
            }
            if (n > 0x3)
            {
                n >>= 2;
                bits |= 0x2;
            }
            if (n > 0x1)
                bits |= 0x1;

            return bits;
        }

        public void ResizeBuffer(int requiredLength)
        {
            if (requiredLength <= buffer.Length)
                return;

            var pow = requiredLength == 0 ? 1024 : FastLog2(requiredLength) - 1;

            Array.Resize(ref buffer, 2 << Math.Max(pow, 0));
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            buffer = null;
        }
    }
}
