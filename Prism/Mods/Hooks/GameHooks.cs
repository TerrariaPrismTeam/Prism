using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Terraria;

namespace Prism.Mods.Hooks
{
    class GameHooks : IHookManager
    {
        IEnumerable<Action> preUpdate, postUpdate;
        IEnumerable<Action<Ref<KeyValuePair<ObjectRef, BgmEntry>>>> updateMusic;

        public void Create()
        {
            var gbs = ModData.mods.Values.Select(m => m.gameBehaviour);

            preUpdate   = HookManager.CreateHooks<GameBehaviour, Action>(gbs, "PreUpdate");
            postUpdate  = HookManager.CreateHooks<GameBehaviour, Action>(gbs, "PostUpdate");

            updateMusic = HookManager.CreateHooks<GameBehaviour, Action<Ref<KeyValuePair<ObjectRef, BgmEntry>>>>(gbs, "UpdateMusic");
        }
        public void Clear ()
        {
            preUpdate   = null;
            postUpdate  = null;

            updateMusic = null;
        }

        public void PreUpdate ()
        {
            HookManager.Call(preUpdate );
        }
        public void PostUpdate()
        {
            HookManager.Call(postUpdate);
        }

        public void UpdateMusic(ref KeyValuePair<ObjectRef, BgmEntry> kvp)
        {
            var kvp_ = new Ref<KeyValuePair<ObjectRef, BgmEntry>>(kvp);
            HookManager.Call(updateMusic, kvp_);
            kvp = kvp_.Value;
        }
    }
}
