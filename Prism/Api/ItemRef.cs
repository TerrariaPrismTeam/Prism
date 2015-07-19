using System;
using System.Collections.Generic;
using System.Linq;

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
            return ItemDef.ByName[ResourceName, ModName];
        }
    }
}
