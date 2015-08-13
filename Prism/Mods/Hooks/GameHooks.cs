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
        IEnumerable<Action<Ref<BgmEntry>>> updateMusic;

        public void Create()
        {
            var gbs = ModData.mods.Values.Select(m => m.gameBehaviour);

            preUpdate   = HookManager.CreateHooks<GameBehaviour, Action>(gbs, "PreUpdate" );
            postUpdate  = HookManager.CreateHooks<GameBehaviour, Action>(gbs, "PostUpdate");

            updateMusic = HookManager.CreateHooks<GameBehaviour, Action<Ref<BgmEntry>>>(gbs, "UpdateMusic");
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

        public void UpdateMusic(ref BgmEntry e)
        {
            var pe = new Ref<BgmEntry>(e);
            HookManager.Call(updateMusic, pe);
            e = pe.Value;
        }
    }
}
