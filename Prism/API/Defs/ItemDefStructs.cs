using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Prism.API.Defs
{
    /// <summary>
    /// Damage types for items.
    /// </summary>
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
        /// Rarity of items only obtainable in Expert Mode.
        /// <para/>Note: Automatically flags this item as an expert item, adding the "Expert" label onto its tooltip.
        /// </summary>
        /// <remarks>Automatically enables Item.expert but Item.expert can be enabled without setting rainbow rarity.</remarks>
        Rainbow = -12,
        /// <summary>
        /// Rarity of quest items.
        /// Unlike <see cref="ItemRarity.Rainbow"/>, this does not automatically flag the item as a quest item.
        /// </summary>
        /// <remarks>Completely separate from Item.questItem</remarks>
        Amber = -11,
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
    public struct ItemArmourData
    {
        internal int headId, maleBodyId, femaleBodyId, legsId;

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
    }
    public struct ItemAccessoryData
    {
        internal int backId, balloonId, faceId, frontId, handsOffId, handsOnId, neckId, shieldId, shoesId, waistId, wingsId;

        public Func<Texture2D>
            Back   , Balloon, Face  , Front, HandsOff      ,
            HandsOn, Neck   , Shield, Shoes, Waist   , Wings;

        public int BackId
        {
            get
            {
                return backId;
            }
        }
        public int BalloonId
        {
            get
            {
                return balloonId;
            }
        }
        public int FaceId
        {
            get
            {
                return faceId;
            }
        }
        public int FrontId
        {
            get
            {
                return frontId;
            }
        }
        public int HandsOffId
        {
            get
            {
                return handsOffId;
            }
        }
        public int HandsOnId
        {
            get
            {
                return handsOnId;
            }
        }
        public int NeckID
        {
            get
            {
                return neckId;
            }
        }
        public int ShieldId
        {
            get
            {
                return shieldId;
            }
        }
        public int ShoesId
        {
            get
            {
                return shoesId;
            }
        }
        public int WaistId
        {
            get
            {
                return waistId;
            }
        }
        public int WingsId
        {
            get
            {
                return wingsId;
            }
        }

        public ItemAccessoryData(
            Func<Texture2D> back   , Func<Texture2D> balloon, Func<Texture2D> face  , Func<Texture2D> front, Func<Texture2D> handsOff,
            Func<Texture2D> handsOn, Func<Texture2D> neck   , Func<Texture2D> shield, Func<Texture2D> shoes, Func<Texture2D> waist   , Func<Texture2D> wings)
        {
            backId = balloonId = faceId = frontId = handsOffId = handsOnId = neckId = shieldId = shoesId = waistId = wingsId = -1;

            Back = back;
            Balloon = balloon;
            Face = face;
            Front = front;
            HandsOff = handsOff;
            HandsOn = handsOn;
            Neck = neck;
            Shield = shield;
            Shoes = shoes;
            Waist = waist;
            Wings = wings;
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
        /// Gets or sets whether this item is labeled with the "Expert" tag in its tool-tip.
        /// <para/>Note: Enabling this will also make the item display with a rainbow name, regardless of its rarity (it doesn't affect its existing rarity value).
        /// </summary>
        /// <remarks>Item.expert</remarks>
        public bool ShowExpert;
        /// <summary>
        /// Gets or sets whether this item is labeled with the "Quest Item" tag in its tool-tip (does not effect rarity/text color).
        /// </summary>
        /// <remarks>Item.questItem</remarks>
        public bool ShowQuestItem;

        /// <summary>
        /// Gets or sets whether this item is labeled as "Ammo" in its tool-tip.
        /// </summary>
        /// <remarks>!Item.notAmmo</remarks>
        public bool HideAmmoFlag;
        public bool HideMaterialFlag;

        /// <summary>
        /// Constructs a new <see cref="ItemDescription"/> structure.
        /// </summary>
        /// <param name="desc"><see cref="ItemDescription.Description"/></param>
        /// <param name="extraDesc"><see cref="ItemDescription.ExtraDescription"/></param>
        /// <param name="vanity"><see cref="ItemDescription.ShowVanity"/></param>
        /// <param name="expert"><see cref="ItemDescription.ShowExpert"/></param>
        /// <param name="quest"><see cref="ItemDescription.ShowQuestItem"/></param>
        /// <param name="hideAmmo"><see cref="ItemDescription.HideAmmoFlag"/></param>
        public ItemDescription(string desc, string extraDesc = null, bool vanity = false, bool expert = false, bool quest = false, bool hideAmmo = false, bool hideMat = false)
        {
            Description      = desc      ?? String.Empty;
            ExtraDescription = extraDesc ?? String.Empty;
            ShowVanity       = vanity;
            ShowExpert       = expert;
            ShowQuestItem    = quest;
            HideAmmoFlag     = hideAmmo;
            HideMaterialFlag = hideMat;
        }

        public bool Equals(ItemDescription other)
        {
            return Description == other.Description && ExtraDescription == other.ExtraDescription && ShowVanity == other.ShowVanity && HideAmmoFlag == other.HideAmmoFlag && HideMaterialFlag == other.HideMaterialFlag;
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
            return (Description.GetHashCode() & ExtraDescription.GetHashCode()) + (ShowVanity.GetHashCode() ^ ShowExpert.GetHashCode() ^ ShowQuestItem.GetHashCode()) + (HideAmmoFlag.GetHashCode() ^ HideMaterialFlag.GetHashCode());
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
