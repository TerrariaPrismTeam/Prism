using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Prism.API;

namespace Prism.Mods.Resources
{
    public static class ResourceLoader
    {
        internal static Dictionary<Type, IResourceReader> ResourceReaders = new Dictionary<Type, IResourceReader>();

        internal static string NormalizeResourceFilePath(string file, string basePath = null)
        {
            if (!String.IsNullOrEmpty(basePath) && file.StartsWith(basePath, StringComparison.Ordinal))
                file = file.Substring(basePath.Length + 1);

            return file.Replace('\\', '/').ToLowerInvariant();
        }

        public static void RegisterReader(Type t, IResourceReader reader)
        {
            ResourceReaders.Add(t, reader);
        }
        public static void RegisterReader<T>(ResourceReader<T> reader)
        {
            ResourceReaders.Add(typeof(T), reader);
        }

        internal static void Setup()
        {
            RegisterReader(new Texture2DResourceReader  ());
            RegisterReader(new SoundEffectResourceReader());
            RegisterReader(new StringResourceReader     ());
        }
        internal static IEnumerable<LoaderError> Load(ModDef mod)
        {
            List<LoaderError> errors = new List<LoaderError>();

            foreach (var f in Directory.EnumerateFiles(mod.Info.ModPath, "*.*", SearchOption.AllDirectories))
            {
                var nf = NormalizeResourceFilePath(f, mod.Info.ModPath);
                string fn = Path.GetFileName(f);

                if (fn == PrismApi.JsonManifestFileName || fn == NormalizeResourceFilePath(mod.Info.AssemblyFileName) ||
                        mod.Info.References.Where(r => r is AssemblyReference).Select(r => NormalizeResourceFilePath(((AssemblyReference)r).Name)).Any(s => Path.GetFileName(s) == fn))
                    continue; // shouldn't be used, obviously

                try
                {
                    mod.resources.Add(nf, File.OpenRead(f));
                }
                catch (Exception e)
                {
                    errors.Add(new LoaderError(mod.Info, "Error reading resource", e));
                }
            }

            return errors;
        }

        internal static void Unload()
        {
            foreach (var v in ResourceReaders.Values)
                v.Dispose();

            ResourceReaders.Clear();
        }
    }
}
