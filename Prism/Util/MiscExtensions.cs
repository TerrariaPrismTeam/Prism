using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;

namespace Prism.Util
{
    public static partial class MiscExtensions
    {
        // see LinqExt
        //public static T Identity<T>(T t)
        //{
        //    return t;
        //}

        public static Ref<TOut> Map<TIn, TOut>(this Ref<TIn> m, Func<TIn, TOut> map)
        {
            if (m == null)
                return null;
            if (ReferenceEquals(m.Value, null))
                return new Ref<TOut>(default(TOut));

            return new Ref<TOut>(map(m.Value));
        }

        public static Maybe<TOut> Map<TIn, TOut>(this Maybe<TIn> m, Func<TIn, TOut> map)
        {
            if (m.HasValue)
                return Maybe.Just(map(m.Value));

            return Maybe<TOut>.Nothing;
        }
        public static Either<TOut1, TOut2> Map<TIn1, TOut1, TIn2, TOut2>(this Either<TIn1, TIn2> m, Func<TIn1, TOut1> mapR, Func<TIn2, TOut2> mapL)
        {
            if (m.Kind == EitherKind.Right)
                return Either.Right<TOut1, TOut2>(mapR(m.Right));

            return Either.Left<TOut1, TOut2>(mapL(m.Left));
        }
        public static TOut? Map<TIn, TOut>(this TIn? m, Func<TIn, TOut> map)
            where TIn  : struct
            where TOut : struct
        {
            return m.HasValue ? map(m.Value) : default(TOut?);
        }
        public static Lazy<TOut> Map<TIn, TOut>(this Lazy<TIn> m, Func<TIn, TOut> map)
        {
            return new Lazy<TOut>(() => map(m.Value));
        }
        public static KeyValuePair<TOutKey, TOutValue> Map<TInKey, TOutKey, TInValue, TOutValue>(this KeyValuePair<TInKey, TInValue> m, Func<TInKey, TOutKey> mapK, Func<TInValue, TOutValue> mapV)
        {
            return new KeyValuePair<TOutKey, TOutValue>(mapK(m.Key), mapV(m.Value));
        }
        public static Tuple<TOut1, TOut2> Map<TIn1, TOut1, TIn2, TOut2>(this Tuple<TIn1, TIn2> m, Func<TIn1, TOut1> map1, Func<TIn2, TOut2> map2)
        {
            return m == null ? null : Tuple.Create(map1(m.Item1), map2(m.Item2));
        }
        public static Tuple<TOut1, TOut2, TOut3> Map<TIn1, TOut1, TIn2, TOut2, TIn3, TOut3>(this Tuple<TIn1, TIn2, TIn3> m, Func<TIn1, TOut1> map1, Func<TIn2, TOut2> map2, Func<TIn3, TOut3> map3)
        {
            return m == null ? null : Tuple.Create(map1(m.Item1), map2(m.Item2), map3(m.Item3));
        }
        public static IEnumerable<TOut> Map<TIn, TOut>(this IEnumerable<TIn> m, Func<TIn, IEnumerable<TOut>> map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            if (m == null)
                yield break;

            foreach (var t in m)
                foreach (var t_ in map(t))
                    yield return t_;

            yield break;
        }

        public static Func<    T> Delay<T    >(T value)
        {
            return () => value;
        }
        public static Func<T2, T> Delay<T, T2>(T value)
        {
            return _ => value;
        }

        public static TResult Match<T1, T2, TResult>(this Either<T1, T2> m, Func<T1, TResult> mapR, Func<T2, TResult> mapL)
        {
            if (m.Kind == EitherKind.Right)
                return mapR(m.Right);
            else
                return mapL(m.Left );
        }

        public static string Name(this Entity e)
        {
            if (e is Player)
                return ((Player)e).name;
            if (e is NPC)
                return ((NPC)e).GivenOrTypeName;
            if (e is Projectile)
                return ((Projectile)e).Name;
            if (e is Item)
                return ((Item)e).Name;

            return e.ToString(); // *shrug*
        }

        public static NetworkText ToNT(this string s)
        {
            return NetworkText.FromLiteral(s);
        }
    }
}
