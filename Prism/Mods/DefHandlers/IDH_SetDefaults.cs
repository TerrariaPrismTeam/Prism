using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.Debugging;
using Prism.IO;
using Prism.Mods.BHandlers;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.DefHandlers
{
    partial class ItemDefHandler
    {
        internal static void OnSetDefaults(Item item, int type, bool noMatCheck)
        {
            Dictionary<string, BinBuffer> data = null;
            if (item.P_BHandler != null)
                data = ((ItemBHandler)item.P_BHandler).data;

            item.P_BHandler = null;
            item.P_UseSound = null;

            if (ModLoader.Reloading)
            {
                item.RealSetDefaults(type, noMatCheck);

                if (!FillingVanilla && !RecipeDefHandler.SettingUpRecipes)
                {
                    Logging.LogWarning("Tried to call SetDefaults on an Item while [re|un]?loading mods.");
                    throw new Exception();
                }

                return;
            }

            ItemBHandler h = null; // will be set to <non-null> only if a behaviour handler will be attached

            item.RealSetDefaults(0, noMatCheck);

            if (Handler.ItemDef.DefsByType.ContainsKey(type))
            {
                var d = Handler.ItemDef.DefsByType[type];

                item.type = item.netID = type;
                item.width = item.height = 16;
                item.stack = item.maxStack = 1;

                Handler.ItemDef.CopyDefToEntity(d, item);

                if (RecipeDefHandler.SettingUpRecipes)
                    return;

                if (d.CreateBehaviour != null)
                {
                    var b = d.CreateBehaviour();

                    if (b != null)
                    {
                        h = new ItemBHandler();

                        b.Mod = d.Mod == PrismApi.VanillaInfo ? null : ModData.mods[d.Mod];

                        h.behaviours.Add(b);
                    }
                }
            }
            else
                item.RealSetDefaults(type, noMatCheck);

            if (RecipeDefHandler.SettingUpRecipes)
                return;

            var bs = ModData.mods.Values
                .Select(m => new KeyValuePair<ModDef, ItemBehaviour>(m, m.ContentHandler.CreateGlobalItemBInternally()))
                .Where(kvp => kvp.Value != null)
                .Select(kvp =>
                {
                    kvp.Value.Mod = kvp.Key;
                    return kvp.Value;
                });

            if (!bs.IsEmpty() && h == null)
                h = new ItemBHandler();

            if (h != null)
            {
                h.behaviours.AddRange(bs);

                if (data != null)
                    h.data = data;

                h.Create();
                item.P_BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = item;

                h.OnInit();
            }
        }
    }
}

