using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prism.Util;
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

        List<T> read = new List<T>();

        protected bool IsDisposed
        {
            get;
            private set;
        }
        protected virtual bool ResetStream
        {
            get
            {
                return true;
            }
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

            if (ResetStream)
                resourceStream.Position = origPos;

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
    public class ContentResourceReader<T> : ResourceReader<T>
    {
        protected override T ReadTypedResource(Stream resourceStream)
        {
            using (var temp = new RandTempContentFile<T>(resourceStream, contentFate: StreamFate.DoNothing))
            {
                return temp.Load(Main.instance.Content);
            }
        }
    }
}
