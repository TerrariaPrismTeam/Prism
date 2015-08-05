using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
