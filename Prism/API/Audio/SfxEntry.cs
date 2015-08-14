using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Prism.API.Audio
{
    public enum SfxPlayBehaviour
    {
        /// <summary>
        /// Replay the sound effect if already playing.
        /// </summary>
        Singleton,
        /// <summary>
        /// Allow multiple instances of the sound effect be played at the same time.
        /// </summary>
        MultipleInstances,
        /// <summary>
        /// Only play the sound effect when it currently isn't.
        /// </summary>
        PlayIfStopped,
        /// <summary>
        /// Only play the sound effect when it currently isn't. Parameters (volume, pan, pitch, ..) will be updated.
        /// </summary>
        PlayIfStoppedUpdateParams
    }
    public class SfxEntry : AudioEntry<SfxEntry, SfxRef>
    {
        public virtual Func<int, SfxPlayBehaviour> PlayBehaviour
        {
            get;
            private set;
        }
        public virtual Func<int, SoundEffectInstance> GetInstance
        {
            get;
            private set;
        }
        public virtual int Variants
        {
            get;
            private set;
        }
        public virtual bool IsAmbient
        {
            get;
            private set;
        }

        public SfxEntry(Func<int, SoundEffectInstance> getInstance, int variants, Func<int, SfxPlayBehaviour> behaviour, bool ambient = false)
        {
            PlayBehaviour = behaviour;
            Variants = variants;
            GetInstance = getInstance;
            IsAmbient = ambient;
        }
        public SfxEntry(Func<SoundEffectInstance> getInstance, SfxPlayBehaviour behaviour, bool ambient = false)
            : this(_ => getInstance(), 1, _ => behaviour, ambient)
        {

        }

        public static implicit operator SfxRef(SfxEntry e)
        {
            return new SfxRef(e.InternalName, e.Mod);
        }
    }
}
