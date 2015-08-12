using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Defs;
using Prism.Mods;

namespace Prism.API.Audio
{
    public class BgmRef : EntityRef<BgmEntry>
    {
        public BgmRef(ObjectRef objRef)
            : base(objRef, Assembly.GetCallingAssembly())
        {

        }
        public BgmRef(string resourceName, ModInfo mod)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {

        }
        public BgmRef(string resourceName, string modName = null)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {

        }

        public override BgmEntry Resolve()
        {
            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.BgmEntries.ContainsKey(ResourceName))
                return Requesting.BgmEntries[ResourceName];

            if (IsVanillaRef)
            {
                if (!Bgm.VanillaDict.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla BGM entry reference '" + ResourceName + "' is not found.");

                return Bgm.VanillaDict[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("BGM entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].BgmEntries.ContainsKey(ResourceName))
                throw new InvalidOperationException("BGM entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the BGM entry is not loaded.");

            return ModData.ModsFromInternalName[ModName].BgmEntries[ResourceName];
        }
    }
}
