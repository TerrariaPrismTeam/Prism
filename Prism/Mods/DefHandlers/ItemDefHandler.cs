using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.Behaviours;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    sealed class ItemDefHandler : TEntityDefHandler<ItemDef, ItemBehaviour, Item>
    {
        protected override Type IDContainerType
        {
            get
            {
                return typeof(ItemID);
            }
        }

        internal static void OnSetDefaults(Item item, int type, bool noMatCheck)
        {
            ItemBHandler h = null; // will be set to <non-null> only if a behaviour handler will be attached

            item.RealSetDefaults(0, noMatCheck);

            if (Handler.ItemDef.DefsByType.ContainsKey(type))
            {
                var d = Handler.ItemDef.DefsByType[type];

                item.type = item.netID = type;
                item.width = item.height = 16;
                item.stack = item.maxStack = 1;

                Handler.ItemDef.CopyDefToEntity(d, item);

                if (d.CreateBehaviour != null)
                {
                    h = new ItemBHandler();

                    var b = d.CreateBehaviour();

                    if (b != null)
                        h.behaviours.Add(b);
                }
            }
            else
                item.RealSetDefaults(type, noMatCheck);

            var bs = ModData.mods.Values.Select(m => m.CreateGlobalItemBInternally()).Where(b => b != null);

            if (!bs.IsEmpty() && h == null)
                h = new ItemBHandler();

            if (h != null)
            {
                h.behaviours.AddRange(bs);

                h.Create();
                item.BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = item;

                h.OnInit();
            }
        }

        protected override void ExtendVanillaArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.itemAnimations.Length + amt : ItemID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.itemTexture, newLen);

            Array.Resize(ref Main.itemAnimations  , newLen);
            Array.Resize(ref Main.itemFlameLoaded , newLen);
            Array.Resize(ref Main.itemFlameTexture, newLen);
            Array.Resize(ref Main.itemFrame       , newLen);
            Array.Resize(ref Main.itemFrameCounter, newLen);
            Array.Resize(ref Main.itemName        , newLen);
            Array.Resize(ref Item.bodyType        , newLen);
            Array.Resize(ref Item.claw            , newLen);
            Array.Resize(ref Item.headType        , newLen);
            Array.Resize(ref Item.legType         , newLen);
            Array.Resize(ref Item.staff           , newLen);

            Array.Resize(ref ItemID.Sets.AnimatesAsSoul          , newLen);
            Array.Resize(ref ItemID.Sets.Deprecated              , newLen);
            Array.Resize(ref ItemID.Sets.ExoticPlantsForDyeTrade , newLen);
            Array.Resize(ref ItemID.Sets.ExtractinatorMode       , newLen);
            Array.Resize(ref ItemID.Sets.gunProj                 , newLen);
            Array.Resize(ref ItemID.Sets.ItemIconPulse           , newLen);
            Array.Resize(ref ItemID.Sets.ItemNoGravity           , newLen);
            Array.Resize(ref ItemID.Sets.NebulaPickup            , newLen);
            Array.Resize(ref ItemID.Sets.NeverShiny              , newLen);
            Array.Resize(ref ItemID.Sets.StaffMinionSlotsRequired, newLen);
        }

        protected override Item GetVanillaEntityFromID(int id)
        {
            Item i = new Item();
            i.netDefaults(id);
            return i;
        }
        protected override ItemDef NewDefFromVanilla(Item item)
        {
            return new ItemDef(Lang.itemName(item.netID, true), getTexture: () => Main.itemTexture[item.type]);
        }

        protected override void CopyEntityToDef(Item item, ItemDef def)
        {
            def.DisplayName         = Main.itemName[item.type];
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
            def.Description         = new ItemDescription(item.toolTip, item.toolTip2, item.vanity, item.expert, item.questItem, item.notAmmo);
            def.Buff                = new AppliedBuff(item.buffType, item.buffTime);

            def.UsedAmmo            = item.useAmmo    ==  0 ? null : new ItemRef      (item.useAmmo   );
            def.ShootProjectile     = item.shoot      ==  0 ? null : new ProjectileRef(item.shoot     );
            def.AmmoType            = item.ammo       ==  0 ? null : new ItemRef      (item.ammo      );
            def.UseSound            = item.useSound;
            def.CreateTile          = item.createTile == -1 ? null : new TileRef      (item.createTile);
            def.CreateWall          = item.createWall;
            def.GetTexture          = () => Main.itemTexture[item.type];

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
                femaleBodyId = item.bodySlot,
                headId = item.headSlot,
                legsId = item.legSlot,
                maleBodyId = item.bodySlot
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
                backId     = item.backSlot   ,
                balloonId  = item.balloonSlot,
                faceId     = item.faceSlot   ,
                frontId    = item.frontSlot  ,
                handsOffId = item.handOffSlot,
                handsOnId  = item.handOnSlot ,
                neckId     = item.neckSlot   ,
                shieldId   = item.shieldSlot ,
                shoesId    = item.shoeSlot   ,
                waistId    = item.waistSlot  ,
                wingsId    = item.wingSlot
            };
            #endregion

            def.Dye = item.dye;
            def.HairDye = item.hairDye;
            def.MountType = item.mountType;
            def.FishingPole = item.fishingPole;

            def.IsSoul                   = ItemID.Sets.AnimatesAsSoul           [def.Type];
            def.IsStrangePlant           = ItemID.Sets.ExoticPlantsForDyeTrade  [def.Type];
            def.IsBullet                 = ItemID.Sets.gunProj                  [def.Type];
            def.Pulses                   = ItemID.Sets.ItemIconPulse            [def.Type];
            def.NoGravity                = ItemID.Sets.ItemNoGravity            [def.Type];
            def.IsNebulaPickup           = ItemID.Sets.NebulaPickup             [def.Type];
            def.NeverShiny               = ItemID.Sets.NeverShiny               [def.Type];
            def.ExtractinatorMode        = ItemID.Sets.ExtractinatorMode        [def.Type];
            def.RequiredStaffMinionSlots = ItemID.Sets.StaffMinionSlotsRequired [def.Type];
        }
        protected override void CopyDefToEntity(ItemDef def, Item item)
        {
            item.name         = def.DisplayName;
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
            item.vanity       = def.Description.ShowVanity;
            item.notAmmo      = def.Description.HideAmmoFlag;
            item.toolTip      = def.Description.Description;
            item.toolTip2     = def.Description.ExtraDescription;
            item.questItem    = def.Description.ShowQuestItem;
            item.expert       = def.Description.ShowExpert;
            item.buffTime     = def.Buff.Duration;
            item.buffType     = def.Buff.Type;
            item.shoot        = def.ShootProjectile == null ?  0 : def.ShootProjectile.Resolve().Type ;
            item.ammo         = def.AmmoType        == null ?  0 : def.AmmoType       .Resolve().NetID;
            item.useSound     = def.UseSound;
            item.createTile   = def.CreateTile      == null ? -1 : def.CreateTile     .Resolve().Type ;
            item.createWall   = def.CreateWall;
            item.useAmmo      = def.UsedAmmo        == null ?  0 : def.UsedAmmo       .Resolve().Type ;

            item.headSlot = def.ArmourData.headId;
            item.bodySlot = def.ArmourData.maleBodyId;
            item.legSlot  = def.ArmourData.legsId;

            item.backSlot    = (sbyte)def.AccessoryData.backId    ;
            item.balloonSlot = (sbyte)def.AccessoryData.balloonId ;
            item.faceSlot    = (sbyte)def.AccessoryData.faceId    ;
            item.frontSlot   = (sbyte)def.AccessoryData.frontId   ;
            item.handOffSlot = (sbyte)def.AccessoryData.handsOffId;
            item.handOnSlot  = (sbyte)def.AccessoryData.handsOnId ;
            item.neckSlot    = (sbyte)def.AccessoryData.neckId    ;
            item.shieldSlot  = (sbyte)def.AccessoryData.shieldId  ;
            item.shoeSlot    = (sbyte)def.AccessoryData.shoesId   ;
            item.waistSlot   = (sbyte)def.AccessoryData.waistId   ;
            item.wingSlot    = (sbyte)def.AccessoryData.wingsId   ;

            item.dye = (byte)def.Dye;
            item.hairDye = (short)def.HairDye;
            item.mountType = def.MountType;
            item.fishingPole = def.FishingPole;
        }

        static int CheckAndPush<T>(Func<T> getter, ref T[] array, ref bool[] loadedArray)
        {
            if (getter == null)
                return -1;
            var t = getter();
            if (t == null)
                return -1;

            var ret = array.Length;

            Array.Resize(ref       array,       array.Length + 1);
            Array.Resize(ref loadedArray, loadedArray.Length + 1);

            loadedArray[ret] = true;
                  array[ret] = t   ;

            return ret;
        }

        static List<LoaderError> LoadArmourTextures(ItemDef def)
        {
            var ret = new List<LoaderError>();

            var ad = def.ArmourData;

            Texture2D t;

            if (ad.MaleBodyArmour != null)
            {
                t = ad.MaleBodyArmour();
                if (t != null)
                {
                    int id = Main.armorBodyTexture.Length;
                    Array.Resize(ref Main.armorBodyTexture, Main.armorBodyTexture.Length + 1);
                    Array.Resize(ref Main.armorBodyLoaded, Main.armorBodyLoaded.Length + 1);
                    Main.armorBodyLoaded[id] = true;
                    Main.armorBodyTexture[id] = t;
                    ad.maleBodyId = id;

                    t = (ad.FemaleBodyArmour ?? ad.MaleBodyArmour)();
                    if (t == null) // will not execute if MaleBodyArmour returned null, that's handled already
                        ret.Add(new LoaderError(def.Mod, "ArmourData.FemaleBodyArmour return value is null for ItemDef " + def + "."));
                    else
                    {
                        id = Main.femaleBodyTexture.Length;
                        if (Main.femaleBodyTexture.Length <= id)
                            Array.Resize(ref Main.femaleBodyTexture, id + 1);
                        Main.femaleBodyTexture[id] = t;
                        ad.femaleBodyId = id;
                    }
                }
            }

            ad.legsId = CheckAndPush(ad.Greaves, ref Main.armorLegTexture , ref Main.armorLegsLoaded);
            ad.headId = CheckAndPush(ad.Helmet , ref Main.armorHeadTexture, ref Main.armorHeadLoaded);

            return ret;
        }
        static void LoadAccessoryTextures(ItemDef def)
        {
            var ad = def.AccessoryData;

            ad.backId     = CheckAndPush(ad.Back    , ref Main.accBackTexture    , ref Main.accBackLoaded    );
            ad.balloonId  = CheckAndPush(ad.Balloon , ref Main.accBalloonTexture , ref Main.accballoonLoaded );
            ad.faceId     = CheckAndPush(ad.Face    , ref Main.accFaceTexture    , ref Main.accFaceLoaded    );
            ad.frontId    = CheckAndPush(ad.Front   , ref Main.accFrontTexture   , ref Main.accFrontLoaded   );
            ad.handsOffId = CheckAndPush(ad.HandsOff, ref Main.accHandsOffTexture, ref Main.accHandsOffLoaded);
            ad.handsOnId  = CheckAndPush(ad.HandsOn , ref Main.accHandsOnTexture , ref Main.accHandsOnLoaded );
            ad.neckId     = CheckAndPush(ad.Neck    , ref Main.accNeckTexture    , ref Main.accNeckLoaded    );
            ad.shieldId   = CheckAndPush(ad.Shield  , ref Main.accShieldTexture  , ref Main.accShieldLoaded  );
            ad.shoesId    = CheckAndPush(ad.Shoes   , ref Main.accShoesTexture   , ref Main.accShoesLoaded   );
            ad.waistId    = CheckAndPush(ad.Waist   , ref Main.accWaistTexture   , ref Main.accWaistLoaded   );
            ad.wingsId    = CheckAndPush(ad.Wings   , ref Main.   wingsTexture   , ref Main.   wingsLoaded   );
        }

        protected override List<LoaderError> CheckTextures(ItemDef def)
        {
            var ret = new List<LoaderError>();

            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of ItemDef " + def + " is null."));

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
            Main.itemName[def.Type] = def.DisplayName;

            ItemID.Sets.AnimatesAsSoul          [def.Type] = def.IsSoul                  ;
            ItemID.Sets.ExoticPlantsForDyeTrade [def.Type] = def.IsStrangePlant          ;
            ItemID.Sets.gunProj                 [def.Type] = def.IsBullet                ;
            ItemID.Sets.ItemIconPulse           [def.Type] = def.Pulses                  ;
            ItemID.Sets.ItemNoGravity           [def.Type] = def.NoGravity               ;
            ItemID.Sets.NebulaPickup            [def.Type] = def.IsNebulaPickup          ;
            ItemID.Sets.NeverShiny              [def.Type] = def.NeverShiny              ;
            ItemID.Sets.ExtractinatorMode       [def.Type] = def.ExtractinatorMode       ;
            ItemID.Sets.StaffMinionSlotsRequired[def.Type] = def.RequiredStaffMinionSlots;
        }
    }
}
