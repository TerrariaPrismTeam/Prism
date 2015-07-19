using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Prism.API
{
    public enum ItemDamageType
    {
        None,
        Melee,
        Ranged,
        Magic,
        Summon,
        Thrown
    }
    public enum ItemUseStyle
    {
        None,
        /// <summary>
        /// Swords, pickaxes, etc.
        /// </summary>
        Swing,
        /// <summary>
        /// Consumable items.
        /// </summary>
        Eat,
        /// <summary>
        /// Shortswords.
        /// </summary>
        Stab,
        /// <summary>
        /// Summoning items, heart/mana crystal, etc.
        /// </summary>
        HoldUp,
        /// <summary>
        /// Guns, spell tomes, etc.
        /// </summary>
        AimToMouse
    }
    public enum ItemHoldStyle
    {
        Default,
        HeldLightSource, // torch etc
        BreathingReed
    }
    public enum ItemRarity
    {
        /// <summary>
        /// Rarity of quest items.
        /// </summary>
        Amber = -3, // Item.questItem
        /// <summary>
        /// Rarity of items only obtainable in Expert Mode.
        /// </summary>
        Rainbow = -2, // Item.expert (not expertOnly)
        Gray = -1,
        White = 0,
        Blue = 1,
        Green = 2,
        Orange = 3,
        LightRed = 4,
        Pink = 5,
        LightPurple = 6,
        Lime = 7,
        Yellow = 8,
        Cyan = 9,
        Red = 10,
        Purple = 11
    }

    /// <summary>
    /// Utility for Item.value stuff.
    /// </summary>
    public struct ItemValue : IEquatable<ItemValue>, IEquatable<int>, IConvertible
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

        public readonly static ItemValue Zero = new ItemValue(0, 0, 0, 0);

        // base 10 is annoying
        public int Copper
        {
            get;
            set;
        }
        public int Silver
        {
            get;
            set;
        }
        public int Gold
        {
            get;
            set;
        }
        public int Platinum
        {
            get;
            set;
        }

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
                Copper   = value % COPPER_MAX           ;
                Silver   = value % SILVER_MAX   - Copper;
                Gold     = value % GOLD_MAX     - Silver;
                Platinum = value % PLATINUM_MAX - Gold  ;
            }
        }

        public ItemValue(int c, int s, int g = 0, int p = 0)
        {
            Copper   = c;
            Silver   = s;
            Gold     = g;
            Platinum = p;
        }
        public ItemValue(int value)
        {
            Copper   = value % COPPER_MAX           ;
            Silver   = value % SILVER_MAX   - Copper;
            Gold     = value % GOLD_MAX     - Silver;
            Platinum = value % PLATINUM_MAX - Gold  ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (obj is int)
                return Value == (int)obj;
            if (obj is ItemValue)
                return Equals((ItemValue)obj);

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

        public bool Equals(ItemValue other)
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
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return Copper.ToString(provider) + "c " + Silver.ToString(provider) + "s " + Gold.ToString(provider) + "g " + Platinum.ToString(provider) + "p";
        }
        #endregion

        public static bool operator ==(ItemValue a, ItemValue b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(ItemValue a, ItemValue b)
        {
            return !a.Equals(b);
        }
    }
    /// <summary>
    /// Container for the properties of the item which relate to player buffs.
    /// I've chosen more friendly/less-vanilla-y names for the fields because
    /// Prism should be the last modding API we *ever* need for Terraria and
    /// I want it to be really nice and user-friendly.
    /// </summary>
    public struct ItemBuff : IEquatable<ItemBuff>
    {
        // TODO: use BuffRef... later
        /// <summary>
        /// Gets or sets the type of buff this item gives the player.
        /// </summary>
        /// <remarks>Item.buffType</remarks>
        public int Type
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the duration for which this item buffs the player (in whole seconds).
        /// </summary>
        /// <remarks>Item.buffTime</remarks>
        public int Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a new <see cref="ItemBuff"/> structure.
        /// </summary>
        /// <param name="type"><see cref="ItemBuff.Type"/></param>
        /// <param name="duration"><see cref="ItemBuff.Duration"/></param>
        public ItemBuff(int type, int duration)
        {
            Type = type;
            Duration = duration;
        }

        public bool Equals(ItemBuff other)
        {
            return Type == other.Type && Duration == other.Duration;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (obj is ItemBuff)
                return Equals((ItemBuff)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return Type | Duration;
        }
        public override string ToString()
        {
            return "{" + Type + ", Duration=" + Duration + "}";
        }

        public static bool operator ==(ItemBuff a, ItemBuff b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(ItemBuff a, ItemBuff b)
        {
            return !a.Equals(b);
        }
    }
    /// <summary>
    /// Container for the properties of the item which determine its use as armor.
    /// </summary>
    public struct ItemArmourData : IEquatable<ItemArmourData>
    {
        public int HeadId
        {
            get;
            internal set;
        }
        public int MaleBodyId
        {
            get;
            internal set;
        }
        public int FemaleBodyId
        {
            get;
            internal set;
        }
        public int LegsId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets whether the item can be used as a helmet.
        /// </summary>
        /// <remarks>Item.headSlot</remarks>
        public Func<Texture2D> Helmet
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item can be used as body armour.
        /// </summary>
        /// <remarks>Item.bodySlot</remarks>
        public Func<Texture2D> MaleBodyArmour
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item can be used as body armour.
        /// </summary>
        /// <remarks>Item.bodySlot</remarks>
        public Func<Texture2D> FemaleBodyArmour
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item can be used as greaves.
        /// </summary>
        /// <remarks>Item.legSlot</remarks>
        public Func<Texture2D> Greaves
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a new <see cref="ItemArmourData"/> structure.
        /// </summary>
        /// <param name="head"><see cref="Helmet"/></param>
        /// <param name="body"><see cref="BodyArmour"/></param>
        /// <param name="legs"><see cref="Greaves"/></param>
        public ItemArmourData(Func<Texture2D> head, Func<Texture2D> maleBody, Func<Texture2D> legs, Func<Texture2D> femaleBody = null)
        {
            HeadId = MaleBodyId = FemaleBodyId = LegsId = -1;

            Helmet = head;
            MaleBodyArmour = maleBody;
            Greaves = legs;

            FemaleBodyArmour = femaleBody ?? maleBody;
        }

        public bool Equals(ItemArmourData other)
        {
            return Helmet == other.Helmet && MaleBodyArmour == other.MaleBodyArmour && FemaleBodyArmour == other.FemaleBodyArmour && Greaves == other.Greaves;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is ItemArmourData)
                return Equals((ItemArmourData)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return Helmet.GetHashCode() + MaleBodyArmour.GetHashCode() + FemaleBodyArmour.GetHashCode() + Greaves.GetHashCode();
        }

        public static bool operator ==(ItemArmourData a, ItemArmourData b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(ItemArmourData a, ItemArmourData b)
        {
            return !a.Equals(b);
        }
    }
    /// <summary>
    /// Container for the properties of the item which relate to its description.
    /// </summary>
    public struct ItemDescription : IEquatable<ItemDescription>
    {
        /// <summary>
        /// Gets or sets the item's description.
        /// </summary>
        /// <remarks>Item.toolTip</remarks>
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the item's extra description (funny quote, reference, etc).
        /// </summary>
        /// <remarks>Item.toolTip2</remarks>
        public string ExtraDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this item is labeled as "Vanity item" in its tool-tip.
        /// </summary>
        /// <remarks>Item.vanity</remarks>
        public bool ShowVanity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item is labeled as "Ammo" in its tool-tip.
        /// </summary>
        /// <remarks>!Item.notAmmo</remarks>
        public bool HideAmmoFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a new <see cref="ItemDescription"/> structure.
        /// </summary>
        /// <param name="desc"><see cref="ItemDescription.Description"/></param>
        /// <param name="extraDesc"><see cref="ItemDescription.ExtraDescription"/></param>
        /// <param name="vanity"><see cref="ItemDescription.ShowVanity"/></param>
        /// <param name="hideAmmo"><see cref="ItemDescription.HideAmmoFlag"/></param>
        public ItemDescription(string desc, string extraDesc, bool vanity, bool hideAmmo)
        {
            Description = desc;
            ExtraDescription = extraDesc;
            ShowVanity = vanity;
            HideAmmoFlag = hideAmmo;
        }

        public bool Equals(ItemDescription other)
        {
            return Description == other.Description && ExtraDescription == other.ExtraDescription && ShowVanity == other.ShowVanity && HideAmmoFlag == other.HideAmmoFlag;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is ItemDescription)
                return Equals((ItemDescription)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return (Description.GetHashCode() & ExtraDescription.GetHashCode()) + (ShowVanity.GetHashCode() ^ HideAmmoFlag.GetHashCode());
        }

        public static bool operator ==(ItemDescription a, ItemDescription b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(ItemDescription a, ItemDescription b)
        {
            return !a.Equals(b);
        }
    }
}
