using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.IO;

namespace Prism.Mods
{
    class UnknownItem : ItemBehaviour
    {
        readonly static ObjectName n = new ObjectName("Unknown Item");

        internal static ItemDef Create()
        {
            return new ItemDef(n, () => new UnknownItem(), () => TMain.UnknownItemTexture)
            {
                InternalName = "_UnknownItem", // this may never change, because otherwise the saved item data will get lost
                Mod = PrismApi.VanillaInfo
            };
        }
        internal static ItemDef Create(string modName, string itemName)
        {
            var r = Create();

            // best way to store it ever
            r.Description = new ItemDescription(
                    new[]{(ObjectName)modName, (ObjectName)itemName});

            return r;
        }

        public override void Save(BinBuffer bb)
        {
            if (Entity.ToolTip == null)
                Entity.RebuildTooltip();
            var itt = Entity.ToolTip;

            if (itt._tooltipLines.Length < 2)
            {
                bb.Write(String.Empty);
                bb.Write(String.Empty);
            }
            else
            {
                bb.Write(itt._tooltipLines[0]);
                bb.Write(itt._tooltipLines[1]);
            }
        }
        public override void Load(BinBuffer bb)
        {
            var modn = bb.ReadString();
            var itmn = bb.ReadString();

            Entity.ToolTip =
                new[]{(ObjectName)modn,(ObjectName)itmn}.ToTooltip();

            ModDef mod = null;
            ItemDef id;
            if (ModData.modsFromInternalName.TryGetValue(modn, out mod)
                    && mod.ItemDefs.TryGetValue(itmn, out id))
                Entity.SetDefaults(id.Type);
        }
    }
}

