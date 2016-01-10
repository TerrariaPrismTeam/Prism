using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria;
using Terraria.DataStructures;

namespace Prism.Mods.Hooks
{
    static class TileHooks
    {
        internal static Dictionary<Point16, TileBHandler> TileSpecificHandlers = new Dictionary<Point16, TileBHandler>();
        internal static Dictionary<ushort , TileBHandler> TypeSpecificHandlers = new Dictionary<ushort , TileBHandler>();

        static TileBHandler LoadTypeSpecific(ushort t)
        {
            if (TypeSpecificHandlers.ContainsKey(t))
                return TypeSpecificHandlers[t];

            TileBHandler h = null; // will be set to <non-null> only if a behaviour handler will be attached

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
                .Where (kvp => kvp.Value != null)
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
                TypeSpecificHandlers.Add(t, h);

                // HasTile is false atm

                h.OnInit();
            }

            return h;
        }

        internal static Tuple<TileBHandler, TileBHandler> CreateBHandler(Point16 p)
        {
            TileBHandler h = null;
            var t = Main.tile[p.X, p.Y].type;

            var tsh = LoadTypeSpecific(t);

            if (Handler.TileDef.DefsByType.ContainsKey(t))
            {
                var d = Handler.TileDef.DefsByType[t];

                if (d.CreateInstanceBehaviour != null)
                {
                    var b = d.CreateInstanceBehaviour();

                    if (b != null)
                    {
                        h = new TileBHandler();

                        b.Mod = d.Mod == PrismApi.VanillaInfo ? null : ModData.mods[d.Mod];

                        h.behaviours.Add(b);
                    }
                }
            }

            if (h != null)
            {
                h.Create();
                TileSpecificHandlers.Add(p, h);

                foreach (var b in h.Behaviours)
                {
                    b.HasTile  = true;
                    b.Position = p;
                }

                h.OnInit();
            }

            return Tuple.Create(h, tsh);
        }

        internal static void OnPlaceThing(bool flag6)
        {
            if (!flag6)
                return;

            var p  = new Point16(Player.tileTargetX, Player.tileTargetY);
            var ts = CreateBHandler(p);

            ts.Item1.OnPlaced(p);
            ts.Item2.OnPlaced(p);
        }

        internal static void Reset()
        {
            TileSpecificHandlers.Clear();
            TypeSpecificHandlers.Clear();
        }

        //TODO: destroy
    }
}
