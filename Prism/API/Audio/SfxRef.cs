using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API.Defs;
using Prism.Mods;

namespace Prism.API.Audio
{
    public class SfxRef : EntityRef<SfxEntry>
    {
        public int VariantID
        {
            get;
            internal set;
        }

        public SfxRef(ObjectRef objRef, int variant = -1)
            : base(objRef, Assembly.GetCallingAssembly())
        {
            VariantID = variant;
        }
        public SfxRef(string resourceName, ModInfo mod, int variant = -1)
            : base(new ObjectRef(resourceName, mod), Assembly.GetCallingAssembly())
        {
            VariantID = variant;
        }
        public SfxRef(string resourceName, string modName = null, int variant = -1)
            : base(new ObjectRef(resourceName, modName), Assembly.GetCallingAssembly())
        {
            VariantID = variant;
        }

        public override SfxEntry Resolve()
        {
            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.SfxEntries.ContainsKey(ResourceName))
                return Requesting.SfxEntries[ResourceName];

            if (IsVanillaRef)
            {
                if (!Sfx.VanillaDict.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla SFX entry reference '" + ResourceName + "' is not found.");

                return Sfx.VanillaDict[ResourceName];
            }

            if (!ModData.ModsFromInternalName.ContainsKey(ModName))
                throw new InvalidOperationException("SFX entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.ModsFromInternalName[ModName].SfxEntries.ContainsKey(ResourceName))
                throw new InvalidOperationException("SFX entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the BGM entry is not loaded.");

            return ModData.ModsFromInternalName[ModName].SfxEntries[ResourceName];
        }
    }
}
