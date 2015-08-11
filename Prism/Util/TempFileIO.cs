using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Prism.Debugging;
using Terraria;

namespace Prism.Util
{
    public enum StreamFate
    {
        DoNothing,
        CloseOnly,
        CloseAndDispose
    }

    public class TempFile : IDisposable
    {
        public string FilePath;
        public bool DoNotOverrite;
        public StreamFate ContentFate;
        public Stream ContentStream;
        public FileStream FStream;

        public TempFile(bool dontOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
        {
            ContentFate = contentFate;
            DoNotOverrite = dontOverrite;
        }

        public TempFile(Stream contents, bool dontOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : this(dontOverrite, contentFate)
        {
            ContentStream = contents;
        }

        public TempFile(string resourceName, bool dontOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : this(dontOverrite, contentFate)
        {
            ContentStream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);
        }

        public TempFile(string path, Stream contents, bool dontOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : this(contents, dontOverrite, contentFate)
        {
            FilePath = path;
            Init();
        }

        public TempFile(string path, string resourceName, bool dontOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : this(resourceName, dontOverrite, contentFate)
        {
            FilePath = path;
            Init();
        }

        public void Init()
        {
            string dir = Path.GetDirectoryName(FilePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else
                foreach (var f in Directory.EnumerateFiles(dir))
                    File.Delete(f);

            if (File.Exists(FilePath))
            {
                if (DoNotOverrite)
                    throw new ArgumentException("A file already exists with the specified path.", "path");
                else
                    File.Delete(FilePath);
            }

            Logging.LogInfo("Creating temp file " + FilePath);

            FStream = File.Create(FilePath);
            ContentStream.CopyTo(FStream);

            if (ContentFate == StreamFate.CloseOnly || ContentFate == StreamFate.CloseAndDispose)
                ContentStream.Close();
            if (ContentFate == StreamFate.CloseAndDispose)
            {
                ContentStream.Dispose();
                ContentStream = null;
            }

            FStream.Close();
            FStream.Dispose();
            FStream = null;
        }

        public void Dispose()
        {
            Logging.LogInfo("Deleting temp file " + FilePath);

            if (FStream != null)
            {
                FStream.Dispose();
                FStream = null;
            }
            if (ContentStream != null)
            {
                ContentStream.Dispose();
                ContentStream = null;
            }
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            FilePath = null;
        }
    }

    public class RandTempFile : TempFile
    {
        public RandTempFile(Stream contents, string directory = null, string extension = null, bool doNotOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : base(contents, doNotOverrite, contentFate)
        {
            FilePath = Path.Combine(directory ?? Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), extension));

            Init();
        }

        public RandTempFile(string resourceName, string directory = null, string extension = null, bool doNotOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : base(resourceName, doNotOverrite, contentFate)
        {
            FilePath = Path.Combine(directory ?? Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), extension));

            Init();
        }
    }

    public class RandTempContentFile<T> : TempFile
    {
        public readonly string ContentPath;
        public T Load(ContentManager content = null)
        {
            return (content ?? Main.instance.Content).Load<T>(ContentPath);
        }

        public RandTempContentFile(Stream contents, bool doNotOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : base(contents, doNotOverrite, contentFate)
        {
            ContentPath = Path.Combine("Prism_Temp", Guid.NewGuid().ToString());
            FilePath = Path.Combine("Content", Path.ChangeExtension(ContentPath, "xnb"));

            Init();
        }

        public RandTempContentFile(string resourceName, bool doNotOverrite = false, StreamFate contentFate = StreamFate.CloseAndDispose)
            : base(resourceName, doNotOverrite, contentFate)
        {
            ContentPath = Path.Combine("Prism_Temp", Guid.NewGuid().ToString());
            FilePath = Path.Combine("Content", Path.ChangeExtension(ContentPath, "xnb"));

            Init();
        }
    }
}
