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
    }
}
