using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods.Defs;

namespace Prism.API
{
    public class ItemRef : EntityRef<ItemDef>
    {
        public ItemRef(int resourceId)
            : base(resourceId)
        {

        }
        public ItemRef(string resourceName, string modName = null)
            : base(resourceName, modName)
        {

        }

        public override ItemDef Resolve()
        {
            // TODO: handle missing keys
            return Mod == PrismApi.VanillaInfo ? ItemDefHandler.VanillaDefFromName[ResourceName] : ItemDef.ByName[ResourceName, ModName];
        }
    }
}
