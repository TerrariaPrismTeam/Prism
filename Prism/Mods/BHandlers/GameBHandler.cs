using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.Mods.BHandlers
{
    sealed class GameBHandler : IOBHandler<GameBehaviour>
    {
        IEnumerable<Action>
            preUpdate, postUpdate, onUpdateKeyboard,
            preScreenClear, postScreenClear;
        IEnumerable<Action<SpriteBatch>> preDraw, postDraw, postDrawBackground;
        IEnumerable<Func<SpriteBatch, bool>> preDrawBackground;
        IEnumerable<Action<Ref<BgmEntry>>> updateMusic;
        IEnumerable<Func<bool>> onLocalChat, isChatAllowed;
        IEnumerable<Action<GameTime>> updateDebug;

        public override void Create()
        {
            behaviours = new List<GameBehaviour>(ModData.mods.Values.Select(m => m.gameBehaviour).Where(gb => gb != null));

            base.Create();

            preUpdate        = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PreUpdate"       );
            postUpdate       = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PostUpdate"      );
            onUpdateKeyboard = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "OnUpdateKeyboard");

            preDraw  = HookManager.CreateHooks<GameBehaviour, Action<SpriteBatch>>(behaviours, "PreDraw" );
            postDraw = HookManager.CreateHooks<GameBehaviour, Action<SpriteBatch>>(behaviours, "PostDraw");

            preScreenClear  = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PreScreenClear" );
            postScreenClear = HookManager.CreateHooks<GameBehaviour, Action>(behaviours, "PostScreenClear");

            preDrawBackground  = HookManager.CreateHooks<GameBehaviour, Func  <SpriteBatch, bool>>(behaviours, "PreDrawBackground" );
            postDrawBackground = HookManager.CreateHooks<GameBehaviour, Action<SpriteBatch      >>(behaviours, "PostDrawBackground");

            updateDebug = HookManager.CreateHooks<GameBehaviour, Action<GameTime>>(behaviours, "UpdateDebug");

            updateMusic = HookManager.CreateHooks<GameBehaviour, Action<Ref<BgmEntry>>>(behaviours, "UpdateMusic");

            onLocalChat = HookManager.CreateHooks<GameBehaviour, Func<bool>>(behaviours, "OnLocalChat");

            isChatAllowed = HookManager.CreateHooks<GameBehaviour, Func<bool>>(behaviours, "IsChatAllowed");
        }
        public override void Clear ()
        {
            base.Clear();

            preUpdate        = null;
            postUpdate       = null;
            onUpdateKeyboard = null;

            preDraw            = null;
            postDraw           = null;
            preScreenClear     = null;
            postScreenClear    = null;
            preDrawBackground  = null;
            postDrawBackground = null;

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

        public void PreScreenClear ()
        {
            HookManager.Call(preScreenClear);
        }
        public void PostScreenClear()
        {
            HookManager.Call(postScreenClear);
        }

        public bool PreDrawBackground (SpriteBatch sb)
        {
            var r = HookManager.Call(preDrawBackground, sb);
            return r.Length == 0 || r.All(Convert.ToBoolean);
        }
        public void PostDrawBackground(SpriteBatch sb)
        {
            HookManager.Call(postDrawBackground, sb);
        }

        [Conditional("DEV_BUILD")]
        public void UpdateDebug(GameTime gt)
        {
            if (PrismApi.VersionType == VersionType.DevBuild)
                HookManager.Call(updateDebug, gt);
        }

        public void UpdateMusic(ref BgmEntry e)
        {
            var pe = new Ref<BgmEntry>(e);
            HookManager.Call(updateMusic, pe);
            e = pe.Value;
        }

        public bool OnLocalChat()
        {
            var r = HookManager.Call(onLocalChat);
            return r.Length == 0 || r.Any(Convert.ToBoolean);
        }

        public bool IsChatAllowed()
        {
            var r = HookManager.Call(isChatAllowed);
            return r.Length == 0 || r.All(Convert.ToBoolean);
        }
    }
}
