using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Terraria;
using Microsoft.Xna.Framework;

namespace Prism.Mods.Hooks
{
    class GameHooks : IHookManager
    {
        IEnumerable<Action> preUpdate, postUpdate, onUpdateKeyboard;
        IEnumerable<Action<Ref<BgmEntry>>> updateMusic;

        public void Create()
        {
            var gbs = ModData.mods.Values.Select(m => m.gameBehaviour);

            preUpdate        = HookManager.CreateHooks<GameBehaviour, Action>(gbs, "PreUpdate"       );
            postUpdate       = HookManager.CreateHooks<GameBehaviour, Action>(gbs, "PostUpdate"      );
            onUpdateKeyboard = HookManager.CreateHooks<GameBehaviour, Action>(gbs, "OnUpdateKeyboard");

            updateMusic = HookManager.CreateHooks<GameBehaviour, Action<Ref<BgmEntry>>>(gbs, "UpdateMusic");
        }
        public void Clear ()
        {
            preUpdate        = null;
            postUpdate       = null;
            onUpdateKeyboard = null;

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

        public void OnUpdateKeyboard()
        {
            HookManager.Call(onUpdateKeyboard);
        }
    }
}
