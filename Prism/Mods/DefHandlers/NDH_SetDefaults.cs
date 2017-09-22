using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Debugging;
using Prism.Mods.BHandlers;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    partial class NpcDefHandler
    {
        static bool InSetDef = false;
        internal static void OnSetDefaults(NPC n, int type, float scaleOverride)
        {
            n.P_BHandler     = null;
            n.P_BuffBHandler = new object[NPC.maxBuffs];
            n.P_Music        = null;
            n.P_SoundOnHit   = null;
            n.P_SoundOnDeath = null;

            if (InSetDef)
                return;

            InSetDef = true;

            try
            {
            if (ModLoader.Reloading || FillingVanilla || TMain.IsInInit)
            {
                n.RealSetDefaults(type, scaleOverride);

                if (!FillingVanilla && !TMain.IsInInit)
                    Logging.LogWarning("Tried to call SetDefaults on an NPC while [re|un]?loading mods.");

                return;
            }

            NpcBHandler h = null; // will be set to <non-null> only if a behaviour handler will be attached

            //if (type < NPCID.Count && Handler.NpcDef.DefsByType.Count > 0)
            //    n.RealSetDefaults(type, scaleOverride);
            //else
                n.RealSetDefaults(0, scaleOverride);

            NpcDef d;
            if (Handler.NpcDef.DefsByType.TryGetValue(type, out d))
            {
                n.type = n.netID = type;
                n.width = n.height = 16;

                Handler.NpcDef.CopyDefToEntity(d, n);

                n.life = n.lifeMax; //! BEEP BOOP
                n.defDamage = n.damage;
                n.defDefense = n.defense;

                if (scaleOverride > -1f)
                {
                    n.scale  = scaleOverride;
                    n.width  = (int)(n.width  * scaleOverride);
                    n.height = (int)(n.height * scaleOverride);

                    if (n.height == 16 || n.width == 32) // wtf?
                        n.height++;

                    n.position += new Vector2(n.width, n.height) * 0.5f;
                }

                if (Main.expertMode)
                    n.scaleStats();

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
            {
                //n.RealSetDefaults(type, scaleOverride);
                Logging.LogWarning("There is no NpcDef of type " + type + "!");
            }

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
            finally
            {
                InSetDef = false;
            }
        }
    }
}

