using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Util
{
    [Flags]
    public enum EitherKind
    {
        Right = 1,
        Left  = 2
    }

    public static class Either
    {
        public static Either<T1, T2> Right<T1, T2>(T1 value)
        {
            return new Either<T1, T2>(EitherKind.Right, value);
        }
        public static Either<T1, T2> Left <T1, T2>(T2 value)
        {
            return new Either<T1, T2>(EitherKind.Left , value);
        }
    }
    public struct Either<T1, T2> : IEquatable<Either<T1, T2>>
    {
        public readonly EitherKind Kind;
        readonly object v;

        public bool IsLeft
        {
            get
            {
                return Kind == EitherKind.Left;
            }
        }
        public bool IsRight
        {
            get
            {
                return Kind == EitherKind.Right;
            }
        }

        public T1 Right
        {
            get
            {
                if (!Equals(Kind, EitherKind.Right))
                    throw new InvalidOperationException();

                return (T1)v;
            }
        }
        public T2 Left
        {
            get
            {
                if (!Equals(Kind, EitherKind.Left))
                    throw new InvalidOperationException();

                return (T2)v;
            }
        }

        internal Either(EitherKind kind, object innerValue)
        {
            Kind = typeof(T1) == typeof(T2) ? EitherKind.Left | EitherKind.Right : kind;
            v = innerValue;
        }

        public static Either<T1, T2> NewRight(T1 value)
        {
            return new Either<T1, T2>(EitherKind.Right, value);
        }
        public static Either<T1, T2> NewLeft (T2 value)
        {
            return new Either<T1, T2>(EitherKind.Left, value);
        }

        static bool Equals(EitherKind a, EitherKind b)
        {
            return (a & b) != 0;
        }

        public bool Equals(Either<T1, T2> other)
        {
            if (!Equals(Kind, other.Kind))
                return false;

            if (ReferenceEquals(v, null))
                return ReferenceEquals(other.v, null);
            if (ReferenceEquals(other.v, null))
                return false;

            if (Kind == EitherKind.Right)
            {
                if (v is IEquatable<T1>)
                    return ((IEquatable<T1>)v).Equals(other.v);
            }
            else if (v is IEquatable<T2>)
                return ((IEquatable<T2>)v).Equals(other.v);

            return v.Equals(other.v);
        }

        bool Equals(T1 other)
        {
            if (Kind != EitherKind.Right)
                return false;

            if (ReferenceEquals(v, null))
                return ReferenceEquals(other, null);
            if (ReferenceEquals(other, null))
                return false;

            if (v is IEquatable<T1>)
                return ((IEquatable<T1>)v).Equals(other);

            return v.Equals(other);
        }
        bool Equals(T2 other)
        {
            if (Kind != EitherKind.Left)
                return false;

            if (ReferenceEquals(v, null))
                return ReferenceEquals(other, null);
            if (ReferenceEquals(other, null))
                return false;

            if (v is IEquatable<T2>)
                return ((IEquatable<T2>)v).Equals(other);

            return v.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is Either<T1, T2>)
                return Equals((Either<T1, T2>)obj);
            if (obj is T1)
                return Equals((T1)obj);
            if (obj is T2)
                return Equals((T2)obj);

            return false;
        }
        public override int GetHashCode()
        {
            var h = ReferenceEquals(v, null) ? 0 : v.GetHashCode();

            if (Kind == EitherKind.Right)
                h |=  1;
            else
                h &= ~1;

            return h;
        }
        public override string ToString()
        {
            return (Kind == (EitherKind.Right | EitherKind.Left) ? String.Empty : (Kind == EitherKind.Right ? "Right " : "Left ")) + v.SafeToString("<null>");
        }

        public static bool operator ==(Either<T1, T2> a, Either<T1, T2> b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Either<T1, T2> a, Either<T1, T2> b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Either<T1, T2> a, T1 b)
        {
            return Equals(a.Kind, EitherKind.Right) && a.Equals(b);
        }
        public static bool operator !=(Either<T1, T2> a, T1 b)
        {
            return !Equals(a.Kind, EitherKind.Right) || !a.Equals(b);
        }
        public static bool operator ==(T1 a, Either<T1, T2> b)
        {
            return Equals(b.Kind, EitherKind.Right) && b.Equals(a);
        }
        public static bool operator !=(T1 a, Either<T1, T2> b)
        {
            return !Equals(b.Kind, EitherKind.Right) || !b.Equals(a);
        }

        public static bool operator ==(Either<T1, T2> a, T2 b)
        {
            return Equals(a.Kind, EitherKind.Left) && a.Equals(b);
        }
        public static bool operator !=(Either<T1, T2> a, T2 b)
        {
            return !Equals(a.Kind, EitherKind.Left) || !a.Equals(b);
        }
        public static bool operator ==(T2 a, Either<T1, T2> b)
        {
            return Equals(b.Kind, EitherKind.Left) && b.Equals(a);
        }
        public static bool operator !=(T2 a, Either<T1, T2> b)
        {
            return !Equals(b.Kind, EitherKind.Left) || !b.Equals(a);
        }
    }
}
