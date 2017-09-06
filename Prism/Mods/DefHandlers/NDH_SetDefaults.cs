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
    }
}

