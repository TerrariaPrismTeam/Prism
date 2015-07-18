using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Prism.API;
using Prism.Util;

namespace Prism.Mods
{
    public static class ModData
    {
        internal readonly static Dictionary<ModInfo, ModDef> mods = new Dictionary<ModInfo, ModDef>();
        internal readonly static Dictionary<string, ModDef> modsFromInternalName = new Dictionary<string, ModDef>();

        public readonly static ReadOnlyDictionary<ModInfo, ModDef> Mods = new ReadOnlyDictionary<ModInfo, ModDef>(mods);
        public readonly static ReadOnlyDictionary<string, ModDef> ModsFromInternalName = new ReadOnlyDictionary<string, ModDef>(modsFromInternalName);
        // other dicts etc

        static T GetOrExn<T>(JsonData j, string key)
        {
            if (j.Has(key))
                return (T)Convert.ChangeType(j[key], typeof(T));

            throw new FormatException("Could not find property '" + key + "'.");
        }
        static T GetOrDef<T>(JsonData j, string key, T def = default(T))
        {
            if (j.Has(key))
                return (T)Convert.ChangeType(j[key], typeof(T));

            return def;
        }
        public static ModInfo ParseModInfo(JsonData j, string path)
        {
            List<IReference> refs = new List<IReference>();

            if (j.Has("dllReferences"))
                foreach (string s in j["dllReferences"])
                    refs.Add(new AssemblyReference(s));
            if (j.Has("modReferences"))
                foreach (string s in j["modReferences"])
                    refs.Add(new ModReference(s));

            return new ModInfo(
                path,
                GetOrExn<string>(j, "internalName"),
                GetOrExn<string>(j, "displayName"),
                GetOrDef(j, "author", "<unspecified>"),
                GetOrDef(j, "version", "0.0.0.0"),
                GetOrDef<string>(j, "description"),
                GetOrExn<string>(j, "asmFileName"),
                GetOrExn<string>(j, "modDefTypeName"),
                new IReference[0]
                );
        }
    }
}
