using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Util;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Prism.Mods.DefHandlers
{
    sealed partial class ItemDefHandler : EEntityDefHandler<ItemDef, ItemBehaviour, Item>
    {
        readonly static LegacySoundStyle defLss = new LegacySoundStyle(SoundID.Item, -1);

        internal static int UnknownItemID = 0;

        protected override Type IDContainerType
        {
            get
            {
                return typeof(ItemID);
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            if (amt == 0)
                return;

            int newLen = amt > 0 ? Main.itemAnimations.Length + amt : ItemID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.itemTexture, newLen);

            Array.Resize(ref Main.itemAnimations  , newLen);
            Array.Resize(ref Main.itemFlameLoaded , newLen);
            Array.Resize(ref Main.itemFlameTexture, newLen);
            Array.Resize(ref Main.itemFrame       , newLen);
            Array.Resize(ref Main.itemFrameCounter, newLen);
            Array.Resize(ref Main.itemFlameLoaded , newLen);
            Array.Resize(ref Main.itemFlameTexture, newLen);

            Array.Resize(ref Lang._itemNameCache   , newLen);
            Array.Resize(ref Lang._itemTooltipCache, newLen);

            Array.Resize(ref Item.bodyType        , newLen);
            Array.Resize(ref Item.claw            , newLen);
            Array.Resize(ref Item.headType        , newLen);
            Array.Resize(ref Item.legType         , newLen);
            Array.Resize(ref Item.staff           , newLen);
            Array.Resize(ref Item.itemCaches      , newLen);

            Array.Resize(ref ItemID.Sets.AnimatesAsSoul             , newLen);
            Array.Resize(ref ItemID.Sets.Deprecated                 , newLen);
            Array.Resize(ref ItemID.Sets.ExoticPlantsForDyeTrade    , newLen);
            Array.Resize(ref ItemID.Sets.ExtractinatorMode          , newLen);
            Array.Resize(ref ItemID.Sets.gunProj                    , newLen);
            Array.Resize(ref ItemID.Sets.ItemIconPulse              , newLen);
            Array.Resize(ref ItemID.Sets.ItemNoGravity              , newLen);
            Array.Resize(ref ItemID.Sets.NebulaPickup               , newLen);
            Array.Resize(ref ItemID.Sets.NeverShiny                 , newLen);
            Array.Resize(ref ItemID.Sets.StaffMinionSlotsRequired   , newLen);
            Array.Resize(ref ItemID.Sets.TextureCopyLoad            , newLen);

            //Below items need to be added to ItemDef's fields:
            Array.Resize(ref ItemID.Sets.TrapSigned                 , newLen);
            Array.Resize(ref ItemID.Sets.SortingPriorityBossSpawns  , newLen);
            Array.Resize(ref ItemID.Sets.SortingPriorityExtractibles, newLen);
            Array.Resize(ref ItemID.Sets.SortingPriorityMaterials   , newLen);
            Array.Resize(ref ItemID.Sets.SortingPriorityPainting    , newLen);
            Array.Resize(ref ItemID.Sets.SortingPriorityRopes       , newLen);
            Array.Resize(ref ItemID.Sets.SortingPriorityTerraforming, newLen);
            Array.Resize(ref ItemID.Sets.SortingPriorityWiring      , newLen);
            Array.Resize(ref ItemID.Sets.GamepadExtraRange          , newLen);
            Array.Resize(ref ItemID.Sets.GamepadSmartQuickReach     , newLen);
            Array.Resize(ref ItemID.Sets.GamepadWholeScreenUseRange , newLen);
            Array.Resize(ref ItemID.Sets.SingleUseInGamepad         , newLen);
            Array.Resize(ref ItemID.Sets.Yoyo                       , newLen);
            Array.Resize(ref ItemID.Sets.AlsoABuildingItem          , newLen);
            Array.Resize(ref ItemID.Sets.LockOnIgnoresCollision     , newLen);
            Array.Resize(ref ItemID.Sets.LockOnAimAbove             , newLen);
            Array.Resize(ref ItemID.Sets.LockOnAimCompensation      , newLen);
        }

        protected override Item GetVanillaEntityFromID(int id)
        {
            var i = new Item();
            i.netDefaults(id);
            return i;
        }

        protected override ItemDef NewDefFromVanilla(Item item)
        {
            return new ItemDef(new ObjectName(Lang.GetItemNameValue(item.netID)),
                    getTexture: () => Main.itemTexture[item.type]);
        }

        static Func<ItemRef, bool> DRMatch(ItemDef d)
        {
            return r => r.Resolve().NetID == d.NetID;
        }

        protected override void CopyEntityToDef(Item item, ItemDef def)
        {
            def.DisplayName         = new ObjectName(item.Name);
            def.Type                = item.type;
            def.NetID               = item.netID;
            def.Damage              = item.damage;
            def.UseAnimation        = item.useAnimation;
            def.UseTime             = item.useTime;
            def.ReuseDelay          = item.reuseDelay;
            def.ManaConsumption     = item.mana;
            def.Width               = item.width;
            def.Height              = item.height;
            def.MaxStack            = item.maxStack;
            def.PlacementStyle      = item.placeStyle;
            def.Alpha               = item.alpha;
            def.Defense             = item.defense;
            def.CritChanceModifier  = item.crit;
            def.PickaxePower        = item.pick;
            def.AxePower            = item.axe * 5;
            def.HammerPower         = item.hammer;
            def.LifeHeal            = item.healLife;
            def.ManaHeal            = item.healMana;
            def.BaitPower           = item.bait;
            def.ShootVelocity       = item.shootSpeed;
            def.Knockback           = item.knockBack;
            def.Scale               = item.scale;
            def.NoMelee             = item.noMelee;
            def.IsConsumable        = item.consumable;
            def.TurnPlayerOnUse     = item.useTurn;
            def.AutoReuse           = item.autoReuse;
            def.HideUseGraphic      = item.noUseGraphic;
            def.IsAccessory         = item.accessory;
            def.IsExpertModeOnly    = item.expertOnly;
            def.IsChanneled         = item.channel;
            def.Colour              = item.color;
            def.Rarity              = (ItemRarity)item.rare;
            def.UseStyle            = (ItemUseStyle)item.useStyle;
            def.HoldStyle           = (ItemHoldStyle)item.holdStyle;
            def.DamageType          = item.melee  ? ItemDamageType.Melee
                                    : item.ranged ? ItemDamageType.Ranged
                                    : item.magic  ? ItemDamageType.Magic
                                    : item.summon ? ItemDamageType.Summon
                                    : item.thrown ? ItemDamageType.Thrown
                                         : ItemDamageType.None;
            def.Value               = new CoinValue(item.value);
            def.Description         = new ItemDescription(
                                        Lang._itemTooltipCache[item.type].ToLines(),
                                        item.vanity, item.expert, item.questItem, item.notAmmo, !item.material);
            def.Buff                = new AppliedBuff(new BuffRef(item.buffType), item.buffTime);

            def.UsedAmmo            = item.useAmmo    ==  0 ? null : new ItemRef      (item.useAmmo   );
            def.ShootProjectile     = item.shoot      <=  0 ? null : new ProjectileRef(item.shoot     );
            def.AmmoType            = item.ammo       ==  0 ? null : new ItemRef      (item.ammo      );
            def.CreateTile          = item.createTile <= -1 ? null : new TileRef      (item.createTile);
            def.CreateWall          = item.createWall <=  0 ? null : new WallRef      (item.createWall);
            def.GetTexture          = () => Main.itemTexture[item.type];
            def.GetFlameTexture     = () => Main.itemFlameTexture[item.type];
            def.UseSound            = item.P_UseSound as SfxRef ?? (item.UseSound == null ? null : new SfxRef("UseItem", variant: item.UseSound.Style));

            #region ArmourData
            def.ArmourData = new ItemArmourData(() =>
            {
                if (item.headSlot == -1)
                    return null;

                Main.instance.LoadArmorHead(item.headSlot);
                return Main.armorHeadTexture[item.headSlot];
            }, () =>
            {
                if (item.bodySlot == -1)
                    return null;

                Main.instance.LoadArmorBody(item.bodySlot);
                return Main.armorBodyTexture[item.bodySlot];
            }, () =>
            {
                if (item.bodySlot == -1)
                    return null;

                Main.instance.LoadArmorBody(item.bodySlot);
                return Main.armorArmTexture[item.bodySlot];
            }, () =>
            {
                if (item.legSlot == -1)
                    return null;

                Main.instance.LoadArmorLegs(item.legSlot);
                return Main.armorLegTexture[item.legSlot];
            }, () =>
            {
                if (item.bodySlot == -1)
                    return null;

                Main.instance.LoadArmorBody(item.bodySlot);
                return Main.femaleBodyTexture[item.bodySlot];
            })
            {
                HeadId     = item.headSlot,
                MaleBodyId = item.bodySlot,
                LegsId     = item.legSlot
            };
            #endregion
            #region AccessoryData
            def.AccessoryData = new ItemAccessoryData(() =>
            {
                if (item.backSlot == -1)
                    return null;

                Main.instance.LoadAccBack(item.backSlot);
                return Main.accBackTexture[item.backSlot];
            }, () =>
            {
                if (item.balloonSlot == -1)
                    return null;

                Main.instance.LoadAccBalloon(item.balloonSlot);
                return Main.accBalloonTexture[item.balloonSlot];
            }, () =>
            {
                if (item.faceSlot == -1)
                    return null;

                Main.instance.LoadAccFace(item.faceSlot);
                return Main.accFaceTexture[item.faceSlot];
            }, () =>
            {
                if (item.frontSlot == -1)
                    return null;

                Main.instance.LoadAccFront(item.frontSlot);
                return Main.accFrontTexture[item.frontSlot];
            }, () =>
            {
                if (item.handOffSlot == -1)
                    return null;

                Main.instance.LoadAccHandsOff(item.handOffSlot);
                return Main.accHandsOffTexture[item.handOffSlot];
            }, () =>
            {
                if (item.handOnSlot == -1)
                    return null;

                Main.instance.LoadAccHandsOn(item.handOnSlot);
                return Main.accHandsOnTexture[item.handOnSlot];
            }, () =>
            {
                if (item.neckSlot == -1)
                    return null;

                Main.instance.LoadAccNeck(item.neckSlot);
                return Main.accNeckTexture[item.neckSlot];
            }, () =>
            {
                if (item.shieldSlot == -1)
                    return null;

                Main.instance.LoadAccShield(item.shieldSlot);
                return Main.accShieldTexture[item.shieldSlot];
            }, () =>
            {
                if (item.shoeSlot == -1)
                    return null;

                Main.instance.LoadAccShoes(item.shoeSlot);
                return Main.accShoesTexture[item.shoeSlot];
            }, () =>
            {
                if (item.waistSlot == -1)
                    return null;

                Main.instance.LoadAccWaist(item.type);
                return Main.accWaistTexture[item.type];
            }, () =>
            {
                if (item.wingSlot == -1)
                    return null;

                Main.instance.LoadWings(item.type);
                return Main.wingsTexture[item.type];
            })
            {
                BackId     = item.backSlot   ,
                BalloonId  = item.balloonSlot,
                FaceId     = item.faceSlot   ,
                FrontId    = item.frontSlot  ,
                HandsOffId = item.handOffSlot,
                HandsOnId  = item.handOnSlot ,
                NeckId     = item.neckSlot   ,
                ShieldId   = item.shieldSlot ,
                ShoesId    = item.shoeSlot   ,
                WaistId    = item.waistSlot  ,
                WingsId    = item.wingSlot
            };
            #endregion

            def.Dye = item.dye;
            def.HairDye = item.hairDye;
            def.Mount = item.mountType <= -1 ? null : new MountRef(item.mountType);
            def.FishingPole = item.fishingPole;

            def.material = item.material;

            def.Description = new ItemDescription(Lang._itemTooltipCache[item.type].ToLines(),
                                item.vanity, item.expert, item.questItem, item.notAmmo, !item.material);

            def.IsSoul                   = ItemID.Sets.AnimatesAsSoul           [def.Type];
            def.IsStrangePlant           = ItemID.Sets.ExoticPlantsForDyeTrade  [def.Type];
            def.IsBullet                 = ItemID.Sets.gunProj                  [def.Type];
            def.Pulses                   = ItemID.Sets.ItemIconPulse            [def.Type];
            def.NoGravity                = ItemID.Sets.ItemNoGravity            [def.Type];
            def.IsNebulaPickup           = ItemID.Sets.NebulaPickup             [def.Type];
            def.NeverShiny               = ItemID.Sets.NeverShiny               [def.Type];
            def.RequiredStaffMinionSlots = ItemID.Sets.StaffMinionSlotsRequired [def.Type];

            def.ExtractinatorMode = (ItemExtractinatorMode)ItemID.Sets.ExtractinatorMode[def.Type];
        }
        protected override void CopyDefToEntity(ItemDef def, Item item)
        {
            if (!def.material.HasValue)
            {
                var m = DRMatch(def);

                def.material = RecipeDef.Recipes.Any(r => r.RequiredItems.Keys.Any(ir => ir.Match(m, g => g.Any(m))));
            }

            item._nameOverride = def.DisplayName.ToString();

            item.type         = def.Type;
            item.netID        = def.NetID;
            item.damage       = def.Damage;
            item.useAnimation = def.UseAnimation;
            item.useTime      = def.UseTime;
            item.reuseDelay   = def.ReuseDelay;
            item.mana         = def.ManaConsumption;
            item.width        = def.Width;
            item.height       = def.Height;
            item.maxStack     = def.MaxStack;
            item.placeStyle   = def.PlacementStyle;
            item.alpha        = def.Alpha;
            item.defense      = def.Defense;
            item.crit         = def.CritChanceModifier;
            item.pick         = def.PickaxePower;
            item.axe          = def.AxePower / 5; // fucking red
            item.hammer       = def.HammerPower;
            item.healLife     = def.LifeHeal;
            item.healMana     = def.ManaHeal;
            item.bait         = def.BaitPower;
            item.shootSpeed   = def.ShootVelocity;
            item.knockBack    = def.Knockback;
            item.scale        = def.Scale;
            item.noMelee      = def.NoMelee;
            item.consumable   = def.IsConsumable;
            item.useTurn      = def.TurnPlayerOnUse;
            item.autoReuse    = def.AutoReuse;
            item.noUseGraphic = def.HideUseGraphic;
            item.accessory    = def.IsAccessory;
            item.expertOnly   = def.IsExpertModeOnly;
            item.channel      = def.IsChanneled;
            item.color        = def.Colour;
            item.useStyle     = (int)def.UseStyle;
            item.holdStyle    = (int)def.HoldStyle;
            item.rare         = (int)def.Rarity;
            item.useStyle     = (int)def.UseStyle;
            item.holdStyle    = (int)def.HoldStyle;
            item.melee        = def.DamageType == ItemDamageType.Melee;
            item.ranged       = def.DamageType == ItemDamageType.Ranged;
            item.magic        = def.DamageType == ItemDamageType.Magic;
            item.summon       = def.DamageType == ItemDamageType.Summon;
            item.thrown       = def.DamageType == ItemDamageType.Thrown;
            item.value        = def.Value.Value;
            item.buffTime     = def.Buff.Duration;
            item.buffType     = def.Buff.Type       == null ?  0 : def.Buff.Type      .Resolve().Type ;
            item.shoot        = def.ShootProjectile == null ?  0 : def.ShootProjectile.Resolve().Type ;
            item.ammo         = def.AmmoType        == null ?  0 : def.AmmoType       .Resolve().NetID;
            item.createTile   = def.CreateTile      == null ? -1 : def.CreateTile     .Resolve().Type ;
            item.createWall   = def.CreateWall      == null ? -1 : def.CreateWall     .Resolve().Type ;
            item.useAmmo      = def.UsedAmmo        == null ?  0 : def.UsedAmmo       .Resolve().Type ;

            item.P_UseSound = def.UseSound;
            item.UseSound   = def.UseSound == null ? defLss : new LegacySoundStyle(SoundID.Item, def.UseSound.VariantID);

            if (def.ArmourData != null)
            {
                item.headSlot = def.ArmourData.HeadId;
                item.bodySlot = def.ArmourData.MaleBodyId;
                item.legSlot  = def.ArmourData.LegsId;
            }
            else
                item.headSlot = item.bodySlot = item.legSlot = -1;

            if (def.AccessoryData != null)
            {
                item.backSlot    = (sbyte)def.AccessoryData.BackId    ;
                item.balloonSlot = (sbyte)def.AccessoryData.BalloonId ;
                item.faceSlot    = (sbyte)def.AccessoryData.FaceId    ;
                item.frontSlot   = (sbyte)def.AccessoryData.FrontId   ;
                item.handOffSlot = (sbyte)def.AccessoryData.HandsOffId;
                item.handOnSlot  = (sbyte)def.AccessoryData.HandsOnId ;
                item.neckSlot    = (sbyte)def.AccessoryData.NeckId    ;
                item.shieldSlot  = (sbyte)def.AccessoryData.ShieldId  ;
                item.shoeSlot    = (sbyte)def.AccessoryData.ShoesId   ;
                item.waistSlot   = (sbyte)def.AccessoryData.WaistId   ;
                item.wingSlot    = (sbyte)def.AccessoryData.WingsId   ;
            }
            else
                item.backSlot = item.balloonSlot = item.faceSlot = item.handOffSlot = item.handOnSlot = item.neckSlot = item.shieldSlot = item.shoeSlot = item.waistSlot = item.wingSlot;

            item.vanity    =  def.Description.ShowVanity      ;
            item.questItem =  def.Description.ShowQuestItem   ;
            item.expert    =  def.Description.ShowExpert      ;
            item.notAmmo   =  def.Description.HideAmmoFlag    ;
            item.material  = !def.Description.HideMaterialFlag && (def.material ?? false);

            item.dye = (byte)def.Dye;
            item.hairDye = (short)def.HairDye;
            item.mountType = def.Mount == null ? -1 : def.Mount.Resolve().Type;
            item.fishingPole = def.FishingPole;

            item.potion = def.LifeHeal > 0;
        }

        static int CheckAndPush<T>(Func<T> getter, ref T[] array, ref bool[] loadedArray)
            where T : class
        {
            if (getter == null)
                return -1;
            var t = getter();
            if (ReferenceEquals(t, null))
                return -1;

            var ret = array.Length;

            Array.Resize(ref       array,       array.Length + 1);
            Array.Resize(ref loadedArray, loadedArray.Length + 1);

            loadedArray[ret] = true;
                  array[ret] = t   ;

            return ret;
        }

        static IEnumerable<LoaderError> LoadArmourTextures(ItemDef def)
        {
            var ad = def.ArmourData;

            if (ad == null)
                return Empty<LoaderError>.Array;

            var ret = new List<LoaderError>();

            Texture2D t;

            if (ad.MaleBodyArmour != null)
            {
                t = ad.MaleBodyArmour();
                if (t != null)
                {
                    int id = Main.armorBodyTexture.Length;
                    Array.Resize(ref Main.armorBodyTexture, Main.armorBodyTexture.Length + 1);
                    Array.Resize(ref Main.armorBodyLoaded , Main.armorBodyLoaded.Length  + 1);
                    Main.armorBodyLoaded[id] = true;
                    Main.armorBodyTexture[id] = t;
                    ad.MaleBodyId = id;

                    t = ad.FemaleBodyArmour() ?? t;
                    if (t == null) // will not execute if MaleBodyArmour returned null, that's handled already
                        ret.Add(new LoaderError(def.Mod, "ArmourData.FemaleBodyArmour return value is null for ItemDef " + def + "."));
                    else
                    {
                        id = Main.femaleBodyTexture.Length;
                        if (Main.femaleBodyTexture.Length <= id)
                            Array.Resize(ref Main.femaleBodyTexture, id + 1);
                        Main.femaleBodyTexture[id] = t;
                    }

                    t = ad.Arm();

                    if (t == null)
                        ret.Add(new LoaderError(def.Mod, "ArmourData.Arm return value is null for ItemDef " + def + "."));

                    id = Main.armorArmTexture.Length;
                    Array.Resize(ref Main.armorArmTexture, Main.armorArmTexture.Length + 1);
                    Main.armorArmTexture[id] = ad.Arm();
                }
            }
            ad.LegsId = CheckAndPush(ad.Greaves, ref Main.armorLegTexture , ref Main.armorLegsLoaded);
            ad.HeadId = CheckAndPush(ad.Helmet , ref Main.armorHeadTexture, ref Main.armorHeadLoaded);

            return ret;
        }
        static void LoadAccessoryTextures(ItemDef def)
        {
            var ad = def.AccessoryData;

            if (ad == null)
                return;

            ad.BackId     = CheckAndPush(ad.Back    , ref Main.accBackTexture    , ref Main.accBackLoaded    );
            ad.BalloonId  = CheckAndPush(ad.Balloon , ref Main.accBalloonTexture , ref Main.accballoonLoaded );
            ad.FaceId     = CheckAndPush(ad.Face    , ref Main.accFaceTexture    , ref Main.accFaceLoaded    );
            ad.FrontId    = CheckAndPush(ad.Front   , ref Main.accFrontTexture   , ref Main.accFrontLoaded   );
            ad.HandsOffId = CheckAndPush(ad.HandsOff, ref Main.accHandsOffTexture, ref Main.accHandsOffLoaded);
            ad.HandsOnId  = CheckAndPush(ad.HandsOn , ref Main.accHandsOnTexture , ref Main.accHandsOnLoaded );
            ad.NeckId     = CheckAndPush(ad.Neck    , ref Main.accNeckTexture    , ref Main.accNeckLoaded    );
            ad.ShieldId   = CheckAndPush(ad.Shield  , ref Main.accShieldTexture  , ref Main.accShieldLoaded  );
            ad.ShoesId    = CheckAndPush(ad.Shoes   , ref Main.accShoesTexture   , ref Main.accShoesLoaded   );
            ad.WaistId    = CheckAndPush(ad.Waist   , ref Main.accWaistTexture   , ref Main.accWaistLoaded   );
            ad.WingsId    = CheckAndPush(ad.Wings   , ref Main.   wingsTexture   , ref Main.   wingsLoaded   );
        }

        protected override List<LoaderError> CheckTextures(ItemDef def)
        {
            var ret = new List<LoaderError>();

            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of ItemDef " + def + " is null."));

            if (def.ArmourData != null && def.ArmourData.MaleBodyArmour != null && def.ArmourData.Arm == null)
                ret.Add(new LoaderError(def.Mod, "ArmourData.Arm of ItemDef " + def + " is null when ArmourData.MaleBodyArmour isn't."));

            return ret;
        }
        protected override List<LoaderError> LoadTextures (ItemDef def)
        {
            var ret = new List<LoaderError>();

            // def.GetTexture itself should be checked when it's loaded
            var t = def.GetTexture();
            if (t == null)
            {
                ret.Add(new LoaderError(def.Mod, "GetTexture return value is null for ItemDef " + def + "."));
                return ret;
            }

            Main.itemTexture[def.Type] = t;

            ret.AddRange(LoadArmourTextures(def));
            LoadAccessoryTextures(def);

            return ret;
        }

        protected override int GetRegularType(Item item)
        {
            return item.type;
        }

        protected override void CopySetProperties(ItemDef def)
        {
            Lang._itemNameCache   [def.Type] = (LocalizedText)def.DisplayName;
            Lang._itemTooltipCache[def.Type] = def.Description.Description.ToTooltip();

            ItemID.Sets.AnimatesAsSoul          [def.Type] = def.IsSoul                  ;
            ItemID.Sets.ExoticPlantsForDyeTrade [def.Type] = def.IsStrangePlant          ;
            ItemID.Sets.gunProj                 [def.Type] = def.IsBullet                ;
            ItemID.Sets.ItemIconPulse           [def.Type] = def.Pulses                  ;
            ItemID.Sets.ItemNoGravity           [def.Type] = def.NoGravity               ;
            ItemID.Sets.NebulaPickup            [def.Type] = def.IsNebulaPickup          ;
            ItemID.Sets.NeverShiny              [def.Type] = def.NeverShiny              ;
            ItemID.Sets.StaffMinionSlotsRequired[def.Type] = def.RequiredStaffMinionSlots;

            ItemID.Sets.ExtractinatorMode[def.Type] = (int)def.ExtractinatorMode;
        }

        protected override void PostFillVanilla()
        {
            ExtendVanillaArrays(1);

            var ui = UnknownItem.Create();

            Load(new Dictionary<string, ItemDef>
            {
                { "_UnloadedItem", ui }
            });

            UnknownItemID = ui.Type;
        }
    }
}
