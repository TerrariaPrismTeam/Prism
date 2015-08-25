using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.Mods.BHandlers;
using Terraria;

namespace Prism.Mods.Hooks
{
    sealed class GameHooks : IOBHandler<GameBehaviour>
    {
        IEnumerable<Action> preUpdate, postUpdate, onUpdateKeyboard;
        IEnumerable<Action<Ref<BgmEntry>>> updateMusic;

        public override void Create()
        {
            behaviours = new List<GameBehaviour>(ModData.mods.Values.Select(m => m.gameBehaviour));

            base.Create();

            preUpdate        = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PreUpdate"       );
            postUpdate       = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PostUpdate"      );
            onUpdateKeyboard = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "OnUpdateKeyboard");

            updateMusic = HookManager.CreateHooks<GameBehaviour, Action<Ref<BgmEntry>>>(behaviours, "UpdateMusic");
        }
        public override void Clear ()
        {
            base.Clear();

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
