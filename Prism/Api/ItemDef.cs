using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.Defs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Prism.API
{
    public enum ItemDamageType
    {
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

    // TODO: equality & tostring stuff
    /// <summary>
    /// Utility for Item.value stuff.
    /// </summary>
    public struct ItemValue
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

        public ItemValue(int c, int s, int g, int p)
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
    }
    /// <summary>
    /// Container for the properties of the item which relate to player buffs.
    /// I've chosen more friendly/less-vanilla-y names for the fields because
    /// Prism should be the last modding API we *ever* need for Terraria and
    /// I want it to be really nice and user-friendly.
    /// </summary>
    public struct ItemBuff
    {
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
    }
    /// <summary>
    /// Container for the properties of the item which determine its use as armor.
    /// </summary>
    public struct ItemArmorType
    {
        /// <summary>
        /// Gets or sets the "Head" equipment type index.
        /// </summary>
        /// <remarks>Item.headSlot</remarks>
        public ItemRef HeadType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the "Body" equipment type index.
        /// </summary>
        /// <remarks>Item.bodySlot</remarks>
        public ItemRef BodyType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the "Legs" equipment type index.
        /// </summary>
        /// <remarks>Item.legSlot</remarks>
        public ItemRef LegType
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a new <see cref="ItemArmorType"/> structure.
        /// </summary>
        /// <param name="head"><see cref="ItemArmorType.HeadType"/></param>
        /// <param name="body"><see cref="ItemArmorType.BodyType"/></param>
        /// <param name="legs"<see cref="ItemArmorType.LegType"/>></param>
        public ItemArmorType(ItemRef head, ItemRef body, ItemRef legs)
        {
            HeadType = head;
            BodyType = body;
            LegType = legs;
        }
    }
    /// <summary>
    /// Container for the properties of the item which relate to its description.
    /// </summary>
    public struct ItemDescription
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
    }

    public class ItemDef : EntityDef
    {
        /// <summary>
        /// Gets ItemDefs by their type number.
        /// </summary>
        public struct ByTypeGetter
        {
            public ItemDef this[int type]
            {
                get
                {
                    return ItemDefHandler.DefFromType[type];
                }
            }
        }
        /// <summary>
        /// Gets ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public struct ByNameGetter
        {
            public ItemDef this[string itemInternalName, string modInternalName = null]
            {
                get
                {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == VanillaString || modInternalName == TerrariaString)
                        return ItemDefHandler.VanillaDefFromName[itemInternalName];

                    return ModData.ModsFromInternalName[modInternalName].ItemDefs[itemInternalName];
                }
            }
        }

        /// <summary>
        /// Gets ItemDefs by their type number.
        /// </summary>
        public static ByTypeGetter ByType
        {
            get
            {
                return new ByTypeGetter();
            }
        }
        /// <summary>
        /// Gets ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public static ByNameGetter ByName
        {
            get
            {
                return new ByNameGetter();
            }
        }

        // stupid red and his stupid netids
        int setNetID = 0;
        /// <summary>
        /// Gets this item's NetID.
        /// </summary>
        public int NetID
        {
            get
            {
                return setNetID == 0 ? Type : setNetID;
            }
            internal set
            {
                setNetID = value;
            }
        }

        /// <summary>
        /// Gets or sets the damage this item inflicts.
        /// </summary>
        /// <remarks>Item.damage</remarks>
        public virtual int Damage
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the use animation of this item.
        /// </summary>
        /// <remarks>Item.useAnimation</remarks>
        public virtual int UseAnimation
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the use time of this item (the amount of time, in frames, that it takes to use this item).
        /// </summary>
        /// <remarks>Item.useTime</remarks>
        public virtual int UseTime
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of mana this item consumes upon use.
        /// </summary>
        /// <remarks>Item.mana</remarks>
        public virtual int ManaConsumption
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the width of this item once it is freed as drop in the game world.
        /// </summary>
        /// <remarks>Item.width</remarks>
        public virtual int Width
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the height of this item once it is freed as drop in the game world.
        /// </summary>
        /// <remarks>Item.height</remarks>
        public virtual int Height
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's max stack.
        /// </summary>
        /// <remarks>Item.maxStack</remarks>
        public virtual int MaxStack
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the style in which this item places tiles.
        /// </summary>
        /// <remarks>Item.placeStyle</remarks>
        public virtual int PlacementStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the opacity at which the item's sprite is rendered (0 = fully opaque, 255 = fully transparent).
        /// </summary>
        /// <remarks>Item.alpha</remarks>
        public virtual int Alpha
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the defense which this item grants to the player while equipped.
        /// </summary>
        /// <remarks>Item.defense</remarks>
        public virtual int Defense
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's critical strike chance modifier (in percent).
        /// </summary>
        /// <remarks>Item.crit</remarks>
        public virtual int CritChanceModifier
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's pickaxe power (in percent).
        /// </summary>
        public virtual int PickaxePower
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's axe power (in percent). <!-- needs modification when loading -->
        /// </summary>
        public virtual int AxePower
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's hammer power (in percent).
        /// </summary>
        public virtual int HammerPower
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the velocity at which this item shoots projectiles (in pixels / game tick).
        /// </summary>
        /// <remarks>Item.shootSpeed</remarks>
        public virtual float ShootVelocity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of knockback this item inflicts.
        /// </summary>
        /// <remarks>Item.knockBack</remarks>
        public virtual float KnockBack
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the scale at which the item's sprite is rendered (1.0f = normal scale).
        /// </summary>
        /// <remarks>Item.scale</remarks>
        public virtual float Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this item should ignore melee contact/damage with enemies.
        /// </summary>
        /// <remarks>Item.noMelee</remarks>
        public virtual bool NoMelee
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item is consumed upon use.
        /// </summary>
        /// <remarks>Item.consumable</remarks>
        public virtual bool IsConsumable
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item turns the player toward its direction upon use.
        /// </summary>
        /// <remarks>Item.useTurn</remarks>
        public virtual bool TurnPlayerOnUse
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item automatically reuses itself while the mouse button is held down.
        /// </summary>
        /// <remarks>Item.autoReuse</remarks>
        public virtual bool AutoReuse
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item's use graphic is hidden.
        /// </summary>
        /// <remarks>Item.noUseGraphic</remarks>
        public virtual bool HideUseGraphic
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item can be equipped in players' accessory slots.
        /// </summary>
        /// <remarks>Item.accessory</remarks>
        public virtual bool IsAccessory
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item can only be used in Expert Mode.
        /// </summary>
        public virtual bool IsExpertModeOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color to which the item's sprite is tinted (<see cref="Color.White"/> = no tinting applied).
        /// </summary>
        /// <remarks>Item.color</remarks>
        public virtual Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the base rarity of this item.
        /// </summary>
        /// <remarks>Item.rare, Item.questItem, Item.expert</remarks>
        public virtual ItemRarity Rarity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the use style of this item.
        /// </summary>
        /// <remarks>Item.useStyle</remarks>
        public virtual ItemUseStyle UseStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the style in which the player holds this item.
        /// </summary>
        /// <remarks>Item.holdStyle</remarks>
        public virtual ItemHoldStyle HoldStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of damage this item does.
        /// </summary>
        /// <remarks>Item.melee, Item.ranged, Item.magic, Item.thrown, Item.</remarks>
        public virtual ItemDamageType DamageType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets this item's value in coins (PPGGSSCC).
        /// </summary>
        /// <remarks>Item.value</remarks>
        public virtual int Value
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's description structure.
        /// </summary>
        public virtual ItemDescription Description
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's armor type structure.
        /// </summary>
        public virtual ItemArmorType ArmorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the buff this item grants to the player.
        /// </summary>
        public virtual ItemBuff Buff
        {
            get;
            set;
        }

        // use references for these... later
        /// <summary>
        /// Gets or sets the projectile which this item shoots upon use.
        /// </summary>
        /// <remarks>Item.shoot</remarks>
        public virtual int ShootProjectile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of ammo this item acts as.
        /// </summary>
        /// <remarks>Item.ammo</remarks>
        public virtual int AmmoType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the use sound effect of this item.
        /// </summary>
        /// <remarks>Item.useSound</remarks>
        public virtual int UseSound
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the tile which this item places upon use.
        /// </summary>
        /// <remarks>Item.createTile</remarks>
        public virtual int CreateTile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the wall which this item places upon use.
        /// </summary>
        /// <remarks>Item.createWall</remarks>
        public virtual int CreateWall
        {
            get;
            set;
        }
    }
}
