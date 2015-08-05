using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Prism.API;
using Prism.API.Defs;
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

            string internalName = j.GetOrExn<string>("internalName");

            return new ModInfo(
                path,
                internalName, 
                j.GetOrDef<string>("displayName", internalName),
                j.GetOrDef("author", "<unspecified>"),
                j.GetOrDef("version", "0.0.0.0"),
                j.GetOrDef<string>("description"),
                j.GetOrExn<string>("asmFileName"),
                j.GetOrExn<string>("modDefTypeName"),
                refs.ToArray()
            );
        }

        public static object ParseAsIntOrEntityInternalName(JsonData j)
        {
            if (j.IsInt)
                return (int)j;
            else if (j.IsString)
                return (string)j;
            else if (j.IsObject)
                foreach (KeyValuePair<string, JsonData> o in j)
                {
                    if (!o.Value.IsString)
                        throw new FormatException("Invalid key/value pair value type " + o.Value.GetJsonType() + ", must be string.");

                    return Tuple.Create(o.Key, (string)o.Value);
                }

            throw new FormatException("Invalid entity reference type " + j.GetJsonType() + ", must be either int or {string}.");
        }
        public static TEnum ParseAsEnum<TEnum>(JsonData j)
            where TEnum : struct, IComparable, IConvertible
        {
            if (j.IsInt)
                return (TEnum)Enum.ToObject(typeof(TEnum), (int)j);
            else if (j.IsString)
            {
                TEnum v;
                if (Enum.TryParse((string)j, true, out v))
                    return v;

                throw new FormatException("Enum member '" + (string)j + "' not found in enum " + typeof(TEnum) + ".");
            }

            throw new FormatException("JsonData is not a valid enum value, it has type " + j.GetJsonType() + ".");
        }

        public static ItemRef       ParseItemRef      (JsonData j)
        {
            var o = ParseAsIntOrEntityInternalName(j);

            if (o is int)
                return new ItemRef((int)o);
            else if (o is string)
                return new ItemRef((string)o);
            else
            {
                var t = (Tuple<string, string>)o;

                return new ItemRef(t.Item1, t.Item2);
            }
        }
        public static NpcRef        ParseNpcRef       (JsonData j)
        {
            var o = ParseAsIntOrEntityInternalName(j);

            if (o is int)
                return new NpcRef((int)o);
            else if (o is string)
                return new NpcRef((string)o);
            else
            {
                var t = (Tuple<string, string>)o;

                return new NpcRef(t.Item1, t.Item2);
            }
        }
        public static ProjectileRef ParseProjectileRef(JsonData j)
        {
            var o = ParseAsIntOrEntityInternalName(j);

            if (o is int)
                return new ProjectileRef((int)o);
            else if (o is string)
                return new ProjectileRef((string)o);
            else
            {
                var t = (Tuple<string, string>)o;

                return new ProjectileRef(t.Item1, t.Item2);
            }
        }
        public static TileRef       ParseTileRef      (JsonData j)
        {
            var o = ParseAsIntOrEntityInternalName(j);

            if (o is int)
                return new TileRef((int)o);
            else if (o is string)
                return new TileRef((string)o);
            else
            {
                var t = (Tuple<string, string>)o;

                return new TileRef(t.Item1, t.Item2);
            }
        }
    }
}
