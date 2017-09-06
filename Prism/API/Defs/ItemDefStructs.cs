using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.Util;

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
    public enum ItemExtractinatorMode
    {
        None = -1,
        SiltAndSlush = 0,
        Fossil = 1
    }

    /// <summary>
    /// Container for the properties of the item which determine its use as armour.
    /// </summary>
    public class ItemArmourData
    {
        /// <summary>
        /// Gets or sets whether the item can be used as a helmet.
        /// </summary>
        /// <remarks>Item.headSlot</remarks>
        public Func<Texture2D> Helmet;
        /// <summary>
        /// Gets or sets whether the item can be used as body armour and the male body armour texture.
        /// </summary>
        /// <remarks>Item.bodySlot</remarks>
        public Func<Texture2D> MaleBodyArmour;
        /// <summary>
        /// Gets or sets the female body armour texture.
        /// </summary>
        /// <remarks>Item.bodySlot</remarks>
        public Func<Texture2D> FemaleBodyArmour;
        /// <summary>
        /// Gets or sets the arm texture of the body armour.
        /// </summary>
        public Func<Texture2D> Arm;
        /// <summary>
        /// Gets or sets whether the item can be used as greaves.
        /// </summary>
        /// <remarks>Item.legSlot</remarks>
        public Func<Texture2D> Greaves;

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
        public int LegsId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Constructs a new <see cref="ItemArmourData"/> structure.
        /// </summary>
        /// <param name="head"><see cref="Helmet"/></param>
        /// <param name="body"><see cref="BodyArmour"/></param>
        /// <param name="legs"><see cref="Greaves"/></param>
        public ItemArmourData(Func<Texture2D> head, Func<Texture2D> maleBody, Func<Texture2D> arm, Func<Texture2D> legs, Func<Texture2D> femaleBody = null)
        {
            HeadId = MaleBodyId = LegsId = -1;

            Helmet = head;
            MaleBodyArmour = maleBody;
            Arm = arm;
            Greaves = legs;

            FemaleBodyArmour = femaleBody ?? maleBody;
        }
    }
    public class ItemAccessoryData
    {
        public Func<Texture2D>
            Back   , Balloon, Face  , Front, HandsOff      ,
            HandsOn, Neck   , Shield, Shoes, Waist   , Wings;

        public int BackId
        {
            get;
            internal set;
        }
        public int BalloonId
        {
            get;
            internal set;
        }
        public int FaceId
        {
            get;
            internal set;
        }
        public int FrontId
        {
            get;
            internal set;
        }
        public int HandsOffId
        {
            get;
            internal set;
        }
        public int HandsOnId
        {
            get;
            internal set;
        }
        public int NeckId
        {
            get;
            internal set;
        }
        public int ShieldId
        {
            get;
            internal set;
        }
        public int ShoesId
        {
            get;
            internal set;
        }
        public int WaistId
        {
            get;
            internal set;
        }
        public int WingsId
        {
            get;
            internal set;
        }

        public ItemAccessoryData(
            Func<Texture2D> back   , Func<Texture2D> balloon, Func<Texture2D> face  , Func<Texture2D> front, Func<Texture2D> handsOff,
            Func<Texture2D> handsOn, Func<Texture2D> neck   , Func<Texture2D> shield, Func<Texture2D> shoes, Func<Texture2D> waist   , Func<Texture2D> wings)
        {
            BackId = BalloonId = FaceId = FrontId = HandsOffId = HandsOnId = NeckId = ShieldId = ShoesId = WaistId = WingsId = -1;

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
        public readonly ObjectName[] Description;

        /// <summary>
        /// Gets or sets whether this item is labeled as "Vanity item" in its tool-tip.
        /// </summary>
        /// <remarks>Item.vanity</remarks>
        public readonly bool ShowVanity;
        /// <summary>
        /// Gets or sets whether this item is labeled with the "Expert" tag in its tool-tip.
        /// <para/>Note: Enabling this will also make the item display with a rainbow name, regardless of its rarity (it doesn't affect its existing rarity value).
        /// </summary>
        /// <remarks>Item.expert</remarks>
        public readonly bool ShowExpert;
        /// <summary>
        /// Gets or sets whether this item is labeled with the "Quest Item" tag in its tool-tip (does not effect rarity/text color).
        /// </summary>
        /// <remarks>Item.questItem</remarks>
        public readonly bool ShowQuestItem;

        /// <summary>
        /// Gets or sets whether this item is labeled as "Ammo" in its tool-tip.
        /// </summary>
        /// <remarks>!Item.notAmmo</remarks>
        public readonly bool HideAmmoFlag;
        public readonly bool HideMaterialFlag;

        /// <summary>
        /// Constructs a new <see cref="ItemDescription"/> structure.
        /// </summary>
        /// <param name="desc"><see cref="Description"/></param>
        /// <param name="extraDesc"><see cref="ExtraDescription"/></param>
        /// <param name="vanity"><see cref="ShowVanity"/></param>
        /// <param name="expert"><see cref="ShowExpert"/></param>
        /// <param name="quest"><see cref="ShowQuestItem"/></param>
        /// <param name="hideAmmo"><see cref="HideAmmoFlag"/></param>
        public ItemDescription(ObjectName[] desc, bool vanity = false, bool expert = false, bool quest = false, bool hideAmmo = false, bool hideMat = false)
        {
            Description      = desc      ?? Empty<ObjectName>.Array;
            ShowVanity       = vanity;
            ShowExpert       = expert;
            ShowQuestItem    = quest;
            HideAmmoFlag     = hideAmmo;
            HideMaterialFlag = hideMat;
        }

        public bool Equals(ItemDescription other)
        {
            return Description == other.Description && ShowVanity == other.ShowVanity && HideAmmoFlag == other.HideAmmoFlag && HideMaterialFlag == other.HideMaterialFlag;
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
            return Description.GetHashCode() + (ShowVanity.GetHashCode() ^ (ShowExpert.GetHashCode() << 8) ^ (ShowQuestItem.GetHashCode() << 24))
                + ((HideAmmoFlag.GetHashCode() << 30) ^ (HideMaterialFlag.GetHashCode() << 16));
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
