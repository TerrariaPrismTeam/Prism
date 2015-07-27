using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.Behaviours;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Groups
{


    /// <summary>
    /// Handles all the <see cref="ItemGroup"/>s.
    /// </summary>
    static class ItemGroupHandler
    {
        internal static Dictionary<string, ItemGroup> VanillaGroupFromName = new Dictionary<string, ItemGroup>();                

        /// <summary>
        /// Adds all the original vanilla item groups.
        /// </summary>
        internal static void FillVanilla()
        {
            for (int i = -24 /* some phasesabre */; i < ItemID.Count; i++)
            {
                //if (i > -19 /* phasesabres stop at -19 because Redigit */ && i <= 0)
                // copper etc items, using <=1.2-style netids instead of the new types (backwards compatibility needed for terraria code that still uses those netids)
                if (i == 0)
                    continue;

                Item it = new Item();
                it.netDefaults(i);

                ItemDef def = new ItemDef(Lang.itemName(it.type, true));

                def.InternalName = it.name;
                def.Type = it.type;
                def.NetID = i;

                CopyItemToDef(def, it);

                DefFromType.Add(i, def);
                // TODO Fix overlapping names
                //VanillaDefFromName.Add(it.name, def);
            }

            Recipes.AddVanillaRecipeReferences();
        }

        internal static void OnSetDefaults(Item i, int type, bool noMatCheck)
        {
            ItemBHandler h = null;

            if (type >= ItemID.Count)
            {
                i.RealSetDefaults(0, noMatCheck);

                if (DefFromType.ContainsKey(type))
                {
                    var d = DefFromType[type];

                    i.type = i.netID = type;
                    i.width = i.height = 16;
                    i.stack = i.maxStack = 1;

                    CopyDefToItem(i, d);

                    h = new ItemBHandler();
                    if (d.CreateBehaviour != null)
                    {
                        var b = d.CreateBehaviour();

                        if (b != null)
                            h.behaviours.Add(b);
                    }

                    i.BHandler = h;
                }
            }
            else
                i.RealSetDefaults(type, noMatCheck);

            //TODO: add global hooks here (and check for null)

            if (h != null)
            {
                h.Create();
                i.BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = i;

                h.OnInit();
            }
        }

        static void LoadTextures     (ItemDef def)
        {
            // def.GetTexture itself should be checked when it's loaded
            var t = def.GetTexture();
            if (t == null)
                throw new ArgumentNullException("GetTexture return value is null for ItemDef " + def.InternalName + " from mod " + def.Mod + ".");
            Main.itemTexture[def.Type] = def.GetTexture();

            var ad = def.ArmourData;

            if (ad.Helmet != null)
            {
                t = ad.Helmet();
                if (t == null)
                    throw new ArgumentNullException("ArmourData.Helmet return value is null for ItemDef " + def.InternalName + " from mod " + def.Mod + ".");

                int id = Main.armorHeadTexture.Length;
                Array.Resize(ref Main.armorHeadTexture, Main.armorHeadTexture.Length + 1);
                Array.Resize(ref Main.armorHeadLoaded, Main.armorHeadLoaded.Length + 1);
                Main.armorHeadLoaded[id] = true;
                Main.armorHeadTexture[id] = t;
                ad.headId = id;
            }
            if (ad.MaleBodyArmour != null)
            {
                t = ad.MaleBodyArmour();
                if (t == null)
                    throw new ArgumentNullException("ArmourData.MaleBodyArmour return value is null for ItemDef " + def.InternalName + " from mod " + def.Mod + ".");

                int id = Main.armorBodyTexture.Length;
                Array.Resize(ref Main.armorBodyTexture, Main.armorBodyTexture.Length + 1);
                Array.Resize(ref Main.armorBodyLoaded, Main.armorBodyLoaded.Length + 1);
                Main.armorBodyLoaded[id] = true;
                Main.armorBodyTexture[id] = t;
                ad.maleBodyId = id;

                t = ad.FemaleBodyArmour();
                if (t == null)
                    throw new ArgumentNullException("ArmourData.FemaleBodyArmour return value is null for ItemDef " + def.InternalName + " from mod " + def.Mod + ".");

                id = Main.femaleBodyTexture.Length;
                if (Main.femaleBodyTexture.Length <= id)
                    Array.Resize(ref Main.femaleBodyTexture, id + 1);
                Main.femaleBodyTexture[id] = t;
                ad.femaleBodyId = id;
            }
            if (ad.Greaves != null)
            {
                t = ad.Greaves();
                if (t == null)
                    throw new ArgumentNullException("ArmourData.Greaves return value is null for ItemDef " + def.InternalName + " from mod " + def.Mod + ".");

                int id = Main.armorLegTexture.Length;
                Array.Resize(ref Main.armorLegTexture, Main.armorLegTexture.Length + 1);
                Array.Resize(ref Main.armorLegsLoaded, Main.armorLegsLoaded.Length + 1);
                Main.armorLegsLoaded[id] = true;
                Main.armorLegTexture[id] = t;
                ad.legsId = id;
            }
        }
        static void LoadSetProperties(ItemDef def)
        {
            // assuming space is allocated in the ExtendArrays call
            Main.itemName[def.Type] = def.DisplayName;

            ItemID.Sets.AnimatesAsSoul         [def.Type] = def.IsSoul;
            ItemID.Sets.ExoticPlantsForDyeTrade[def.Type] = def.IsStrangePlant;
            ItemID.Sets.gunProj                [def.Type] = def.IsBullet;
            ItemID.Sets.ItemIconPulse          [def.Type] = def.Pulses;
            ItemID.Sets.ItemNoGravity          [def.Type] = def.NoGravity;
            ItemID.Sets.NebulaPickup           [def.Type] = def.IsNebulaPickup;
            ItemID.Sets.NeverShiny             [def.Type] = def.NeverShiny;

            ItemID.Sets.ExtractinatorMode       [def.Type] = def.ExtractinatorMode;
            ItemID.Sets.StaffMinionSlotsRequired[def.Type] = def.RequiredStaffMinionSlots;
        }

        // set properties aren't copied, type/netid is preserved
        static void CopyDefToItem(Item    tar, ItemDef source)
        {
            tar.damage = source.Damage;
            tar.useAnimation = source.UseAnimation;
            tar.useTime = source.UseTime;
            tar.reuseDelay = source.ReuseDelay;
            tar.mana = source.ManaConsumption;
            tar.width = source.Width;
            tar.height = source.Height;
            tar.maxStack = source.MaxStack;
            tar.placeStyle = source.PlacementStyle;
            tar.alpha = source.Alpha;
            tar.defense = source.Defense;
            tar.crit = source.CritChanceModifier;
            tar.pick = source.PickaxePower;
            tar.axe = source.AxePower / 5; // fucking red
            tar.hammer = source.HammerPower;
            tar.healLife = source.LifeHeal;
            tar.healMana = source.ManaHeal;

            tar.shootSpeed = source.ShootVelocity;
            tar.knockBack = source.Knockback;
            tar.scale = source.Scale;

            tar.noMelee = source.NoMelee;
            tar.consumable = source.IsConsumable;
            tar.useTurn = source.TurnPlayerOnUse;
            tar.autoReuse = source.AutoReuse;
            tar.noUseGraphic = source.HideUseGraphic;
            tar.accessory = source.IsAccessory;
            tar.expertOnly = source.IsExpertModeOnly;
            tar.channel = source.IsChanneled;

            tar.color = source.Colour;

            if (source.Rarity == ItemRarity.Amber)
                tar.questItem = true;
            else if (source.Rarity == ItemRarity.Rainbow)
                tar.expert = true;
            else
                tar.rare = (int)source.Rarity;

            tar.useStyle = (int)source.UseStyle;
            tar.holdStyle = (int)source.HoldStyle;

            switch (source.DamageType)
            {
                case DamageType.Melee:
                    tar.melee = true;
                    break;
                case DamageType.Ranged:
                    tar.ranged = true;
                    break;
                case DamageType.Magic:
                    tar.magic = true;
                    break;
                case DamageType.Minion:
                    tar.summon = true;
                    break;
                case DamageType.Thrown:
                    tar.thrown = true;
                    break;
                // None -> all false
            }

            tar.value = source.Value.Value;

            tar.vanity = source.Description.ShowVanity;
            tar.notAmmo = source.Description.HideAmmoFlag;
            tar.toolTip = source.Description.Description;
            tar.toolTip2 = source.Description.ExtraDescription;

            tar.buffTime = source.Buff.Duration;
            tar.buffType = source.Buff.Type;

            if (source.UsedAmmo != null)
                tar.useAmmo = source.UsedAmmo.Resolve().Type;

            tar.shoot = source.ShootProjectile;
            tar.ammo = source.AmmoType;
            tar.useSound = source.UseSound;
            tar.createTile = source.CreateTile;
            tar.createWall = source.CreateWall;

            tar.name = source.InternalName;
            Main.itemName[tar.type] = source.DisplayName;
        }
        static void CopyItemToDef(ItemDef tar, Item    source)
        {
            tar.Damage = source.damage;
            tar.UseAnimation = source.useAnimation;
            tar.UseTime = source.useTime;
            tar.ReuseDelay = source.reuseDelay;
            tar.ManaConsumption = source.mana;
            tar.Width = source.width;
            tar.Height = source.height;
            tar.MaxStack = source.maxStack;
            tar.PlacementStyle = source.placeStyle;
            tar.Alpha = source.alpha;
            tar.Defense = source.defense;
            tar.CritChanceModifier = source.crit;
            tar.PickaxePower = source.pick;
            tar.AxePower = source.axe * 5; // again, red, why did you do this?
            tar.HammerPower = source.hammer;
            tar.LifeHeal = source.healLife;
            tar.ManaHeal = source.healMana;

            tar.ShootVelocity = source.shootSpeed;
            tar.Knockback = source.knockBack;
            tar.Scale = source.scale;

            tar.NoMelee = source.noMelee;
            tar.IsConsumable = source.consumable;
            tar.TurnPlayerOnUse = source.useTurn;
            tar.AutoReuse = source.autoReuse;
            tar.HideUseGraphic = source.noUseGraphic;
            tar.IsAccessory = source.accessory;
            tar.IsExpertModeOnly = source.expertOnly;
            tar.IsChanneled = source.channel;

            tar.Colour = source.color;

            tar.Rarity = (ItemRarity)source.rare;
            if (source.questItem)
                tar.Rarity = ItemRarity.Amber;
            if (source.expert)
                tar.Rarity = ItemRarity.Rainbow;

            tar.UseStyle = (ItemUseStyle)source.useStyle;
            tar.HoldStyle = (ItemHoldStyle)source.holdStyle;

            tar.DamageType = DamageType.None;
            if (source.melee)
                tar.DamageType = DamageType.Melee;
            else if (source.ranged)
                tar.DamageType = DamageType.Ranged;
            else if (source.magic)
                tar.DamageType = DamageType.Magic;
            else if (source.summon)
                tar.DamageType = DamageType.Minion;
            else if (source.thrown)
                tar.DamageType = DamageType.Thrown;

            tar.Value = new CoinValue(source.value);
            tar.Description = new ItemDescription(source.toolTip, source.toolTip2, source.vanity, source.notAmmo);
            tar.Buff = new BuffDef(source.buffType, source.buffTime);

            if (source.useAmmo != 0)
                tar.UsedAmmo = new ItemRef(source.useAmmo);
            tar.ShootProjectile = source.shoot;
            tar.AmmoType = source.ammo;
            tar.UseSound = source.useSound;
            tar.CreateTile = source.createTile;
            tar.CreateWall = source.createWall;

            tar.GetTexture = () => Main.itemTexture[source.type];

            tar.InternalName = source.name;
            tar.DisplayName = Main.itemName[source.type];
        }
    }
}
