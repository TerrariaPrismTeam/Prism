using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class ProjectileRef : EntityRefWithId<ProjectileDef>
    {
        static string ToResName(int id)
        {
            ProjectileDef pd = null;
            if (Handler.ProjDef.DefsByType.TryGetValue(id, out pd))
                return pd.InternalName;

            string r = null;
            if (Handler.ProjDef.IDLUT.TryGetValue(id, out r))
                return r;

            throw new ArgumentException("id", "Unknown Projectile ID '" + id + "'.");
        }

        public ProjectileRef(int resourceId)
            : base(resourceId, ToResName)
        {
            if (resourceId >= ProjectileID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla Projectile type.");
        }
        public ProjectileRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public ProjectileRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public ProjectileRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName, Assembly.GetCallingAssembly()), Assembly.GetCallingAssembly())
        {

        }

        [ThreadStatic]
        static ProjectileDef pd;
        ProjectileRef(int resourceId, object ignore)
            : base(resourceId, id => Handler.ProjDef.DefsByType.TryGetValue(id, out pd) ? pd.InternalName : String.Empty)
        {

        }

        public static ProjectileRef FromIDUnsafe(int resourceId)
        {
            return new ProjectileRef(resourceId, null);
        }

        public override ProjectileDef Resolve()
        {
            ProjectileDef r;

            if (ResourceID.HasValue && Handler.ProjDef.DefsByType.TryGetValue(ResourceID.Value, out r))
                return r;

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.ProjectileDefs.TryGetValue(ResourceName, out r))
                return r;

            if (IsVanillaRef)
            {
                if (!Handler.ProjDef.VanillaDefsByName.TryGetValue(ResourceName, out r))
                    throw new InvalidOperationException("Vanilla NPC reference '" + ResourceName + "' is not found.");

                return r;
            }

            ModDef m;
            if (!ModData.ModsFromInternalName.TryGetValue(ModName, out m))
                throw new InvalidOperationException("Projectile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!m.ProjectileDefs.TryGetValue(ResourceName, out r))
                throw new InvalidOperationException("Projectile reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the Projectile is not loaded.");

            return r;
        }

        public static implicit operator ProjectileRef(Projectile pr)
        {
            if (pr.type < ProjectileID.Count)
                return new ProjectileRef(pr.type);

            ProjectileDef d;
            if (Handler.ProjDef.DefsByType.TryGetValue(pr.type, out d))
                return d;

            throw new InvalidOperationException("Projectile '" + pr + "' (" + pr.type + ") is not in the def database.");
        }
    }
}
