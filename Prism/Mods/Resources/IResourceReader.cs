using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

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

        Ref<T> read;

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
            if (read != null)
                return read.Value;

            read = new Ref<T>(ReadTypedResource(resourceStream));

            return read.Value;
        }

        protected abstract T ReadTypedResource(Stream resourceStream);

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (TIsDisposable)
                ((IDisposable)read.Value).Dispose();

            read = null;

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
