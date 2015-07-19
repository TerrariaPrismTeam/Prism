using System;
using System.Collections.Generic;
using System.Linq;
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

        static object CastJsonToT(JsonData j)
        {
            switch (j.GetJsonType())
            {
                case JsonType.Boolean:
                    return (bool)j;
                case JsonType.Double:
                    return (double)j;
                case JsonType.Int:
                    return (int)j;
                case JsonType.Long:
                    return (long)j;
                case JsonType.None:
                    return null;
                case JsonType.Array:
                case JsonType.Object:
                    return j;
                case JsonType.String:
                    return (string)j;
            }

            throw new InvalidCastException();
        }
        static T CastObjToT<T>(object o)
        {
            if (o is T)
                return (T)o;

            return (T)Convert.ChangeType(o, typeof(T));
        }
        /// <summary>
        /// Gets the property from the Json data or throws an exception if it fails (See <see cref="GetOrDef{T}(JsonData, string, T)"/> to return a default on failure)."/>
        /// </summary>
        /// <typeparam name="T">Type to convert the Json property to</typeparam>
        /// <param name="j">The Json Data</param>
        /// <param name="key">The Json Property's Key</param>
        /// <returns>The Json data</returns>
        static T GetOrExn<T>(JsonData j, string key)
        {
            if (j.Has(key))
                return CastObjToT<T>(CastJsonToT(j[key]));

            throw new FormatException("Could not find property '" + key + "'.");
        }

        /// <summary>
        /// Gets the property from the Json data or returns a specified default if it fails (See <see cref="GetOrExn{T}(JsonData, string)"/> to throw an exception on failure)."/>
        /// </summary>
        /// <typeparam name="T">Type to convert the Json property to</typeparam>
        /// <param name="j">The Json Data</param>
        /// <param name="key">The Json Property's Key</param>
        /// <param name="def">The default T value to return. Defaults to default(T).</param>
        /// <returns>Either the Json data, if successful, or the default, if not successful.</returns>
        static T GetOrDef<T>(JsonData j, string key, T def = default(T))
        {
            if (j.Has(key))
                return CastObjToT<T>(CastJsonToT(j[key]));

            return def;
        }

        /// <summary>
        /// Parses the mod's information from Json, loading any required references, and returns its <see cref="ModInfo"/> object.
        /// </summary>
        /// <param name="j">Json Data to load the <see cref="ModInfo"/> from</param>
        /// <param name="path">The path to the mod</param>
        /// <returns>The <see cref="ModInfo"/> of the mod</returns>
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
                refs.ToArray()
            );
        }
    }
}
