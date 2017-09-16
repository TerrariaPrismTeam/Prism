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
            BgmEntry r = null;

            if (String.IsNullOrEmpty(ModName) && Requesting != null && Requesting.BgmEntries.TryGetValue(ResourceName, out r))
                return r;

            if (IsVanillaRef)
            {
                if (!Bgm.VanillaDict.TryGetValue(ResourceName, out r))
                    throw new InvalidOperationException("Vanilla BGM entry reference '" + ResourceName + "' is not found.");

                return r;
            }

            ModDef m = null;
            if (!ModData.ModsFromInternalName.TryGetValue(ModName, out m))
                throw new InvalidOperationException("BGM entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!m.BgmEntries.TryGetValue(ResourceName, out r))
                throw new InvalidOperationException("BGM entry reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the BGM entry is not loaded.");

            return r;
        }
    }
}
