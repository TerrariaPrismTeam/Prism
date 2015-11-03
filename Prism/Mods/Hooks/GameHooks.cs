using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.Mods.BHandlers;
using Terraria;

namespace Prism.Mods.Hooks
{
    sealed class GameHooks : IOBHandler<GameBehaviour>
    {
        IEnumerable<Action>
            preUpdate, postUpdate, onUpdateKeyboard,
            preScreenClear, updateDebug;
        IEnumerable<Action<SpriteBatch>> preDraw, postDraw;
        IEnumerable<Action<Ref<BgmEntry>>> updateMusic;

        public override void Create()
        {
            behaviours = new List<GameBehaviour>(ModData.mods.Values.Select(m => m.gameBehaviour));

            base.Create();

            preUpdate        = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PreUpdate"       );
            postUpdate       = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PostUpdate"      );
            onUpdateKeyboard = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "OnUpdateKeyboard");

            preDraw  = HookManager.CreateHooks<GameBehaviour, Action<SpriteBatch>>(behaviours, "PreDraw" );
            postDraw = HookManager.CreateHooks<GameBehaviour, Action<SpriteBatch>>(behaviours, "PostDraw");

            preScreenClear = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PreScreenClear");

            updateDebug = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "UpdateDebug");

            updateMusic = HookManager.CreateHooks<GameBehaviour, Action<Ref<BgmEntry>>>(behaviours, "UpdateMusic");
        }
        public override void Clear ()
        {
            base.Clear();

            preUpdate        = null;
            postUpdate       = null;
            onUpdateKeyboard = null;

            preDraw        = null;
            postDraw       = null;
            preScreenClear = null;

            updateDebug = null;

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

        public void OnUpdateKeyboard()
        {
            HookManager.Call(onUpdateKeyboard);
        }

        public void PreDraw (SpriteBatch sb)
        {
            HookManager.Call(preDraw, sb);
        }
        public void PostDraw(SpriteBatch sb)
        {
            HookManager.Call(postDraw, sb);
        }

        public void PreScreenClear()
        {
            HookManager.Call(preScreenClear);
        }

        [Conditional("DEV_BUILD")]
        public void UpdateDebug()
        {
            if (PrismApi.VersionType == VersionType.DevBuild)
                HookManager.Call(updateDebug);
        }

        public void UpdateMusic(ref BgmEntry e)
        {
            var pe = new Ref<BgmEntry>(e);
            HookManager.Call(updateMusic, pe);
            e = pe.Value;
        }
    }
}
