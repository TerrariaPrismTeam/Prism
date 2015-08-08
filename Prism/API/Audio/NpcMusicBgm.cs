using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Prism.API.Audio
{
    class NpcMusicBgm : IBgm
    {
        IBgm Real
        {
            get
            {
                return Bgm.Entries[Bgm.bossMusic_custom].Music;
            }
        }

        public SoundState State
        {
            get
            {
                return Real.State;
            }
        }
        public float Volume
        {
            get
            {
                return Real.Volume;
            }
            set
            {
                Real.Volume = value;
            }
        }

        public void Play ()
        {
            Real.Play();
        }
        public void Pause()
        {
            Real.Pause();
        }
        public void Stop ()
        {
            Real.Stop();
        }

        public void Dispose()
        {
            Real.Dispose();
        }
    }
}
