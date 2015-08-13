using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Debugging;
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
            if (ModLoader.Reloading && !RecipeDefHandler.SettingUpRecipes)
            {
                item.RealSetDefaults(type, noMatCheck);

                if (!FillingVanilla)
                    Logging.LogWarning("Tried to call SetDefaults on an Item while [re|un]?loading mods.");

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

                if (!ModLoader.Reloading && d.CreateBehaviour != null)
                {
                    h = new ItemBHandler();

                    var b = d.CreateBehaviour();

                    if (b != null)
                        h.behaviours.Add(b);
                }
            }
            else
                item.RealSetDefaults(type, noMatCheck);

            var bs = ModLoader.Reloading ? Empty<ItemBehaviour>.Array : ModData.mods.Values.Select(m => m.contentHandler.CreateGlobalItemBInternally()).Where(b => b != null);

            if (!ModLoader.Reloading && !bs.IsEmpty() && h == null)
                h = new ItemBHandler();

            if (!ModLoader.Reloading && h != null)
            {
                h.behaviours.AddRange(bs);

                h.Create();
                item.P_BHandler = h;

                foreach (var b in h.Behaviours)
                    b.Entity = item;

                h.OnInit();
            }
        }
        internal static void OnSetDefaultsByName(Item item, string name)
        {
            item.name = String.Empty;
            bool noMatCheck = true;

            if (Handler.ItemDef.VanillaDefsByName.ContainsKey(name))
            {
                item.SetDefaults(Handler.ItemDef.VanillaDefsByName[name].NetID); // custom SetDefaults supports that, vanilla doesn't
                return;
            }
            if (name == "Blue Phasesaber")
            {
                item.SetDefaults(ItemID.BluePhaseblade);

                item.damage = 41;
                item.scale = 1.15f;
                item.autoReuse = true;
                item.useTurn = true;
                item.rare = 4;
                item.netID = ItemID.BluePhasesaber;
            }
            else if (name == "Red Phasesaber")
            {
                item.SetDefaults(ItemID.RedPhaseblade);

                item.damage = 41;
                item.scale = 1.15f;
                item.autoReuse = true;
                item.useTurn = true;
                item.rare = 4;
                item.netID = ItemID.RedPhasesaber;
            }
            else if (name == "Green Phasesaber")
            {
                item.SetDefaults(ItemID.GreenPhaseblade);

                item.damage = 41;
                item.scale = 1.15f;
                item.autoReuse = true;
                item.useTurn = true;
                item.rare = 4;
                item.netID = ItemID.GreenPhasesaber;
            }
            else if (name == "Purple Phasesaber")
            {
                item.SetDefaults(ItemID.PurplePhaseblade);

                item.damage = 41;
                item.scale = 1.15f;
                item.autoReuse = true;
                item.useTurn = true;
                item.rare = 4;
                item.netID = ItemID.PurplePhasesaber;
            }
            else if (name == "White Phasesaber")
            {
                item.SetDefaults(ItemID.WhitePhaseblade);

                item.damage = 41;
                item.scale = 1.15f;
                item.autoReuse = true;
                item.useTurn = true;
                item.rare = 4;
                item.netID = ItemID.WhitePhasesaber;
            }
            else if (name == "Yellow Phasesaber")
            {
                item.SetDefaults(ItemID.YellowPhaseblade);

                item.damage = 41;
                item.scale = 1.15f;
                item.autoReuse = true;
                item.useTurn = true;
                item.rare = 4;
                item.netID = ItemID.YellowPhasesaber;
            }
            else
            {
                noMatCheck = false;

                if (!String.IsNullOrEmpty(name))
                {
                    for (int i = 0; i < ItemID.Count; i++)
                        if (Main.itemName[i] == name)
                        {
                            item.SetDefaults(i);
                            item.checkMat();
                            return;
                        }

                    item.name = String.Empty;
                    item.stack = item.type = 0;
                }
            }

            if (item.type != 0)
            {
                if (noMatCheck)
                    item.material = false;
                else
                    item.checkMat();

                if (Handler.ItemDef.DefsByType.ContainsKey(item.type))
                    item.name = Handler.ItemDef.DefsByType[item.type].DisplayName;
                else
                {
                    item.name = name;
                    item.name = Lang.itemName(item.netID);
                }

                item.CheckTip();
            }
        }
    }
}
