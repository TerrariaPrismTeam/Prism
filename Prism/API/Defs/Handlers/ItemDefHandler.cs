using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;
using Prism.Mods.Behaviours;
using Prism.Mods;

namespace Prism.Defs.Handlers
{
    public class ItemDefHandler : EntityDefHandler<ItemDef, ItemBehaviour, Item>
    {
        internal static void OnSetDefaults(Item item, int type, bool noMatCheck)
        {
            ItemBHandler h = null;

            if (type >= ItemID.Count)
            {
                item.RealSetDefaults(0, noMatCheck);

                if (Handler.ItemDef.DefsByType.ContainsKey(type))
                {
                    var def = Handler.ItemDef.DefsByType[type];

                    item.type = item.netID = type;
                    item.width = item.height = 16;
                    item.stack = item.maxStack = 1;

                    Handler.ItemDef.CopyDefToEntity(def, item);

                    h = new ItemBHandler();
                    if (def.CreateBehaviour != null)
                    {
                        var b = def.CreateBehaviour();

                        if (b != null)
                            h.behaviours.Add(b);
                    }

                    item.BHandler = h;
                }
            }
            else
                item.RealSetDefaults(type, noMatCheck);

            //TODO: add global hooks here (and check for null)

            if (h != null)
            {
                h.Create();
                item.BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = item;

                h.OnInit();
            }
        }

        public override void ExtendVanillaArrays(int amt = 1)
        {
            int newLen = amt > 0 ? Main.itemAnimations.Length + amt : ItemID.Count;
            if (!Main.dedServ)
                Array.Resize(ref Main.itemTexture                  , newLen);
            Array.Resize(ref Main.itemAnimations                   , newLen);
            Array.Resize(ref Main.itemFlameLoaded                  , newLen);
            Array.Resize(ref Main.itemFlameTexture                 , newLen);
            Array.Resize(ref Main.itemFrame                        , newLen);
            Array.Resize(ref Main.itemFrameCounter                 , newLen);
            Array.Resize(ref Main.itemName                         , newLen);                                                                   
            Array.Resize(ref Item.bodyType                         , newLen);
            Array.Resize(ref Item.claw                             , newLen);
            Array.Resize(ref Item.headType                         , newLen);
            Array.Resize(ref Item.legType                          , newLen);
            Array.Resize(ref Item.staff                            , newLen);                                                                   
            Array.Resize(ref ItemID.Sets.AnimatesAsSoul            , newLen);
            Array.Resize(ref ItemID.Sets.Deprecated                , newLen);
            Array.Resize(ref ItemID.Sets.ExoticPlantsForDyeTrade   , newLen);
            Array.Resize(ref ItemID.Sets.ExtractinatorMode         , newLen);
            Array.Resize(ref ItemID.Sets.gunProj                   , newLen);
            Array.Resize(ref ItemID.Sets.ItemIconPulse             , newLen);
            Array.Resize(ref ItemID.Sets.ItemNoGravity             , newLen);
            Array.Resize(ref ItemID.Sets.NebulaPickup              , newLen);
            Array.Resize(ref ItemID.Sets.NeverShiny                , newLen);
            Array.Resize(ref ItemID.Sets.StaffMinionSlotsRequired  , newLen);
        }       

        public override Item GetVanillaEntityFromID(int id)
        {
            Item i = new Item();
            i.netDefaults(id);
            return i;
        }

        public override void CopyEntityToDef(Item entity, ItemDef def)
        {
            def.InternalName                                = entity.name;
            def.DisplayName                                 = Main.itemName[entity.type];
            def.Type                                        = entity.type;
            def.NetID                                       = entity.netID;
            def.Damage                                      = entity.damage;
            def.UseAnimation                                = entity.useAnimation;
            def.UseTime                                     = entity.useTime;
            def.ReuseDelay                                  = entity.reuseDelay;
            def.ManaConsumption                             = entity.mana;
            def.Width                                       = entity.width;
            def.Height                                      = entity.height;
            def.MaxStack                                    = entity.maxStack;
            def.PlacementStyle                              = entity.placeStyle;
            def.Alpha                                       = entity.alpha;
            def.Defense                                     = entity.defense;
            def.CritChanceModifier                          = entity.crit;
            def.PickaxePower                                = entity.pick;
            def.AxePower                                    = entity.axe * 5; // again, red, why did you do this?
            def.HammerPower                                 = entity.hammer;
            def.LifeHeal                                    = entity.healLife;
            def.ManaHeal                                    = entity.healMana;
            def.ShootVelocity                               = entity.shootSpeed;
            def.Knockback                                   = entity.knockBack;
            def.Scale                                       = entity.scale;
            def.NoMelee                                     = entity.noMelee;
            def.IsConsumable                                = entity.consumable;
            def.TurnPlayerOnUse                             = entity.useTurn;
            def.AutoReuse                                   = entity.autoReuse;
            def.HideUseGraphic                              = entity.noUseGraphic;
            def.IsAccessory                                 = entity.accessory;
            def.IsExpertModeOnly                            = entity.expertOnly;
            def.IsChanneled                                 = entity.channel;
            def.Colour                                      = entity.color;
            def.Rarity                                      = (ItemRarity)entity.rare;            
            def.UseStyle                                    = (ItemUseStyle)entity.useStyle;
            def.HoldStyle                                   = (ItemHoldStyle)entity.holdStyle;
            def.DamageType                                  = entity.melee ? ItemDamageType.Melee
                                                            : (entity.ranged ? ItemDamageType.Ranged
                                                            : (entity.magic ? ItemDamageType.Magic
                                                            : (entity.summon ? ItemDamageType.Summon
                                                            : (entity.thrown ? ItemDamageType.Thrown 
                                                            : ItemDamageType.None))));
            def.Value                                       = new CoinValue(entity.value);
            def.Description                                 = new ItemDescription(entity.toolTip, entity.toolTip2, entity.vanity, entity.expert, entity.questItem, entity.notAmmo);                              
            def.Buff                                        = new BuffDef(entity.buffType, entity.buffTime);
            def.UsedAmmo                                    = (entity.useAmmo != 0) ? new ItemRef(entity.useAmmo) : null;
            def.ShootProjectile                             = entity.shoot;
            def.AmmoType                                    = entity.ammo;
            def.UseSound                                    = entity.useSound;
            def.CreateTile                                  = entity.createTile;
            def.CreateWall                                  = entity.createWall;
            def.GetTexture                                  = () => Main.itemTexture[entity.type];   
               
            def.DisplayName                                 = Main.itemName                        [def.Type];
            def.IsSoul                                      = ItemID.Sets.AnimatesAsSoul           [def.Type];
            def.IsStrangePlant                              = ItemID.Sets.ExoticPlantsForDyeTrade  [def.Type];
            def.IsBullet                                    = ItemID.Sets.gunProj                  [def.Type];
            def.Pulses                                      = ItemID.Sets.ItemIconPulse            [def.Type];
            def.NoGravity                                   = ItemID.Sets.ItemNoGravity            [def.Type];
            def.IsNebulaPickup                              = ItemID.Sets.NebulaPickup             [def.Type];
            def.NeverShiny                                  = ItemID.Sets.NeverShiny               [def.Type];
            def.ExtractinatorMode                           = ItemID.Sets.ExtractinatorMode        [def.Type];
            def.RequiredStaffMinionSlots                    = ItemID.Sets.StaffMinionSlotsRequired [def.Type];     
        }

        public override void CopyDefToEntity(ItemDef def, Item entity)
        {
            entity.name                                     = def.InternalName;
            entity.type                                     = def.Type;
            entity.netID                                    = def.NetID;
            entity.damage                                   = def.Damage;
            entity.useAnimation                             = def.UseAnimation;
            entity.useTime                                  = def.UseTime;
            entity.reuseDelay                               = def.ReuseDelay;
            entity.mana                                     = def.ManaConsumption;
            entity.width                                    = def.Width;
            entity.height                                   = def.Height;
            entity.maxStack                                 = def.MaxStack;
            entity.placeStyle                               = def.PlacementStyle;
            entity.alpha                                    = def.Alpha;
            entity.defense                                  = def.Defense;
            entity.crit                                     = def.CritChanceModifier;
            entity.pick                                     = def.PickaxePower;
            entity.axe                                      = def.AxePower / 5; // fucking red
            entity.hammer                                   = def.HammerPower;
            entity.healLife                                 = def.LifeHeal;
            entity.healMana                                 = def.ManaHeal;
            entity.shootSpeed                               = def.ShootVelocity;
            entity.knockBack                                = def.Knockback;
            entity.scale                                    = def.Scale;
            entity.noMelee                                  = def.NoMelee;
            entity.consumable                               = def.IsConsumable;
            entity.useTurn                                  = def.TurnPlayerOnUse;
            entity.autoReuse                                = def.AutoReuse;
            entity.noUseGraphic                             = def.HideUseGraphic;
            entity.accessory                                = def.IsAccessory;
            entity.expertOnly                               = def.IsExpertModeOnly;
            entity.channel                                  = def.IsChanneled;
            entity.color                                    = def.Colour;
            entity.useStyle                                 = (int)def.UseStyle;
            entity.holdStyle                                = (int)def.HoldStyle;                                                                                    
            entity.rare                                     = (int)def.Rarity;                                          
            entity.useStyle                                 = (int)def.UseStyle;
            entity.holdStyle                                = (int)def.HoldStyle;                                          
            entity.melee                                    = (def.DamageType == ItemDamageType.Melee);
            entity.ranged                                   = (def.DamageType == ItemDamageType.Ranged);
            entity.magic                                    = (def.DamageType == ItemDamageType.Magic);
            entity.summon                                   = (def.DamageType == ItemDamageType.Summon);
            entity.thrown                                   = (def.DamageType == ItemDamageType.Thrown);                                          
            entity.value                                    = def.Value.Value;                                          
            entity.vanity                                   = def.Description.ShowVanity;
            entity.notAmmo                                  = def.Description.HideAmmoFlag;
            entity.toolTip                                  = def.Description.Description;
            entity.toolTip2                                 = def.Description.ExtraDescription;
            entity.questItem                                = def.Description.ShowQuestItem;
            entity.expert                                   = def.Description.ShowExpert;                                          
            entity.buffTime                                 = def.Buff.Duration;
            entity.buffType                                 = def.Buff.Type;                                                           
            if (def.UsedAmmo != null)                      
                entity.useAmmo                              = def.UsedAmmo.Resolve().Type;                                                           
            entity.shoot                                    = def.ShootProjectile;
            entity.ammo                                     = def.AmmoType;
            entity.useSound                                 = def.UseSound;
            entity.createTile                               = def.CreateTile;
            entity.createWall                               = def.CreateWall;  
                        
            Main.itemName                        [def.Type] = def.DisplayName;
            ItemID.Sets.AnimatesAsSoul           [def.Type] = def.IsSoul;
            ItemID.Sets.ExoticPlantsForDyeTrade  [def.Type] = def.IsStrangePlant;
            ItemID.Sets.gunProj                  [def.Type] = def.IsBullet;
            ItemID.Sets.ItemIconPulse            [def.Type] = def.Pulses;
            ItemID.Sets.ItemNoGravity            [def.Type] = def.NoGravity;
            ItemID.Sets.NebulaPickup             [def.Type] = def.IsNebulaPickup;
            ItemID.Sets.NeverShiny               [def.Type] = def.NeverShiny;
            ItemID.Sets.ExtractinatorMode        [def.Type] = def.ExtractinatorMode;
            ItemID.Sets.StaffMinionSlotsRequired [def.Type] = def.RequiredStaffMinionSlots;
        }

        public override bool CheckTextures(ItemDef def)
        {
            return !(def.GetTexture == null);
        }        

        public override void LoadTextures(ref List<LoaderError> err, ItemDef def)
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
        
        public override void PostFillVanilla()
        {
            Recipes.AddVanillaRecipeReferences();
        }        
    }   
}