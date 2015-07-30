using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Prism.Mods.Resources
{
    public interface IResourceReader : IDisposable
    {
        Type ResourceType
        {
            get;
        }

        object ReadResource(Stream resourceStream);
    }
    public abstract class ResourceReader<T> : IResourceReader
    {
        static bool TIsDisposable = Array.IndexOf(typeof(T).GetInterfaces(), typeof(IDisposable)) != -1;

        List<T> read = new List<T>();

        protected bool IsDisposed
        {
            get;
            private set;
        }

        public Type ResourceType
        {
            get
            {
                return typeof(T);
            }
        }

        public object ReadResource(Stream resourceStream)
        {
            var origPos = resourceStream.Position;

            var r = ReadTypedResource(resourceStream);

            resourceStream.Position = origPos; // reset the position so the resource can be read again

            read.Add(r);

            return r;
        }

        protected abstract T ReadTypedResource(Stream resourceStream);

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (TIsDisposable)
                for (int i = 0; i < read.Count; i++)
                    ((IDisposable)read[i]).Dispose();

            read.Clear();
            read = null;

            if (!IsDisposed)
                IsDisposed = true;
        }

        ~ResourceReader()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
