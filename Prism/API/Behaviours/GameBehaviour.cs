using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Audio;
using Prism.Mods.Hooks;
using Terraria;
using Microsoft.Xna.Framework;

namespace Prism.API.Behaviours
{
    public abstract class GameBehaviour : IOBehaviour
    {
        /// <summary>
        /// A hook called at the beginning of the game's Update method.
        /// </summary>
        [Hook]
        public virtual void PreUpdate() { }
        /// <summary>
        /// A hook called at the end of the game's Update method.
        /// </summary>
        [Hook]
        public virtual void PostUpdate() { }
        /// <summary>
        /// A hook called which checks if chat is allowed (returns false in singleplayer and true in multiplayer)
        /// </summary>
        [Hook]
        public virtual bool IsChatAllowed() { return Main.netMode > 0; }

        /// <summary>
        /// A hook called when a player types something into the chat box during single player (not normally allowed; see GameBehaviour.IsChatAllowed().)
        /// </summary>
        [Hook]
        public virtual bool OnLocalChat() { return true; }

        /// <summary>
        /// A hook called right after the game updates Main.keyState
        /// </summary>
        [Hook]
        public virtual void OnUpdateKeyboard() { }

        /// <summary>
        /// A hook called after the graphics device has been cleared, but before one frame of the game will be drawn.
        /// </summary>
        [Hook]
        public virtual void PreDraw (SpriteBatch sb) { }
        /// <summary>
        /// A hook called after one frame of the game has been drawn.
        /// </summary>
        [Hook]
        public virtual void PostDraw(SpriteBatch sb) { }
        /// <summary>
        /// Called in the game's Draw method, before the graphics device will be cleared.
        /// </summary>
        [Hook]
        public virtual void PreScreenClear() { }
        /// <summary>
        /// Called in the game's Draw method, after the graphics device has been cleared.
        /// </summary>
        [Hook]
        public virtual void PostScreenClear() { }
        /// <summary>
        /// Called in the game's Draw method, before the background will be drawn.
        /// </summary>
        /// <returns>True whenever vanilla can draw its backgrounds.</returns>
        [Hook]
        public virtual bool PreDrawBackground(SpriteBatch sb) { return true; }
        /// <summary>
        /// Called in the game's Draw method, after the background has been drawn.
        /// </summary>
        [Hook]
        public virtual void PostDrawBackground(SpriteBatch sb) { }

        /// <summary>
        /// A hook used to change the current music last-minute.
        /// </summary>
        /// <param name="current">The inner value can be changed.</param>
        [Hook]
        public virtual void UpdateMusic(Ref<BgmEntry> current) { }

        /// <summary>
        /// Remember that this will only work on this dev build. Be sure to remove this override (or comment it out) and retarget to the release build before releasing the mod.
        /// </summary>
        [Hook, Obsolete("WARNING: This only works for Prism DevBuilds!")]
        public virtual void UpdateDebug(GameTime gt) { }
    }
}
