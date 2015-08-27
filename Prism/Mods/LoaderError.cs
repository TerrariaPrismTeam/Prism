using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using Terraria;

namespace Prism.Mods
{
    /// <summary>
    /// Represents an error encountered while loading mods.
    /// </summary>
    public class LoaderError
    {
        /// <summary>
        /// Gets the path to the mod associated with this error.
        /// </summary>
        public string ModPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message associated with this error.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="ModInfo"/> of the mod associated with this error.
        /// </summary>
        public ModInfo? Mod
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the data associated with this error.
        /// </summary>
        public object Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new <see cref="LoaderError"/>.
        /// </summary>
        /// <param name="modPath">The path to the mod associated with this error</param>
        /// <param name="message">The message associated with this error</param>
        /// <param name="data">The data associated with this error</param>
        /// <param name="info">The <see cref="ModInfo"/> of the mod associated with this error</param>
        public LoaderError(string modPath, string message = null, object data = null, ModInfo? info = null)
        {
            if (String.IsNullOrEmpty(modPath))
                throw new ArgumentNullException("modPath");

            ModPath = modPath;
            Message = message;

            Mod = info;
            Data = data;
        }
        public LoaderError(ModInfo info, string message = null, object data = null)
            : this(info.ModPath, message, data, info)
        {

        }
        public LoaderError(string modPath, Exception e, ModInfo? info = null)
            : this(modPath, e == null ? null : e.Message, e, info)
        {

        }
        public LoaderError(ModInfo info, Exception e)
            : this(info.ModPath, e, info)
        {

        }

        static string Join(IEnumerable coll, string jw)
        {
            var b = new StringBuilder();

            bool notOnce = true;

            foreach (object o in coll)
            {
                if (notOnce)
                    b.Append(jw);

                b.Append(PrettyPrintObject(o));
            }

            return b.ToString();
        }
        static string PrettyPrintObject(object o)
        {
            var t = o.GetType();

            if (t.IsPrimitive || o is string || o is Exception)
                return o.ToString();
            if (o is JsonData)
                return JsonMapper.ToJson((JsonData)o);
            if (o is IEnumerable)
                return "[" + Join((IEnumerable)o, ", ") + "]";
            if (t == typeof(Ref<>))
                return PrettyPrintObject(((dynamic)o).Value);
            if (t == typeof(Tuple<>))
            {
                var tu = (dynamic)o;

                return "(" + tu.Item1 + ", " + tu.Item2 + ")";
            }
            if (t == typeof(Tuple<,>))
            {
                var tu = (dynamic)o;

                return "(" + tu.Item1 + ", " + tu.Item2 + ", " + tu.Item3 + ")";
            }
            if (t == typeof(Nullable<>))
            {
                var n = (dynamic)o;

                return n.HasValue ? "Just " + PrettyPrintObject(n.Value) : "Nothing";
            }
            if (t == typeof(Lazy<>))
            {
                var l = (dynamic)o;

                return PrettyPrintObject(l.Value);
            }
            if (t == typeof(KeyValuePair<,>))
            {
                var p = (dynamic)o;

                return "(" + p.Key + " => " + p.Value + ")";
            }

            return o.ToString();
        }

        /// <summary>
        /// Gets the string representation of this error including its <see cref="ModPath"/>, <see cref="ModInfo"/>, <see cref="Message"/>, and <see cref="Data"/>.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return "An error occured when trying to load mod '"
                + (Mod.HasValue ? Mod.Value.InternalName : Path.GetFileName(ModPath)) + "'"
                + (Message == null ? String.Empty : ": " + Message)
                + (Data    == null ? String.Empty : ": " + PrettyPrintObject(Data)) + ".";
        }
    }
}
