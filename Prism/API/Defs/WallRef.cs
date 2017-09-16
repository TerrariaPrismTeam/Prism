using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class WallRef : EntityRefWithId<WallDef>
    {
        static string ToResName(int id)
        {
            WallDef wd = null;
            if (Handler.WallDef.DefsByType.TryGetValue(id, out wd))
                return wd.InternalName;

            string r = null;
            if (Handler.WallDef.IDLUT.TryGetValue(id, out r))
                return r;

            throw new ArgumentException("id", "Unknown Wall ID '" + id + "'.");
        }

        public WallRef(int resourceId)
            : base(resourceId, ToResName)
        {
            /*
             * The C# compiler is drunk: WallID.Count is a byte of
             * value 231, but (int)WallID.Count == -25, according to
             * the compiler... >__>
             */
            if (resourceId < 0 || (uint)resourceId >= unchecked((uint)WallID.Count))
            {
                Console.Error.WriteLine("The resourceId must be a vanilla wall type, but is " + resourceId + "/" + WallID.Count + ".");
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla wall type, but is " + resourceId + "/" + WallID.Count + ".");
            }
        }
        public WallRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public WallRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public WallRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        WallRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.WallDef.DefsByType.ContainsKey(id) ? Handler.WallDef.DefsByType[id].InternalName : WallDefHandler.WALL + id)
        {

        }

        public static WallRef FromIDUnsafe(int resourceId)
        {
            return new WallRef(resourceId, null);
        }

        public override WallDef Resolve()
        {
            if (ResourceID.HasValue && Handler.WallDef.DefsByType.ContainsKey(ResourceID.Value))
                return Handler.WallDef.DefsByType[ResourceID.Value];

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.WallDefs.ContainsKey(ResourceName))
                return Requesting.WallDefs[ResourceName];

            if (IsVanillaRef)
            {
                if (!Handler.WallDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla wall reference '" + ResourceName + "' is not found.");

                return Handler.WallDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("Wall reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].WallDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("Wall reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the wall is not loaded.");

            return ModData.ModsFromInternalName[ModName].WallDefs[ResourceName];
        }
    }
}
