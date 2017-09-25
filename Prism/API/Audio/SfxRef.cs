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
            SfxEntry r = null;

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.SfxEntries.TryGetValue(ResourceName, out r))
                return r;

            if (IsVanillaRef)
            {
                if (!Sfx.VanillaDict.TryGetValue(ResourceName, out r))
                    throw new InvalidOperationException("Vanilla SFX entry reference '" + ResourceName + "' is not found.");

                return r;
            }

            ModDef m = null;
            if (!ModData.ModsFromInternalName.TryGetValue(ModName, out m))
                throw new InvalidOperationException("SFX entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!m.SfxEntries.TryGetValue(ResourceName, out r))
                throw new InvalidOperationException("SFX entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the SFX entry is not loaded.");

            return r;
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(ResourceName))
                return "<empty>";

            return "{" + Mod.InternalName + "." + ResourceName + (VariantID < 0 ? String.Empty : (":" + VariantID)) + "}";
        }
    }
}
