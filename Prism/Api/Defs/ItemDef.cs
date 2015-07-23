using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Mods;
using Prism.Mods.Defs;

namespace Prism.API.Defs
{
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
        /// Gets or sets the the amount of time, in frames, which you must wait in order to reuse this item.
        /// </summary>
        /// <remarks>Item.resuseDelay</remarks>
        public virtual int ReuseDelay
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
        /// Gets or sets the amount of health restored when the item is used.
        /// </summary>
        public virtual int LifeHeal
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of mana restored when the item is used.
        /// </summary>
        public virtual int ManaHeal
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
        public virtual float Knockback
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
        /// Gets or sets whether the item is channeled, instead of used once or continuously used over and over again.
        /// </summary>
        public virtual bool IsChanneled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the item behaves as a soul.
        /// </summary>
        public virtual bool IsSoul
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item can be traded as a strange plant.
        /// </summary>
        public virtual bool IsStrangePlant
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item is a bullet kind of ammo.
        /// </summary>
        public virtual bool IsBullet
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item pulses.
        /// </summary>
        public virtual bool Pulses
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item is affected by gravity.
        /// </summary>
        public virtual bool NoGravity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether... idk, really.
        /// </summary>
        public virtual bool IsNebulaPickup
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item can blink or not.
        /// </summary>
        public virtual bool NeverShiny
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the extractinator can do something with the item.
        /// </summary>
        public virtual int ExtractinatorMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of minion slots required to use the item.
        /// </summary>
        public virtual int RequiredStaffMinionSlots
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color to which the item's sprite is tinted (<see cref="Color.White"/> = no tinting applied).
        /// </summary>
        /// <remarks>Item.color</remarks>
        public virtual Color Colour
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
        public virtual CoinValue Value
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
        public virtual ItemArmourData ArmourData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the buff this item grants to the player.
        /// </summary>
        public virtual BuffDef Buff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ammo item the item consumes when used.
        /// </summary>
        public virtual ItemRef UsedAmmo
        {
            get;
            set;
        }
        //TODO: use references for these... later
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

        /// <summary>
        /// Gets or sets the item's texture.
        /// </summary>
        public virtual Func<Texture2D> GetTexture
        {
            get;
            set;
        }

        public ItemDef(
            #region arguments
            string displayName,

            int damage = 0,
            int useAnimation = 0,
            int useTime = 0,
            int reuseDelay = 0,
            int mana = 0,
            int width = 16,
            int height = 16,
            int maxStack = 1,
            int placeStyle = 0,
            int alpha = 0,
            int defense = 0,
            int crit = 4,
            int pick = 0,
            int axe = 0,
            int hammer = 0,
            int healLife = 0,
            int healMana = 0,

            float shootSpeed = 0f,
            float knockback = 0f,
            float scale = 1f,

            bool noMelee = false,
            bool consumable = false,
            bool useTurn = false,
            bool autoReuse = false,
            bool noUseGraphic = false,
            bool accessory = false,
            bool expertOnly = false,
            bool channel = false,

            bool soul = false,
            bool strangePlant = false,
            bool bullet = false,
            bool pulses = false,
            bool noGravity = false,
            bool nebulaPickup = false,
            bool neverShiny = false,

            int extractinatorMode = 0,
            int staffMinionSlotsRequired = 0,

            Color colour = default(Color),
            ItemRarity rare = ItemRarity.White,
            ItemUseStyle useStyle = ItemUseStyle.None,
            ItemHoldStyle holdStyle = ItemHoldStyle.Default,
            ItemDamageType damageType = ItemDamageType.None,

            CoinValue value = default(CoinValue),
            ItemDescription descr = default(ItemDescription),
            ItemArmourData armour = default(ItemArmourData),
            BuffDef buff = default(BuffDef),

            ItemRef useAmmo = null,
            int shoot = 0,
            int ammo = 0,
            int useSound = 1,
            int createTile = -1,
            int createWall = -1,

            Func<Texture2D> getTex = null
            #endregion
            )
        {
            DisplayName = displayName;

            Damage = damage;
            UseAnimation = useAnimation;
            UseTime = useTime;
            ReuseDelay = reuseDelay;
            ManaConsumption = mana;
            Width = width;
            Height = height;
            MaxStack = maxStack;
            PlacementStyle = placeStyle;
            Alpha = alpha;
            Defense = defense;
            CritChanceModifier = crit;
            PickaxePower = pick;
            AxePower = axe;
            HammerPower = hammer;
            ManaHeal = healMana;
            LifeHeal = healLife;

            ShootVelocity = shootSpeed;
            Knockback = knockback;
            Scale = scale;

            NoMelee = noMelee;
            IsConsumable = consumable;
            TurnPlayerOnUse = useTurn;
            AutoReuse = autoReuse;
            HideUseGraphic = noUseGraphic;
            IsAccessory = accessory;
            IsExpertModeOnly = expertOnly;
            IsChanneled = channel;

            IsSoul = soul;
            IsStrangePlant = strangePlant;
            ExtractinatorMode = extractinatorMode;
            IsBullet = bullet;
            Pulses = pulses;
            NoGravity = noGravity;
            IsNebulaPickup = nebulaPickup;
            NeverShiny = neverShiny;
            RequiredStaffMinionSlots = staffMinionSlotsRequired;

            Colour = colour;
            Rarity = rare;
            UseStyle = useStyle;
            HoldStyle = holdStyle;
            DamageType = damageType;

            Value = value;
            Description = descr;
            ArmourData = armour;
            Buff = buff;

            UsedAmmo = useAmmo;
            ShootProjectile = shoot;
            AmmoType = ammo;
            UseSound = useSound;
            CreateTile = createTile;
            CreateWall = createWall;

            GetTexture = getTex ?? (() => null);
        }
    }
}
