using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Defs;
using Terraria;

namespace Prism.API
{       
    public struct ItemGroup : IEnumerable<ItemDef>
    {
        private IEnumerable<ItemDef> ItemDefs;

        public ItemGroup(params ItemDef[] itemDefs)
        {
            ItemDefs = from i in itemDefs select i;
        }

        public ItemGroup(params int[] itemTypes)
        {
            ItemDefs = from i in itemTypes select ItemDef.ByType[i];
        }

        public IEnumerator<ItemDef> GetEnumerator()
        {
            return ItemDefs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}
