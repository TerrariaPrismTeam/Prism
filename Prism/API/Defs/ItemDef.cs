using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;

namespace Prism.API.Defs
{
    public class ItemDef : EntityDef<ItemBehaviour, Item>
    {
        /// <summary>
        /// Gets ItemDefs by their type number.
        /// </summary>
        public struct ByTypeIndexer
        {
            public ItemDef this[int type]
            {
                get
                {
                    return Handler.ItemDef.DefsByType[type];
                }
            }
        }
        /// <summary>
        /// Gets ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public struct ByNameIndexer
        {
            public ItemDef this[string itemInternalName, string modInternalName = null]
            {
                get
                {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == PrismApi.VanillaString || modInternalName == PrismApi.TerrariaString)
                        return Handler.ItemDef.VanillaDefsByName[itemInternalName];

                    return ModData.ModsFromInternalName[modInternalName].ItemDefs[itemInternalName];
                }
            }
        }

        /// <summary>
        /// Gets ItemDefs by their type number.
        /// </summary>
        public static ByTypeIndexer ByType
        {
            get
            {
                return new ByTypeIndexer();
            }
        }
        /// <summary>
        /// Gets ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public static ByNameIndexer ByName
        {
            get
            {
                return new ByNameIndexer();
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
            Func<Texture2D> getTex = null,
            Func<ItemBehaviour> newBehaviour = null,

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
            ItemDescription description = default(ItemDescription),
            ItemArmourData armour = default(ItemArmourData),
            BuffDef buff = default(BuffDef),

            ItemRef useAmmo = null,
            int shoot = 0,
            int ammo = 0,
            int useSound = 1,
            int createTile = -1,
            int createWall = -1
            #endregion
            )
        {
            DisplayName = displayName;

            GetTexture      = getTex       ?? Empty<Texture2D    >.Func;
            CreateBehaviour = newBehaviour ?? Empty<ItemBehaviour>.Func;

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
            Description = description;
            ArmourData = armour;
            Buff = buff;

            UsedAmmo = useAmmo;
            ShootProjectile = shoot;
            AmmoType = ammo;
            UseSound = useSound;
            CreateTile = createTile;
            CreateWall = createWall;
        }

        public ItemDef(string displayName, JsonData json,
            Func<Texture2D> getTex = null,
            ItemArmourData armour = default(ItemArmourData),
            Func<ItemBehaviour> newBehaviour = null)
        {
            DisplayName = displayName;
            GetTexture = getTex ?? Empty<Texture2D>.Func;
            ArmourData = armour;
            CreateBehaviour = newBehaviour ?? Empty<ItemBehaviour>.Func;

            Damage = (int)json["damage"];
            UseAnimation = (int)json["useAnimation"];
            UseTime = (int)json["useTime"];
            ReuseDelay = (int)json["reuseDelay"];
            ManaConsumption = (int)json["mana"];
            Width = json.Has("width") ? (int)json["width"] : 16;
            Height = json.Has("height") ? (int)json["height"] : 16;
            MaxStack = json.Has("maxStack") ? (int)json["maxStack"] : 1;
            PlacementStyle = (int)json["placeStyle"];
            Alpha = (int)json["alpha"];
            Defense = (int)json["defense"];
            CritChanceModifier = json.Has("crit") ? (int)json["crit"] : 4;
            PickaxePower = (int)json["pick"];
            AxePower = (int)json["axe"];
            HammerPower = (int)json["hammer"];
            ManaHeal = (int)json["healMana"];
            LifeHeal = (int)json["healLife"];

            ShootVelocity = (float)json["shootSpeed"];
            Knockback = (float)json["knockback"];
            Scale = json.Has("scale") ? (float)json["scale"] : 1f;

            NoMelee = (bool)json["noMelee"];
            IsConsumable = (bool)json["consumable"];
            TurnPlayerOnUse = (bool)json["useTurn"];
            AutoReuse = (bool)json["autoReuse"];
            HideUseGraphic = (bool)json["noUseGraphic"];
            IsAccessory = (bool)json["accessory"];
            IsExpertModeOnly = (bool)json["expertOnly"];
            IsChanneled = (bool)json["channel"];

            IsSoul = (bool)json["soul"];
            IsStrangePlant = (bool)json["strangePlant"];
            ExtractinatorMode = (int)json["extractinatorMode"];
            IsBullet = (bool)json["bullet"];
            Pulses = (bool)json["pulses"];
            NoGravity = (bool)json["noGravity"];
            IsNebulaPickup = (bool)json["nebulaPickup"];
            NeverShiny = (bool)json["neverShiny"];
            RequiredStaffMinionSlots = (int)json["staffMinionSlotsRequired"];

            if (json.Has("colour"))
            {
                JsonData colour = json["colour"];
                Colour = new Color((int)colour[0], (int)colour[1], (int)colour[2]);
            }
            else
            {
                Colour = default(Color);
            }

            if (json.Has("rare"))
            {
                JsonData rare = json["rare"];
                if (rare.IsString)
                {
                    Rarity = (ItemRarity)Enum.Parse(typeof(ItemRarity), (string)rare);
                }
                else
                {
                    Rarity = (ItemRarity)(int)rare;
                }
            }
            else
            {
                Rarity = default(ItemRarity);
            }

            if (json.Has("useStyle"))
            {
                JsonData useStyle = json["useStyle"];
                if (useStyle.IsString)
                {
                    UseStyle = (ItemUseStyle)Enum.Parse(typeof(ItemUseStyle), (string)useStyle);
                }
                else
                {
                    UseStyle = (ItemUseStyle)(int)useStyle;
                }
            }
            else
            {
                UseStyle = default(ItemUseStyle);
            }

            if (json.Has("holdStyle"))
            {
                JsonData holdStyle = json["holdStyle"];
                if (holdStyle.IsString)
                {
                    HoldStyle = (ItemHoldStyle)Enum.Parse(typeof(ItemHoldStyle), (string)holdStyle);
                }
                else
                {
                    HoldStyle = (ItemHoldStyle)(int)holdStyle;
                }
            }
            else
            {
                HoldStyle = default(ItemHoldStyle);
            }

            if (json.Has("damageType"))
            {
                JsonData damageType = json["damageType"];
                if (damageType.IsString)
                {
                    DamageType = (ItemDamageType)Enum.Parse(typeof(ItemDamageType), (string)damageType);
                }
                else
                {
                    DamageType = (ItemDamageType)(int)damageType;
                }
            }
            else
            {
                DamageType = default(ItemDamageType);
            }

            if (json.Has("value"))
            {
                JsonData value = json["value"];
                if (value.IsArray)
                {
                    Value = new CoinValue((int)value[0], (int)value[1], (int)value[2], (int)value[3]);
                }
                else
                {
                    Value = (CoinValue)(int)value;
                }
            }
            else
            {
                Value = default(CoinValue);
            }

            if (json.Has("description"))
            {
                JsonData description = json["description"];
                Description = new ItemDescription(
                    (string)description["tooltip1"],
                    (string)description["tooltip2"],
                    (bool)description["vanity"],
                    (bool)description["hideAmmo"]);
            }

            if (json.Has("buff"))
            {
                JsonData buff = json["buff"];
                JsonData type = buff["type"];
                // TODO Add string compatibility with BuffRef
                Buff = new BuffDef((int)type, (int)buff["duration"]);
            }
            else
            {
                Buff = default(BuffDef);
            }

            if (json.Has("useAmmo"))
            {
                JsonData useAmmo = json["useAmmo"];
                if (useAmmo.IsString)
                {
                    string[] s = ((string)useAmmo).Split('.');

                    if (s.Length < 2)
                    {
                        UsedAmmo = new ItemRef(s[0]);
                    }
                    else
                    {
                        UsedAmmo = new ItemRef(s[1], s[0]);
                    }
                }
                else
                {
                    UsedAmmo = new ItemRef((int)useAmmo);
                }
            }
            else
            {
                UsedAmmo = null;
            }

            if (json.Has("shoot"))
            {
                JsonData shoot = json["shoot"];
                // TODO Add string compatibility with ProjectileRef
                ShootProjectile = (int)shoot;
            }

            AmmoType = (int)json["ammo"];
            UseSound = json.Has("useSound") ? (int)json["useSound"] : 1;
            CreateTile = json.Has("createTile") ? (int)json["createTile"] : -1;
            CreateWall = json.Has("createWall") ? (int)json["createWall"] : -1;
        }

        public static implicit operator ItemRef(ItemDef  def)
        {
            return new ItemRef(def.InternalName, def.Mod.InternalName);
        }
        public static explicit operator ItemDef(ItemRef @ref)
        {
            return @ref.Resolve();
        }
    }
}
