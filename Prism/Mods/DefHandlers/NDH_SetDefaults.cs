using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API;
using Prism.API.Behaviours;
using Prism.Debugging;
using Prism.Mods.BHandlers;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    partial class NpcDefHandler
    {
        internal static void OnSetDefaults(NPC n, int type, float scaleOverride)
        {
            n.P_BHandler     = null;
            n.P_BuffBHandler = new object[NPC.maxBuffs];
            n.P_Music        = null;
            n.P_SoundOnHit   = null;
            n.P_SoundOnDeath = null;

            if (ModLoader.Reloading)
            {
                n.RealSetDefaults(type);

                if (!FillingVanilla)
                    Logging.LogWarning("Tried to call SetDefaults on an NPC while [re|un]?loading mods.");

                return;
            }

            NpcBHandler h = null; // will be set to <non-null> only if a behaviour handler will be attached

            if (type < NPCID.Count && !ModLoader.Reloading && !FillingVanilla && type != 0 && Handler.NpcDef.DefsByType.Count > 0)
                n.RealSetDefaults(type < 0 ? Handler.NpcDef.DefsByType[type].Type : type, scaleOverride);
            else
                n.RealSetDefaults(0, scaleOverride);

            if (Handler.NpcDef.DefsByType.ContainsKey(type))
            {
                var d = Handler.NpcDef.DefsByType[type];

                n.type = n.netID = type;
                n.width = n.height = 16;

                Handler.NpcDef.CopyDefToEntity(d, n);

                if (Main.expertMode)
                    n.scaleStats();

                n.life = n.lifeMax; //! BEEP BOOP
                n.defDamage = n.damage;
                n.defDefense = n.defense;

                if (scaleOverride > -1f)
                    n.scale = scaleOverride;

                if (d.CreateBehaviour != null)
                {
                    var b = d.CreateBehaviour();

                    if (b != null)
                    {
                        h = new NpcBHandler();

                        b.Mod = d.Mod == PrismApi.VanillaInfo ? null : ModData.mods[d.Mod];

                        h.behaviours.Add(b);
                    }
                }
            }
            else
                n.RealSetDefaults(type, scaleOverride);

            var bs = ModLoader.Reloading ? Empty<NpcBehaviour>.Array : ModData.mods.Values
                .Select(m => new KeyValuePair<ModDef, NpcBehaviour>(m, m.ContentHandler.CreateGlobalNpcBInternally()))
                .Where(kvp => kvp.Value != null)
                .Select(kvp =>
            {
                kvp.Value.Mod = kvp.Key;
                return kvp.Value;
            });

            if (!bs.IsEmpty() && h == null)
                h = new NpcBHandler();

            if (h != null)
            {
                h.behaviours.AddRange(bs);

                h.Create();
                n.P_BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = n;

                //h.OnInit();
            }
        }
        /*internal static void OnSetDefaultsByName(NPC n, string name)
        {
            bool isSlime = false;
            n.SetDefaults(0, -1f);

            if (Handler.NpcDef.VanillaDefsByName.ContainsKey(name))
            {
                n.SetDefaults(Handler.NpcDef.VanillaDefsByName[name].NetID);
                return;
            }
            if (name == "Slimeling")
            {
                n.SetDefaults(81, 0.6f);

                n.name = name;
                n.damage = 45;
                n.defense = 10;
                n.life = 90;
                n.knockBackResist *= 1.2f;
                n.value = 100f;
                n.netID = -1;
                isSlime = true;
            }
            else if (name == "Slimer2")
            {
                n.SetDefaults(81, 0.9f);

                n.displayName = "Slimer";
                n.name = name;
                n.damage = 45;
                n.defense = 20;
                n.life = 90;
                n.knockBackResist *= 1.2f;
                n.value = 100f;
                n.netID = -2;
                isSlime = true;
            }
            else if (name == "Green Slime")
            {
                n.SetDefaults(1, 0.9f);

                n.name = name;
                n.damage = 6;
                n.defense = 0;
                n.life = 14;
                n.knockBackResist *= 1.2f;
                n.color = new Color(0, 220, 40, 100);
                n.value = 3f;
                n.netID = -3;
                isSlime = true;
            }
            else if (name == "Pinky")
            {
                n.SetDefaults(1, 0.6f);

                n.name = name;
                n.damage = 5;
                n.defense = 5;
                n.life = 150;
                n.knockBackResist *= 1.4f;
                n.color = new Color(250, 30, 90, 90);
                n.value = 10000f;
                n.netID = -4;
                isSlime = true;
                n.rarity = 1;
            }
            else if (name == "Baby Slime")
            {
                n.SetDefaults(1, 0.9f);

                n.name = name;
                n.damage = 13;
                n.defense = 4;
                n.life = 30;
                n.knockBackResist *= 0.95f;
                n.alpha = 120;
                n.color = new Color(0, 0, 0, 50);
                n.value = 10f;
                n.netID = -5;
                isSlime = true;
            }
            else if (name == "Black Slime")
            {
                n.SetDefaults(1, 1.05f);

                n.name = name;
                n.damage = 15;
                n.defense = 4;
                n.life = 45;
                n.color = new Color(0, 0, 0, 50);
                n.value = 20f;
                n.netID = -6;
                isSlime = true;
            }
            else if (name == "Purple Slime")
            {
                n.SetDefaults(1, 1.2f);

                n.name = name;
                n.damage = 12;
                n.defense = 6;
                n.life = 40;
                n.knockBackResist *= 0.9f;
                n.color = new Color(200, 0, 255, 150);
                n.value = 10f;
                n.netID = -7;
                isSlime = true;
            }
            else if (name == "Red Slime")
            {
                n.SetDefaults(1, 1.025f);

                n.name = name;
                n.damage = 12;
                n.defense = 4;
                n.life = 35;
                n.color = new Color(255, 30, 0, 100);
                n.value = 8f;
                n.netID = -8;
                isSlime = true;
            }
            else if (name == "Yellow Slime")
            {
                n.SetDefaults(1, 1.2f);

                n.name = name;
                n.damage = 15;
                n.defense = 7;
                n.life = 45;
                n.color = new Color(255, 255, 0, 100);
                n.value = 10f;
                n.netID = -9;
                isSlime = true;
            }
            else if (name == "Jungle Slime")
            {
                n.SetDefaults(1, 1.1f);

                n.name = name;
                n.damage = 18;
                n.defense = 6;
                n.life = 60;
                n.color = new Color(143, 215, 93, 100);
                n.value = 500f;
                n.netID = -10;
                isSlime = true;
            }
            else if (name == "Little Eater")
            {
                n.SetDefaults(6, 0.85f);

                n.name = name;
                n.displayName = "Eater of Souls";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -11;
            }
            else if (name == "Big Eater")
            {
                n.SetDefaults(6, 1.15f);

                n.name = name;
                n.displayName = "Eater of Souls";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -12;
            }
            else if (name == "Short Bones")
            {
                n.SetDefaults(31, 0.9f);

                n.name = name;
                n.displayName = "Angry Bones";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.netID = -13;
            }
            else if (name == "Big Boned")
            {
                n.SetDefaults(31, 1.15f);

                n.name = name;
                n.displayName = "Angry Bones";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale * 1.1);
                n.life = (int)(n.life * n.scale * 1.1);
                n.value = (int)(n.value * n.scale);
                n.npcSlots = 2f;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -14;
            }
            else if (name == "Heavy Skeleton")
            {
                n.SetDefaults(77, 1.15f);

                n.name = name;
                n.displayName = "Armored Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale * 1.1);
                n.life = 400;
                n.value = (int)(n.value * n.scale);
                n.npcSlots = 2f;
                n.knockBackResist *= 2f - n.scale;
                n.height = 44;
                n.netID = -15;
            }
            else if (name == "Little Stinger")
            {
                n.SetDefaults(42, 0.85f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -16;
            }
            else if (name == "Big Stinger")
            {
                n.SetDefaults(42, 1.2f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -17;
            }
            else if (name == "Tiny Moss Hornet")
            {
                n.SetDefaults(176, 0.8f);

                n.displayName = "Moss Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -18;
            }
            else if (name == "Little Moss Hornet")
            {
                n.SetDefaults(176, 0.9f);

                n.displayName = "Moss Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -19;
            }
            else if (name == "Big Moss Hornet")
            {
                n.SetDefaults(176, 1.1f);

                n.displayName = "Moss Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -20;
            }
            else if (name == "Giant Moss Hornet")
            {
                n.SetDefaults(176, 1.2f);

                n.displayName = "Moss Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -21;
            }
            else if (name == "Little Crimera")
            {
                n.SetDefaults(173, 0.85f);

                n.displayName = "Crimera";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -22;
            }
            else if (name == "Big Crimera")
            {
                n.SetDefaults(173, 1.15f);

                n.displayName = "Crimera";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -23;
            }
            else if (name == "Little Crimslime")
            {
                n.SetDefaults(183, 0.85f);

                n.displayName = "Crimslime";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -24;
            }
            else if (name == "Big Crimslime")
            {
                n.SetDefaults(183, 1.15f);

                n.displayName = "Crimslime";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -25;
            }
            else if (name == "Small Zombie")
            {
                n.SetDefaults(3, 0.9f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -26;
            }
            else if (name == "Big Zombie")
            {
                n.SetDefaults(3, 1.1f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -27;
            }
            else if (name == "Small Bald Zombie")
            {
                n.SetDefaults(132, 0.85f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -28;
            }
            else if (name == "Big Bald Zombie")
            {
                n.SetDefaults(132, 1.15f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -29;
            }
            else if (name == "Small Pincushion Zombie")
            {
                n.SetDefaults(186, 0.93f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -30;
            }
            else if (name == "Big Pincushion Zombie")
            {
                n.SetDefaults(186, 1.13f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -31;
            }
            else if (name == "Small Slimed Zombie")
            {
                n.SetDefaults(187, 0.89f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -32;
            }
            else if (name == "Big Slimed Zombie")
            {
                n.SetDefaults(187, 1.11f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -33;
            }
            else if (name == "Small Swamp Zombie")
            {
                n.SetDefaults(188, 0.87f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -34;
            }
            else if (name == "Big Swamp Zombie")
            {
                n.SetDefaults(188, 1.13f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -35;
            }
            else if (name == "Small Twiggy Zombie")
            {
                n.SetDefaults(189, 0.92f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -36;
            }
            else if (name == "Big Twiggy Zombie")
            {
                n.SetDefaults(189, 1.08f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -37;
            }
            else if (name == "Cataract Eye 2")
            {
                n.SetDefaults(190, 1.15f);

                n.name = name;
                n.displayName = "Demon Eye";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -38;
            }
            else if (name == "Sleepy Eye 2")
            {
                n.SetDefaults(191, 1.1f);

                n.name = name;
                n.displayName = "Demon Eye";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -39;
            }
            else if (name == "Dialated Eye 2")
            {
                n.SetDefaults(192, 0.9f);

                n.name = name;
                n.displayName = "Demon Eye";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -40;
            }
            else if (name == "Green Eye 2")
            {
                n.SetDefaults(193, 0.85f);

                n.name = name;
                n.displayName = "Demon Eye";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -41;
            }
            else if (name == "Purple Eye 2")
            {
                n.SetDefaults(194, 1.1f);

                n.name = name;
                n.displayName = "Demon Eye";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -42;
            }
            else if (name == "Demon Eye 2")
            {
                n.SetDefaults(2, 1.15f);

                n.name = name;
                n.displayName = "Demon Eye";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -43;
            }
            else if (name == "Small Female Zombie")
            {
                n.SetDefaults(200, 0.87f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -44;
            }
            else if (name == "Big Female Zombie")
            {
                n.SetDefaults(200, 1.05f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -45;
            }
            else if (name == "Small Skeleton")
            {
                n.SetDefaults(21, 0.9f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -46;
            }
            else if (name == "Big Skeleton")
            {
                n.SetDefaults(21, 1.1f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -47;
            }
            else if (name == "Small Headache Skeleton")
            {
                n.SetDefaults(201, 0.93f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -48;
            }
            else if (name == "Big Headache Skeleton")
            {
                n.SetDefaults(201, 1.07f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -49;
            }
            else if (name == "Small Misassembled Skeleton")
            {
                n.SetDefaults(202, 0.87f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -50;
            }
            else if (name == "Big Misassembled Skeleton")
            {
                n.SetDefaults(202, 1.13f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -51;
            }
            else if (name == "Small Pantless Skeleton")
            {
                n.SetDefaults(203, 0.85f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -52;
            }
            else if (name == "Big Pantless Skeleton")
            {
                n.SetDefaults(203, 1.15f);

                n.name = name;
                n.displayName = "Skeleton";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -53;
            }
            else if (name == "Small Rain Zombie")
            {
                n.SetDefaults(223, 0.9f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -54;
            }
            else if (name == "Big Rain Zombie")
            {
                n.SetDefaults(223, 1.1f);

                n.name = name;
                n.displayName = "Zombie";
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -55;
            }
            else if (name == "Little Hornet Fatty")
            {
                n.SetDefaults(231, 0.85f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -56;
            }
            else if (name == "Big Hornet Fatty")
            {
                n.SetDefaults(231, 1.25f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -57;
            }
            else if (name == "Little Hornet Honey")
            {
                n.SetDefaults(232, 0.8f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -58;
            }
            else if (name == "Big Hornet Honey")
            {
                n.SetDefaults(232, 1.15f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -59;
            }
            else if (name == "Little Hornet Leafy")
            {
                n.SetDefaults(233, 0.92f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -60;
            }
            else if (name == "Big Hornet Leafy")
            {
                n.SetDefaults(233, 1.1f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -61;
            }
            else if (name == "Little Hornet Spikey")
            {
                n.SetDefaults(234, 0.78f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -62;
            }
            else if (name == "Big Hornet Spikey")
            {
                n.SetDefaults(234, 1.16f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -63;
            }
            else if (name == "Little Hornet Stingy")
            {
                n.SetDefaults(235, 0.87f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -64;
            }
            else if (name == "Big Hornet Stingy")
            {
                n.SetDefaults(235, 1.21f);

                n.displayName = "Hornet";
                n.name = name;
                n.defense = (int)(n.defense * n.scale);
                n.damage = (int)(n.damage * n.scale);
                n.life = (int)(n.life * n.scale);
                n.value = (int)(n.value * n.scale);
                n.npcSlots *= n.scale;
                n.knockBackResist *= 2f - n.scale;
                n.netID = -65;
            }
            else if (name != String.Empty)
            {
                for (int i = 1; i < NPCID.Count; i++)
                    if (Main.npcName[i] == name)
                    {
                        n.SetDefaults(i);
                        return;
                    }

                n.SetDefaults(0, -1f);
                n.active = false;
            }

            if (n.type == 0)
                n.active = false;

            n.displayName = Lang.npcName(n.netID, false);
            n.lifeMax = n.life;
            n.defDamage = n.damage;
            n.defDefense = n.defense;

            if (Main.expertMode && isSlime)
                n.scaleStats();
        }*/
    }
}
