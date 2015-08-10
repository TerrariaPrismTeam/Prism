using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Audio;
using Prism.Mods.Hooks;
using Terraria;

namespace Prism.API.Behaviours
{
    public abstract class GameBehaviour : HookContainer
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
        /// A hook used to change the current music last-minute.
        /// </summary>
        /// <param name="current">The inner value can be changed.</param>
        [Hook]
        public virtual void UpdateMusic(Ref<KeyValuePair<ObjectRef, BgmEntry>> current) { }

#if DEV_BUILD
        /// <summary>
        /// Remember that this will only work on this dev build. Be sure to remove this override (or comment it out) and retarget to the release build before releasing the mod.
        /// </summary>
        [Hook]
        public virtual void UpdateDebug() { }
#endif
    }
}
