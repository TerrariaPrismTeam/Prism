using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.IO;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;

namespace Prism.Mods.Hooks
{
    /*
     *
     * A TileEntity is used to provide hooks for TileBehaviours.
     * These are saved and loaded as usual, but when vanilla would read an unknown TileEntity ID,
     * it'd nullref (when Prism -> vanilla). Faking a training dummy seems to be a solution, it writes an NPC ID.
     * By assigning 199 as the dummy's NPC ID (-1 happens more often, lower than -1 or greater than 199 would
     * result in an IndexOutOfRange), it is marked as 'Prism TileBHandler TileEntity', and vanilla will handle it
     * correctly when moving Prism worlds to vanilla (behaviour data is written to the .prism file).
     *
     *
     * This is a disgusting hack.
     *
     */

    static class TileHooks
    {
        static List<TETrainingDummy> fakeDummies = new List<TETrainingDummy>();

        static TileBHandler CreateBHandler(Point16 p)
        {
            TileBHandler h = null;
            var t = Main.tile[p.X, p.Y].type;

            if (Handler.TileDef.DefsByType.ContainsKey(t))
            {
                var d = Handler.TileDef.DefsByType[t];

                if (d.CreateBehaviour != null)
                {
                    var b = d.CreateBehaviour();

                    if (b != null)
                    {
                        h = new TileBHandler();

                        b.Mod = d.Mod == PrismApi.VanillaInfo ? null : ModData.mods[d.Mod];

                        h.behaviours.Add(b);
                    }
                }
            }

            var bs = ModData.mods.Values
                .Select(m => new KeyValuePair<ModDef, TileBehaviour>(m, m.ContentHandler.CreateGlobalTileBInternally()))
                .Where(kvp => kvp.Value != null)
                .Select(kvp =>
                {
                    kvp.Value.Mod = kvp.Key;
                    return kvp.Value;
                });

            if (!bs.IsEmpty() && h == null)
                h = new TileBHandler();

            if (h != null)
            {
                h.behaviours.AddRange(bs);

                h.Create();

                foreach (var b in h.behaviours)
                    b.Entity = Main.tile[p.X, p.Y];

                h.OnInit();
            }
            return h;
        }

        internal static void TDReadExtraData(TETrainingDummy d, BinaryReader r)
        {
            d.npc = r.ReadInt16();

            if (d.npc == Main.maxNPCs - 1)
            {
                d.npc = -1;

                fakeDummies.Add(d);
            }
        }

        internal static void SwapFakeDummies()
        {
            for (int i = 0; i < fakeDummies.Count; i++)
            {
                fakeDummies[i].Deactivate();

                TileEntity.ByID.Remove      (fakeDummies[i].ID      );
                TileEntity.ByPosition.Remove(fakeDummies[i].Position);

                var replacement = new TileBHandlerEntity(null);

                replacement.ID       = TileEntity.AssignNewID();
                replacement.Position = fakeDummies[i].Position;
                replacement.type     = fakeDummies[i].type;

                TileEntity.ByID.Add      (replacement.ID      , replacement);
                TileEntity.ByPosition.Add(replacement.Position, replacement);
            }

            fakeDummies.Clear();
        }
        internal static void CreateBHandlers()
        {
            foreach (var te in TileEntity.ByID.Values)
                if (te is TileBHandlerEntity)
                {
                    var bhe = te as TileBHandlerEntity;

                    bhe.bHandler = CreateBHandler(bhe.Position);
                }
        }
    }
}
