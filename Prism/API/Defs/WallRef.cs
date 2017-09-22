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
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla wall type, but is " + resourceId + "/" + WallID.Count + ".");
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
            : base(new ObjectRef(resourceName, modName, Assembly.GetCallingAssembly()), Assembly.GetCallingAssembly())
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
            WallDef r;

            if (ResourceID.HasValue && Handler.WallDef.DefsByType.TryGetValue(ResourceID.Value, out r))
                return r;

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.WallDefs.TryGetValue(ResourceName, out r))
                return r;

            if (IsVanillaRef)
            {
                if (!Handler.WallDef.VanillaDefsByName.TryGetValue(ResourceName, out r))
                    throw new InvalidOperationException("Vanilla wall reference '" + ResourceName + "' is not found.");

                return r;
            }

            ModDef m;
            if (!ModData.ModsFromInternalName.TryGetValue(ModName, out m))
                throw new InvalidOperationException("Wall reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!m.WallDefs.TryGetValue(ResourceName, out r))
                throw new InvalidOperationException("Wall reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the wall is not loaded.");

            return r;
        }

        public static implicit operator WallRef(int w)
        {
            if (w < WallID.Count)
                return new WallRef(w);

            WallDef d;
            if (Handler.WallDef.DefsByType.TryGetValue(w, out d))
                return d;

            throw new InvalidOperationException("Wall " + w + " is not in the def database.");
        }
    }
}
