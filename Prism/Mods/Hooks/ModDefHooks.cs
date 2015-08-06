using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Audio;
using Terraria;

namespace Prism.Mods.Hooks
{
    class ModDefHooks : IHookManager
    {
        IEnumerable<Action>
            onAllModsLoaded,
            onUnload       ,
            preUpdate      ,
            postUpdate     ;
        IEnumerable<Action<Ref<KeyValuePair<string, BgmEntry>>>> updateMusic;

        public void Create()
        {
            onAllModsLoaded = HookManager.CreateHooks<ModDef, Action>(ModData.mods.Values, "OnAllModsLoaded");
            onUnload        = HookManager.CreateHooks<ModDef, Action>(ModData.mods.Values, "OnUnload"       );
            preUpdate       = HookManager.CreateHooks<ModDef, Action>(ModData.mods.Values, "PreUpdate"      );
            postUpdate      = HookManager.CreateHooks<ModDef, Action>(ModData.mods.Values, "PostUpdate"     );

            updateMusic     = HookManager.CreateHooks<ModDef, Action<Ref<KeyValuePair<string, BgmEntry>>>>(ModData.mods.Values, "UpdateMusic");
        }
        public void Clear ()
        {
            onAllModsLoaded = null;
            onUnload        = null;
            preUpdate       = null;
            postUpdate      = null;
            updateMusic     = null;
        }

        public void OnAllModsLoaded()
        {
            HookManager.Call(onAllModsLoaded);
        }
        public void OnUnload()
        {
            HookManager.Call(onUnload);
        }
        public void PreUpdate()
        {
            HookManager.Call(preUpdate);
        }
        public void PostUpdate()
        {
            HookManager.Call(postUpdate);
        }

        public void UpdateMusic(ref KeyValuePair<string, BgmEntry> current)
        {
            var r = new Ref<KeyValuePair<string, BgmEntry>>(current);
            HookManager.Call(updateMusic, r);
            current = r.Value;
        }
    }
}
