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
        /// <summary>
        /// Gets or sets the wall which this item places upon use.
        /// </summary>
        /// <remarks>Item.createWall</remarks>
        public WallRef CreateWall
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

        /// <summary>
        /// Gets or sets the item's flame texture...?
        /// </summary>
        public Func<Texture2D> GetFlameTexture
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

        public ItemDef(ObjectName displayName, Func<ItemBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null)
            : base(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;
            GetFlameTexture = Empty<Texture2D>.Func;

            ArmourData    = new ItemArmourData(null, null, null, null);
            AccessoryData = new ItemAccessoryData(null, null, null, null, null, null, null, null, null, null, null); // this was extremely boring to write

            Width = Height = 16;
            MaxStack = 1;

            Scale = 1f;

            ExtractinatorMode = ItemExtractinatorMode.None;

            Colour = Color.White;

            // null when filling vanilla items
            //UseSound = new SfxRef("UseItem", variant: 1);
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
