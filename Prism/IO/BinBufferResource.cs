using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Prism.IO
{
    public abstract class BinBufferResource : IDisposable
    {
        public abstract int Position
        {
            get;
            set;
        }
        public abstract bool IsEmpty
        {
            get;
        }
        public abstract int Size
        {
            get;
        }
        public abstract int BufferSize
        {
            get;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }
        public int BytesLeft
        {
            get
            {
                return Size - Position;
            }
        }
        public bool IsFilled
        {
            get
            {
                return BytesLeft == 0L;
            }
        }

        ~BinBufferResource()
        {
            if (!IsDisposed)
            {
                Dispose(false);

                IsDisposed = true;
            }
        }

        public abstract void Clear(bool wipeData = false);

        public abstract void Write(Union v, int size);

        public abstract void WriteByte(byte value);
        public abstract void Write(byte[] data, int startIndex, int count);

        public abstract Union ReadUnion(int size);

        public abstract byte ReadByte();
        public abstract int Read(byte[] data, int startIndex, int count);

        public abstract Stream AsStream();
        public abstract byte[] AsByteArray();

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);

                IsDisposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }
}
