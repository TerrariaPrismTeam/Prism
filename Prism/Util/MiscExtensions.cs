using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Prism.Util
{
    public static class MiscExtensions
    {
        public static string SafeToString(this object v, string defValue = null)
        {
            if (ReferenceEquals(v, null))
                return defValue;

            return v.ToString();
        }

        public static Ref<TOut> Bind<TIn, TOut>(this Ref<TIn> m, Func<TIn, TOut> map)
        {
            if (m == null)
                return null;
            if (ReferenceEquals(m.Value, null))
                return new Ref<TOut>(default(TOut));

            return new Ref<TOut>(map(m.Value));
        }
        public static Maybe<TOut> Bind<TIn, TOut>(this Maybe<TIn> m, Func<TIn, TOut> map)
        {
            if (m.HasValue)
                return Maybe.Just(map(m.Value));

            return Maybe<TOut>.Nothing;
        }
        public static Either<TOut1, TOut2> Bind<TIn1, TOut1, TIn2, TOut2>(this Either<TIn1, TIn2> m, Func<TIn1, TOut1> mapR, Func<TIn2, TOut2> mapL)
        {
            if (m.Kind == EitherKind.Right)
                return Either.Right<TOut1, TOut2>(mapR(m.Right));

            return Either.Left<TOut1, TOut2>(mapL(m.Left));
        }
        public static TOut? Bind<TIn, TOut>(this TIn? m, Func<TIn, TOut> map)
            where TIn  : struct
            where TOut : struct
        {
            return m.HasValue ? map(m.Value) : default(TOut);
        }
        public static Lazy<TOut> Bind<TIn, TOut>(this Lazy<TIn> m, Func<TIn, TOut> map)
        {
            return new Lazy<TOut>(() => map(m.Value));
        }
        public static KeyValuePair<TOutKey, TOutValue> Bind<TInKey, TOutKey, TInValue, TOutValue>(this KeyValuePair<TInKey, TInValue> m, Func<TInKey, TOutKey> mapK, Func<TInValue, TOutValue> mapV)
        {
            return new KeyValuePair<TOutKey, TOutValue>(mapK(m.Key), mapV(m.Value));
        }
        public static Tuple<TOut1, TOut2> Bind<TIn1, TOut1, TIn2, TOut2>(this Tuple<TIn1, TIn2> m, Func<TIn1, TOut1> map1, Func<TIn2, TOut2> map2)
        {
            return m == null ? null : Tuple.Create(map1(m.Item1), map2(m.Item2));
        }
        public static Tuple<TOut1, TOut2, TOut3> Bind<TIn1, TOut1, TIn2, TOut2, TIn3, TOut3>(this Tuple<TIn1, TIn2, TIn3> m, Func<TIn1, TOut1> map1, Func<TIn2, TOut2> map2, Func<TIn3, TOut3> map3)
        {
            return m == null ? null : Tuple.Create(map1(m.Item1), map2(m.Item2), map3(m.Item3));
        }
    }
}
