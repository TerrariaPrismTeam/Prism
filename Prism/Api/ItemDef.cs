using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.API
{
    public class ItemDef : EntityDef
    {
        public struct ByTypeGetter
        {
            public ItemDef this[int type]
            {
                get
                {
                    return null;
                }
            }
        }
        public struct ByNameGetter
        {
            public ItemDef this[string itemInternalName, string modInternalName = null]
            {
                get
                {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == VanillaString || modInternalName == TerrariaString)
                    {
                        return null;
                    }

                    return null;
                }
            }
        }

        public virtual int Damage
        {
            get;
            set;
        }
    }
}
