﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Prism.API.Defs
{
    /// <summary>
    /// Utility for coin value stuff: enemy drops, selling, buying, etc etc
    /// </summary>
    public struct CoinValue : IEquatable<CoinValue>, IEquatable<int>, IConvertible
    {
        public const int
            BASE = 100,

            COPPER_MULT   =                  1,
            SILVER_MULT   = BASE * COPPER_MULT,
            GOLD_MULT     = BASE * SILVER_MULT,
            PLATINUM_MULT = BASE * GOLD_MULT  ,

            COPPER_MAX   = BASE * COPPER_MULT  ,
            SILVER_MAX   = BASE * SILVER_MULT  ,
            GOLD_MAX     = BASE * GOLD_MULT    ,
            PLATINUM_MAX = BASE * PLATINUM_MULT;

        public readonly static CoinValue Zero = new CoinValue(0, 0, 0, 0);

        // base 10 is annoying
        public int
            Platinum,
            Gold    ,
            Silver  ,
            Copper  ;

        /// <summary>
        /// Gets or sets the resulting value.
        /// </summary>
        public int Value
        {
            get
            {
                return (Copper   % BASE) * COPPER_MULT
                     + (Silver   % BASE) * SILVER_MULT
                     + (Gold     % BASE) * GOLD_MULT
                     + (Platinum % BASE) * PLATINUM_MULT;
            }
            set
            {
                Copper   = value % COPPER_MAX;
                Silver   = value % SILVER_MAX   - Copper;
                Gold     = value % GOLD_MAX     - Silver;
                Platinum = value % PLATINUM_MAX - Gold  ;
            }
        }

        public CoinValue(int p, int g, int s, int c)
        {
            Copper = c;
            Silver = s;
            Gold = g;
            Platinum = p;
        }
        public CoinValue(int value)
        {
            Copper   =  value % COPPER_MAX                            ;
            Silver   = (value % SILVER_MAX   - Copper) / SILVER_MULT  ;
            Gold     = (value % GOLD_MAX     - Silver) / GOLD_MULT    ;
            Platinum = (value % PLATINUM_MAX - Gold  ) / PLATINUM_MULT;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (obj is int)
                return Value == (int)obj;
            if (obj is CoinValue)
                return Equals((CoinValue)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
        }

        public bool Equals(CoinValue other)
        {
            return Copper == other.Copper && Silver == other.Silver && Gold == other.Gold && Platinum == other.Platinum;
        }
        public bool Equals(int other)
        {
            return Value.Equals(other);
        }

        #region IConvertible stuff
        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Int32;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Value != 0;
        }
        char IConvertible.ToChar(IFormatProvider provider)
        {
            unchecked
            {
                return (char)Value;
            }
        }
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            unchecked
            {
                return (sbyte)Value;
            }
        }
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            unchecked
            {
                return (byte)Value;
            }
        }
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            unchecked
            {
                return (short)Value;
            }
        }
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            unchecked
            {
                return (ushort)Value;
            }
        }
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Value;
        }
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            unchecked
            {
                return (uint)Value;
            }
        }
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Value;
        }
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            unchecked
            {
                return (ulong)Value;
            }
        }
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Value;
        }
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Value;
        }
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Value;
        }
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            // meh
            throw new NotSupportedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return
                Platinum.ToString(provider) + "p " +
                Gold    .ToString(provider) + "g " +
                Silver  .ToString(provider) + "s " +
                Copper  .ToString(provider) + "c"  ;
        }
        #endregion

        public static bool operator ==(CoinValue a, CoinValue b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(CoinValue a, CoinValue b)
        {
            return !a.Equals(b);
        }

        public static explicit operator CoinValue(int v)
        {
            return new CoinValue() { Value = v };
        }
    }

    public struct AppliedBuff : IEquatable<AppliedBuff>
    {
        /// <summary>
        /// Gets or sets the type of buff.
        /// </summary>
        /// <remarks>Item.buffType</remarks>
        public readonly BuffRef Type;
        /// <summary>
        /// Gets or sets the duration for which this buff lasts.
        /// </summary>
        /// <remarks>Item.buffTime</remarks>
        public readonly int Duration;

        /// <summary>
        /// Constructs a new <see cref="AppliedBuff"/> structure.
        /// </summary>
        /// <param name="type"><see cref="AppliedBuff.Type"/></param>
        /// <param name="duration"><see cref="AppliedBuff.Duration"/></param>
        public AppliedBuff(BuffRef type, int duration)
        {
            Type = type;
            Duration = duration;
        }

        public bool Equals(AppliedBuff other)
        {
            return Type == other.Type && Duration == other.Duration;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (obj is AppliedBuff)
                return Equals((AppliedBuff)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return Type.GetHashCode() | Duration.GetHashCode();
        }
        public override string ToString()
        {
            return "{" + Type + ", Duration=" + Duration + "}";
        }

        public static bool operator ==(AppliedBuff a, AppliedBuff b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(AppliedBuff a, AppliedBuff b)
        {
            return !a.Equals(b);
        }
    }
}
