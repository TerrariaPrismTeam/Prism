using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.Defs;

namespace Prism.API
{
    public class ItemDef : EntityDef
    {
        /// <summary>
        /// Returns ItemDefs by their type number.
        /// </summary>
        public struct ByTypeGetter
        {
            public ItemDef this[int type]
            {
                get
                {
                    return ItemDefHandler.DefFromType[type];
                }
            }
        }

        /// <summary>
        /// Returns ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public struct ByNameGetter
        {
            public ItemDef this[string itemInternalName, string modInternalName = null]
            {
                get
                {
                    if (String.IsNullOrEmpty(modInternalName) || modInternalName == VanillaString || modInternalName == TerrariaString)
                        return ItemDefHandler.VanillaDefFromName[itemInternalName];

                    return ModData.ModsFromInternalName[modInternalName].ItemDefs[itemInternalName];
                }
            }
        }

        /// <summary>
        /// Returns ItemDefs by their type number.
        /// </summary>
        public static ByTypeGetter ByType
        {
            get
            {
                return new ByTypeGetter();
            }
        }

        /// <summary>
        /// Returns ItemDefs by their internal name (and optionally by their mod's internal name).
        /// </summary>
        public static ByNameGetter ByName
        {
            get
            {
                return new ByNameGetter();
            }
        }

        // stupid red and his stupid netids
        int setNetID = 0;
        /// <summary>
        /// Returns this item's stupid ass NetID (aka Phasesabre ID).
        /// </summary>
        public int NetID
        {
            get
            {
                return setNetID == 0 ? Type : setNetID;
            }
            internal set
            {
                setNetID = value;
            }
        }
		
        /// <summary>
        /// The damage this item does if it were a weapon.
        /// </summary>
        public virtual int Damage
        {
            get;
            set;
        }
    }
}
