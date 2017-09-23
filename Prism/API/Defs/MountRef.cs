using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;

namespace Prism.API.Defs
{
    public class MountRef : EntityRefWithId<MountDef>
    {
        static string ToResName(int id)
        {
            MountDef md = null;
            if (Handler.MountDef.DefsByType.TryGetValue(id, out md))
                return md.InternalName;

            string r = null;
            if (Handler.MountDef.IDLUT.TryGetValue(id, out r))
                return r;

            throw new ArgumentException("id", "Unknown Mount ID '" + id + "'.");
        }

        public MountRef(int resourceId)
            : base(resourceId, ToResName)
        {
            if (resourceId >= MountID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Mount type, but is " + resourceId + "/" + MountID.Count + ".");
        }
        public MountRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public MountRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public MountRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName, Assembly.GetCallingAssembly()), Assembly.GetCallingAssembly())
        {

        }

        [ThreadStatic]
        static MountDef md;
        MountRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.MountDef.DefsByType.TryGetValue(id, out md) ? md.InternalName : String.Empty)
        {

        }

        public static MountRef FromIDUnsafe(int resourceId)
        {
            return new MountRef(resourceId, null);
        }

        public override MountDef Resolve()
        {
            MountDef r;

            if (ResourceID.HasValue && Handler.MountDef.DefsByType.TryGetValue(ResourceID.Value, out r))
                return r;

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.MountDefs.TryGetValue(ResourceName, out r))
                return r;

            if (IsVanillaRef)
            {
                if (!Handler.MountDef.VanillaDefsByName.TryGetValue(ResourceName, out r))
                    throw new InvalidOperationException("Vanilla mount reference '" + ResourceName + "' is not found.");

                return r;
            }

            ModDef m;
            if (!ModData.ModsFromInternalName.TryGetValue(ModName, out m))
                throw new InvalidOperationException("Mount reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!m.MountDefs.TryGetValue(ResourceName, out r))
                throw new InvalidOperationException("Mount reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mount is not loaded.");

            return r;
        }

        public static implicit operator MountRef(Mount m)
        {
            if (m.Type < MountID.Count)
                return new MountRef(m.Type);

            MountDef d;
            if (Handler.MountDef.DefsByType.TryGetValue(m.Type, out d))
                return d;

            throw new InvalidOperationException("Mount '" + m + "' (" + m.Type + ") is not in the def database.");
        }
    }
}
