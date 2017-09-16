using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class ItemRef : EntityRefWithId<ItemDef>
    {
        static string ToResName(int id)
        {
            ItemDef ii = null;
            if (Handler.ItemDef.DefsByType.TryGetValue(id, out ii))
                return ii.InternalName;

            string r = null;
            if (Handler.ItemDef.IDLUT.TryGetValue(id, out r))
                return r;

            throw new ArgumentException("id", "Unknown Item ID '" + id + "'.");
        }

        public ItemRef(int resourceId)
            : base(resourceId, ToResName)
        {
            if (resourceId >= ItemID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Item type or netID.");
        }
        public ItemRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public ItemRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public ItemRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        ItemRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.ItemDef.DefsByType.ContainsKey(id) ? Handler.ItemDef.DefsByType[id].InternalName : String.Empty)
        {

        }

        public static ItemRef FromIDUnsafe(int resourceId)
        {
            return new ItemRef(resourceId, null);
        }

        public override ItemDef Resolve()
        {
            if (ResourceID.HasValue && Handler.ItemDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.ItemDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.ItemDefs.ContainsKey(ResourceName))
                return Requesting.ItemDefs[ResourceName];

            if (IsVanillaRef)
            {
                if (!Handler.ItemDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla item reference '" + ResourceName + "' is not found.");

                return Handler.ItemDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Item reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].ItemDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Item reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the item is not loaded.");

            return ModData.ModsFromInternalName[ModName].ItemDefs[ResourceName];
        }

        public static implicit operator Either<ItemRef, CraftGroup<ItemDef, ItemRef>>(ItemRef r)
        {
            return Either<ItemRef, CraftGroup<ItemDef, ItemRef>>.NewRight(r);
        }
        public static implicit operator Either<ItemRef, ItemGroup>(ItemRef r)
        {
            return Either<ItemRef, ItemGroup>.NewRight(r);
        }
    }
}
