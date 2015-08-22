using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Audio;
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
        public int Damage
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the length of this item's use animation.
        /// </summary>
        /// <remarks>Item.useAnimation</remarks>
        public int UseAnimation
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the use time of this item (the amount of time, in frames, that it takes to use this item).
        /// </summary>
        /// <remarks>Item.useTime</remarks>
        public int UseTime
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the the amount of time, in frames, which you must wait in order to reuse this item.
        /// </summary>
        /// <remarks>Item.resuseDelay</remarks>
        public int ReuseDelay
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of mana this item consumes upon use.
        /// </summary>
        /// <remarks>Item.mana</remarks>
        public int ManaConsumption
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's max stack.
        /// </summary>
        /// <remarks>Item.maxStack</remarks>
        public int MaxStack
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the style in which this item places tiles.
        /// </summary>
        /// <remarks>Item.placeStyle</remarks>
        public int PlacementStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the opacity at which the item's sprite is rendered (0 = fully opaque, 255 = fully transparent).
        /// </summary>
        /// <remarks>Item.alpha</remarks>
        public int Alpha
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the defense which this item grants to the player while equipped.
        /// </summary>
        /// <remarks>Item.defense</remarks>
        public int Defense
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's critical strike chance modifier (in percent).
        /// </summary>
        /// <remarks>Item.crit</remarks>
        public int CritChanceModifier
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's pickaxe power (in percent).
        /// </summary>
        public int PickaxePower
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's axe power (in percent). <!-- needs modification when loading -->
        /// </summary>
        public int AxePower
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's hammer power (in percent).
        /// </summary>
        public int HammerPower
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of health restored when the item is used.
        /// </summary>
        public int LifeHeal
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of mana restored when the item is used.
        /// </summary>
        public int ManaHeal
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the bait power the item has.
        /// </summary>
        public int BaitPower
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the velocity at which this item shoots projectiles (in pixels / game tick).
        /// </summary>
        /// <remarks>Item.shootSpeed</remarks>
        public float ShootVelocity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of knockback this item inflicts.
        /// </summary>
        /// <remarks>Item.knockBack</remarks>
        public float Knockback
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the scale at which the item's sprite is rendered (1.0f = normal scale).
        /// </summary>
        /// <remarks>Item.scale</remarks>
        public float Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this item should ignore melee contact/damage with enemies.
        /// </summary>
        /// <remarks>Item.noMelee</remarks>
        public bool NoMelee
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item is consumed upon use.
        /// </summary>
        /// <remarks>Item.consumable</remarks>
        public bool IsConsumable
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item turns the player toward its direction upon use.
        /// </summary>
        /// <remarks>Item.useTurn</remarks>
        public bool TurnPlayerOnUse
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item automatically reuses itself while the mouse button is held down.
        /// </summary>
        /// <remarks>Item.autoReuse</remarks>
        public bool AutoReuse
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item's use graphic is hidden.
        /// </summary>
        /// <remarks>Item.noUseGraphic</remarks>
        public bool HideUseGraphic
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item can be equipped in players' accessory slots.
        /// </summary>
        /// <remarks>Item.accessory</remarks>
        public bool IsAccessory
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this item can only be used in Expert Mode.
        /// </summary>
        public bool IsExpertModeOnly
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item is channeled, instead of used once or continuously used over and over again.
        /// </summary>
        public bool IsChanneled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the item behaves as a soul.
        /// </summary>
        public bool IsSoul
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item can be traded as a strange plant.
        /// </summary>
        public bool IsStrangePlant
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item is a bullet kind of ammo.
        /// </summary>
        public bool IsBullet
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item pulses.
        /// </summary>
        public bool Pulses
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item is affected by gravity.
        /// </summary>
        public bool NoGravity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether... idk, really.
        /// </summary>
        public bool IsNebulaPickup
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the item can shine or not.
        /// </summary>
        public bool NeverShiny
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets what the extractinator can use this item to produce.
        /// </summary>
        public ItemExtractinatorMode ExtractinatorMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the amount of minion slots required to use the item.
        /// </summary>
        public int RequiredStaffMinionSlots
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color to which the item's sprite is tinted (<see cref="Color.White"/> = no tinting applied).
        /// </summary>
        /// <remarks>Item.color</remarks>
        public Color Colour
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the base rarity of this item.
        /// </summary>
        /// <remarks>Item.rare, Item.questItem, Item.expert</remarks>
        public ItemRarity Rarity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the use style of this item.
        /// </summary>
        /// <remarks>Item.useStyle</remarks>
        public ItemUseStyle UseStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the style in which the player holds this item.
        /// </summary>
        /// <remarks>Item.holdStyle</remarks>
        public ItemHoldStyle HoldStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of damage this item does.
        /// </summary>
        /// <remarks>Item.melee, Item.ranged, Item.magic, Item.thrown, Item.</remarks>
        public ItemDamageType DamageType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's value in coins (PPGGSSCC).
        /// </summary>
        /// <remarks>Item.value</remarks>
        public CoinValue Value
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's description structure.
        /// </summary>
        public ItemDescription Description
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets this item's armor type structure.
        /// </summary>
        public ItemArmourData ArmourData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the buff this item grants to the player.
        /// </summary>
        public AppliedBuff Buff
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the item's accessory texture stuff.
        /// </summary>
        public ItemAccessoryData AccessoryData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ammo item the item consumes when used.
        /// </summary>
        public ItemRef UsedAmmo
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the projectile which this item shoots upon use.
        /// </summary>
        /// <remarks>Item.shoot</remarks>
        public ProjectileRef ShootProjectile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of ammo this item acts as.
        /// </summary>
        /// <remarks>Item.ammo</remarks>
        public ItemRef AmmoType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the use sound effect of this item.
        /// </summary>
        /// <remarks>Item.useSound</remarks>
        public SfxRef UseSound
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the tile which this item places upon use.
        /// </summary>
        /// <remarks>Item.createTile</remarks>
        public TileRef CreateTile
        {
            get;
            set;
        }
        //TODO: use a reference for this... later
        /// <summary>
        /// Gets or sets the wall which this item places upon use.
        /// </summary>
        /// <remarks>Item.createWall</remarks>
        public int CreateWall
        {
            get;
            set;
        }
        public MountRef Mount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item's texture.
        /// </summary>
        public Func<Texture2D> GetTexture
        {
            get;
            set;
        }

        public string SetName
        {
            get;
            set;
        }

        //TODO: add custom things for these... later
        public int Dye
        {
            get;
            set;
        }
        public int HairDye
        {
            get;
            set;
        }
        public int FishingPole
        {
            get;
            set;
        }

        public ItemDef(string displayName, Func<ItemBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null)
            : base(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;

            ArmourData    = new ItemArmourData(null, null, null, null);
            AccessoryData = new ItemAccessoryData(null, null, null, null, null, null, null, null, null, null, null); // this was extremely boring to write

            Width = Height = 16;
            MaxStack = 1;

            Scale = 1f;

            ExtractinatorMode = ItemExtractinatorMode.None;

            Colour = Color.White;

            CreateWall = -1;

            // null when filling vanilla items
            UseSound = new SfxRef("UseItem", variant: 1);
        }

        [Obsolete("JSON files aren't supported for now, please use the other constructor and/or a custom deserializer.")]
        public ItemDef(string displayName, JsonData json,
            Func<Texture2D> getTexture = null,
            ItemArmourData armour = default(ItemArmourData),
            ItemAccessoryData accessoryStuff = default(ItemAccessoryData),
            Func<ItemBehaviour> newBehaviour = null)
            : this(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;
            ArmourData = armour;
            AccessoryData = accessoryStuff;

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
            ExtractinatorMode = json.Has("extractinatorMode") ? json["extractinatorMode"].ParseAsEnum<ItemExtractinatorMode>() : ItemExtractinatorMode.None;
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
                Value = value.IsArray ? new CoinValue((int)value[0], (int)value[1], (int)value[2], (int)value[3]) : (CoinValue)(int)value;
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
                Buff = new AppliedBuff(type.ParseBuffRef(), (int)buff["duration"]);
            }

            if (json.Has("useAmmo"))
                UsedAmmo = json["useAmmo"].ParseItemRef();
            if (json.Has("shoot"))
                ShootProjectile = json["shoot"].ParseProjectileRef();
            if (json.Has("ammo"))
                AmmoType = json["ammo"].ParseItemRef();
            if (json.Has("createTile"))
                CreateTile = json["createTile"].ParseTileRef();

            UseSound = VanillaSfxes.NpcHit[json.Has("useSound") ? (int)json["useSound"] : 1];
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
