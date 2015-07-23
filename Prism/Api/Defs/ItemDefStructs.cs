using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Prism.API.Defs
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
        BreathingReed,
        Harp
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
    /// Container for the properties of the item which determine its use as armour.
    /// </summary>
    public struct ItemArmourData : IEquatable<ItemArmourData>
    {
        internal int headId, maleBodyId, femaleBodyId, legsId;

        public int HeadId
        {
            get
            {
                return headId;
            }
        }
        public int MaleBodyId
        {
            get
            {
                return maleBodyId;
            }
        }
        public int FemaleBodyId
        {
            get
            {
                return femaleBodyId;
            }
        }
        public int LegsId
        {
            get
            {
                return legsId;
            }
        }

        /// <summary>
        /// Gets or sets whether the item can be used as a helmet.
        /// </summary>
        /// <remarks>Item.headSlot</remarks>
        public Func<Texture2D> Helmet;
        /// <summary>
        /// Gets or sets whether the item can be used as body armour.
        /// </summary>
        /// <remarks>Item.bodySlot</remarks>
        public Func<Texture2D> MaleBodyArmour;
        /// <summary>
        /// Gets or sets whether the item can be used as body armour.
        /// </summary>
        /// <remarks>Item.bodySlot</remarks>
        public Func<Texture2D> FemaleBodyArmour;
        /// <summary>
        /// Gets or sets whether the item can be used as greaves.
        /// </summary>
        /// <remarks>Item.legSlot</remarks>
        public Func<Texture2D> Greaves;

        /// <summary>
        /// Constructs a new <see cref="ItemArmourData"/> structure.
        /// </summary>
        /// <param name="head"><see cref="Helmet"/></param>
        /// <param name="body"><see cref="BodyArmour"/></param>
        /// <param name="legs"><see cref="Greaves"/></param>
        public ItemArmourData(Func<Texture2D> head, Func<Texture2D> maleBody, Func<Texture2D> legs, Func<Texture2D> femaleBody = null)
        {
            headId = maleBodyId = femaleBodyId = legsId = -1;

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
        public string Description;
        /// <summary>
        /// Gets or sets the item's extra description (funny quote, reference, etc).
        /// </summary>
        /// <remarks>Item.toolTip2</remarks>
        public string ExtraDescription;

        /// <summary>
        /// Gets or sets whether this item is labeled as "Vanity item" in its tool-tip.
        /// </summary>
        /// <remarks>Item.vanity</remarks>
        public bool ShowVanity;
        /// <summary>
        /// Gets or sets whether this item is labeled as "Ammo" in its tool-tip.
        /// </summary>
        /// <remarks>!Item.notAmmo</remarks>
        public bool HideAmmoFlag;

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
