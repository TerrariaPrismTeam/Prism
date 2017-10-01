using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LitJson;
using Prism.API;
using Prism.Util;

namespace Prism.Mods
{
    /// <summary>
    /// Provides global access to the data of all mods loaded into Prism.
    /// </summary>
    public static class ModData
    {
        internal readonly static Dictionary<ModInfo, ModDef> mods = new Dictionary<ModInfo, ModDef>();
        internal readonly static Dictionary<string, ModDef> modsFromInternalName = new Dictionary<string, ModDef>();

        /// <summary>
        /// Contains all loaded mods indexed by their <see cref="ModInfo"/>.
        /// </summary>
        public readonly static ReadOnlyDictionary<ModInfo, ModDef> Mods = new ReadOnlyDictionary<ModInfo, ModDef>(mods);

        /// <summary>
        /// Contains all loaded mods indexed by their <see cref="ModInfo.InternalName"/>.
        /// </summary>
        public readonly static ReadOnlyDictionary<string, ModDef> ModsFromInternalName = new ReadOnlyDictionary<string, ModDef>(modsFromInternalName);
        // other dicts etc

        public static ModDef ModFromAssembly(Assembly modAsm)
        {
            return mods.Values.FirstOrDefault(d => d.Assembly == modAsm);
        }

        /// <summary>
        /// Parses the mod's information from Json, loading any required references, and returns its <see cref="ModInfo"/> object.
        /// </summary>
        /// <param name="j">Json Data to load the <see cref="ModInfo"/> from</param>
        /// <param name="path">The path to the mod</param>
        /// <returns>The <see cref="ModInfo"/> of the mod</returns>
        public static ModInfo ParseModInfo(JsonData j, string path)
        {
            var refs = new List<IReference>();

            if (j.Has("dllReferences"))
                foreach (object s in j["dllReferences"])
                    refs.Add(new AssemblyReference(s.ToString(), path));
            if (j.Has("modReferences"))
                foreach (KeyValuePair<string, JsonData> kvp in j["modReferences"])
                    refs.Add(new ModReference(kvp.Value.ToString(), ModInfo.ParseVer(kvp.Value.ToString())));

            string internalName = j.GetOrExn<string>("internalName");

            return new ModInfo(
                path,
                internalName,
                j.GetOrDef("displayName", internalName),
                j.GetOrDef("author", "<unspecified>"),
                j.GetOrDef("version", "0.0.0.0"),
                j.GetOrDef<string>("description"),
                j.GetOrExn<string>("asmFileName"),
                j.GetOrExn<string>("modDefTypeName"),
                refs.ToArray()
            );
        }
    }
}
