using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Defs;
using Prism.API;
using Prism.API.Audio;

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

        public static Either<int, ObjectRef> ParseAsIntOrObjectRef(this JsonData j)
        {
            if (j.IsInt)
                return Either.Right<int, ObjectRef>((int)j);
            else if (j.IsString || j.IsObject)
                return Either.Left<int, ObjectRef>(j.ParseAsObjectRef());

            throw new FormatException("Invalid entity reference type " + j.GetJsonType() + ", must be either int or {string}.");
        }
        public static ObjectRef ParseAsObjectRef(this JsonData j)
        {
            if (j.IsString)
                return new ObjectRef((string)j);
            else if (j.IsObject)
                foreach (KeyValuePair<string, JsonData> o in j)
                {
                    if (!o.Value.IsString)
                        throw new FormatException("Invalid object ref modname type " + o.Value.GetJsonType() + ", must be string.");

                    return new ObjectRef(o.Key, (string)o.Value);
                }

            throw new FormatException("Invalid object reference type " + j.GetJsonType() + ", must be either string or {string}.");
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

        public static BuffRef ParseBuffRef(this JsonData j)
        {
            return j.ParseAsIntOrObjectRef().Bind(i => new BuffRef(i), or => new BuffRef(or)).Right;
        }
        public static ItemRef ParseItemRef(this JsonData j)
        {
            return j.ParseAsIntOrObjectRef().Bind(i => new ItemRef(i), or => new ItemRef(or)).Right;
        }
        public static NpcRef ParseNpcRef(this JsonData j)
        {
            return j.ParseAsIntOrObjectRef().Bind(i => new NpcRef(i), or => new NpcRef(or)).Right;
        }
        public static ProjectileRef ParseProjectileRef(this JsonData j)
        {
            return j.ParseAsIntOrObjectRef().Bind(i => new ProjectileRef(i), or => new ProjectileRef(or)).Right;
        }
        public static TileRef ParseTileRef(this JsonData j)
        {
            return j.ParseAsIntOrObjectRef().Bind(i => new TileRef(i), or => new TileRef(or)).Right;
        }

        public static BgmRef ParseBgmRef(this JsonData j)
        {
            return j.ParseAsIntOrObjectRef().Bind(i => new BgmRef(VanillaBgms.RefOfId(i)), or => new BgmRef(or)).Right;
        }
    }
}
