using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Util
{
    public static class Maybe
    {
        public static Maybe<T> Just<T>(T value) => new Maybe<T>(value);
    }
    public struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T> // Nullable is only for structs, need support for any T
    {
        public readonly static Maybe<T> Nothing = new Maybe<T>();

        public readonly bool HasValue;
        readonly T v;

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException();

                return v;
            }
        }

        public Maybe(T value)
        {
            HasValue = true;
            v = value;
        }

        public bool Equals(Maybe<T> other)
        {
            if (!HasValue)
                return !other.HasValue;

            if (ReferenceEquals(v, null))
                return ReferenceEquals(other.v, null);
            if (ReferenceEquals(other.v, null))
                return false;

            if (v is IEquatable<T>)
                return ((IEquatable<T>)v).Equals(other.v);

            return v.Equals(other.v);
        }
        public bool Equals(T other)
        {
            if (!HasValue)
                return false;

            if (ReferenceEquals(v, null))
                return ReferenceEquals(other, null);
            if (ReferenceEquals(other, null))
                return false;

            if (v is IEquatable<T>)
                return ((IEquatable<T>)v).Equals(other);

            return v.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is Maybe<T>)
                return Equals((Maybe<T>)obj);
            if (obj is T)
                return Equals((T)obj);

            return false;
        }
        public override int GetHashCode()
        {
            if (!HasValue)
                return 0;

            return ReferenceEquals(v, null) ? 0 : v.GetHashCode();
        }
        public override string ToString()
        {
            return HasValue ? "Just " + v.SafeToString("<null>") : "Nothing";
        }

        public static bool operator ==(Maybe<T> a, Maybe<T> b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(Maybe<T> a, Maybe<T> b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Maybe<T> a, T b)
        {
            return a.HasValue && a.Equals(b);
        }
        public static bool operator !=(Maybe<T> a, T b)
        {
            return !a.HasValue || !a.Equals(b);
        }
        public static bool operator ==(T a, Maybe<T> b)
        {
            return b.HasValue && b.Equals(a);
        }
        public static bool operator !=(T a, Maybe<T> b)
        {
            return !b.HasValue || !b.Equals(a);
        }
    }
}
