using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.API.Defs;

namespace Prism.Util
{
    public static class JsonExtensions
    {
        public static object CastToObj(this JsonData j)
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
        public static T CastTo<T>(this JsonData from)
        {
            return from.CastToObj().CastTo<T>();
        }
        public static T CastTo<T>(this object o)
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
        public static T GetOrExn<T>(this JsonData j, string key)
        {
            if (j.Has(key))
                return j[key].CastTo<T>();

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
        public static T GetOrDef<T>(this JsonData j, string key, T def = default(T))
        {
            if (j.Has(key))
                return j[key].CastTo<T>();

            return def;
        }

        public static object ParseAsIntOrEntityInternalName(this JsonData j)
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
        public static TEnum ParseAsEnum<TEnum>(this JsonData j)
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

        public static ItemRef ParseItemRef(this JsonData j)
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
        public static NpcRef ParseNpcRef(this JsonData j)
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
        public static ProjectileRef ParseProjectileRef(this JsonData j)
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
        public static TileRef ParseTileRef(this JsonData j)
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
