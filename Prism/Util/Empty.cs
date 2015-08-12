using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Prism.Util
{
    // use these instead of allocating a new object every time

    /// <summary>
    /// A value type that holds nothing, a <see cref="void" /> that can be used as a type argument.
    /// </summary>
    public enum Unit : byte { Value }

    public static class Empty
    {
        [Obsolete("Please use String.Empty instead.")]
        public readonly static string String = String.Empty;
    }
    public static class Empty<T>
    {
        [Obsolete("Please use the default keyword instead.")]
        public readonly static T Value = default(T);
        public readonly static T[] Array = new T[0];
        public readonly static List<T> List = new List<T>();
        public readonly static Ref<T> Ref = new Ref<T>(default(T));

        public readonly static Action<T> Action = _  => {        };
        public readonly static Func  <T> Func   = () => default(T);
    }
    public static class Empty<T1, T2>
    {
        public readonly static Dictionary<T1, T2> Dictionary = new Dictionary<T1, T2>(                        );
        public readonly static Tuple     <T1, T2> Tuple      = new Tuple     <T1, T2>(default(T1), default(T2));

        public readonly static Action<T1, T2> Action = (_, __) => {         };
        public readonly static Func  <T1, T2> Func   =  _      => default(T2);
    }
}
