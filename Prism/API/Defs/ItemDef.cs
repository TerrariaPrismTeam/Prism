using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Util;
using Terraria;

namespace Prism.API.Defs
{
    public partial class ItemDef : EntityDef<ItemBehaviour, Item>
    {
        internal bool? material;

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
        /// Gets or sets the length of this item's use animation.
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
        //TODO: move this to EntityDef?
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
        /// Gets or sets the bait power the item has.
        /// </summary>
        public virtual int BaitPower
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
        /// Gets or sets whether the item can shine or not.
        /// </summary>
        public virtual bool NeverShiny
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets what the extractinator can use this item to produce.
        /// <remarks>TODO: Make an enum for the types?</remarks>
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
        public virtual AppliedBuff Buff
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the item's accessory texture stuff.
        /// </summary>
        public virtual ItemAccessoryData AccessoryData
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
        public virtual ProjectileRef ShootProjectile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of ammo this item acts as.
        /// </summary>
        /// <remarks>Item.ammo</remarks>
        public virtual ItemRef AmmoType
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
        public virtual TileRef CreateTile
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

        //TODO: add custom things for these... later
        public virtual int Dye
        {
            get;
            set;
        }
        public virtual int HairDye
        {
            get;
            set;
        }
        public virtual int MountType
        {
            get;
            set;
        }
        public virtual int FishingPole
        {
            get;
            set;
        }

        public ItemDef(string displayName, Func<ItemBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null)
            : base(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;

            ArmourData = new ItemArmourData(null, null, null);

            Width = Height = 16;
            MaxStack = 1;

            Scale = 1f;

            ExtractinatorMode = MountType = -1;

            Colour = Color.White;

            CreateWall = -1;
        }

        public ItemDef(string displayName, JsonData json,
            Func<Texture2D> getTexture = null,
            ItemArmourData armour = default(ItemArmourData), //TODO: support this in JSON
            ItemAccessoryData accessoryStuff = default(ItemAccessoryData), //TODO: this, too
            Func<ItemBehaviour> newBehaviour = null)
            : this(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;
            ArmourData = armour;
            AccessoryData = accessoryStuff;

            //TODO: check if the fields exist
            //TODO: use error handling, exceptions shouldn't be thrown from a constructor
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

            if (json.Has("rare"))
                Rarity = json["rare"].ParseAsEnum<ItemRarity>();
            if (json.Has("useStyle"))
                UseStyle = json["useStyle"].ParseAsEnum<ItemUseStyle>();
            if (json.Has("holdStyle"))
                HoldStyle = json["holdStyle"].ParseAsEnum<ItemHoldStyle>();
            if (json.Has("damageType"))
                DamageType = json["damageType"].ParseAsEnum<ItemDamageType>();

            if (json.Has("value"))
            {
                JsonData value = json["value"];
                if (value.IsArray)
                    Value = new CoinValue((int)value[0], (int)value[1], (int)value[2], (int)value[3]);
                else
                    Value = (CoinValue)(int)value;
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
                //TODO: Add string compatibility with BuffRef
                Buff = new AppliedBuff((int)type, (int)buff["duration"]);
            }

            if (json.Has("useAmmo"))
                UsedAmmo = json["useAmmo"].ParseItemRef();
            if (json.Has("shoot"))
                ShootProjectile = json["shoot"].ParseProjectileRef();
            if (json.Has("ammo"))
                AmmoType = json["ammo"].ParseItemRef();
            if (json.Has("createTile"))
                CreateTile = json["createTile"].ParseTileRef();

            UseSound = json.Has("useSound") ? (int)json["useSound"] : 1;
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
