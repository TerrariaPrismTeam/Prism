using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.IO;

namespace Prism.Mods
{
    class UnknownItem : ItemBehaviour
    {
        internal static ItemDef Create()
        {
            return new ItemDef("Unknown Item", () => new UnknownItem(), () => TMain.UnknownItemTexture)
            {
                InternalName = "_UnknownItem", // this may never change, because otherwise the saved item data will get lost
                Mod = PrismApi.VanillaInfo
            };
        }
        internal static ItemDef Create(string modName, string itemName)
        {
            var r = Create();

            // best way to store it ever
            r.Description = new ItemDescription(modName, itemName);

            return r;
        }

        public override void Save(BinBuffer bb)
        {
            bb.Write(Entity.toolTip );
            bb.Write(Entity.toolTip2);
        }
        public override void Load(BinBuffer bb)
        {
            Entity.toolTip  = bb.ReadString();
            Entity.toolTip2 = bb.ReadString();

            if (ModData.modsFromInternalName.ContainsKey(Entity.toolTip))
            {
                var mod = ModData.modsFromInternalName[Entity.toolTip];

                if (mod.ItemDefs.ContainsKey(Entity.toolTip2))
                    Entity.SetDefaults(ItemDef.Defs[Entity.toolTip2, Entity.toolTip].Type);
            }
        }
    }
}
