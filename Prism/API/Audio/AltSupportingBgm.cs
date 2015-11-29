using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Terraria;

namespace Prism.API.Audio
{
    public enum SwitchMode
    {
        Random,
        Alternate
    }

    public class AltSupportingBgm : IBgm
    {
        IBgm current;

        public virtual SwitchMode SwitchMode
        {
            get;
            protected set;
        }

        public virtual IBgm Regular
        {
            get;
            protected set;
        }
        public virtual IBgm Alt
        {
            get;
            protected set;
        }

        public float Volume
        {
            get
            {
                return current.Volume;
            }
            set
            {
                current.Volume = value;
            }
        }
        public SoundState State
        {
            get
            {
                return current.State;
            }
        }

        public AltSupportingBgm(IBgm regular, IBgm alternate, SwitchMode switchMode)
        {
#pragma warning disable RECS0021
            Regular = regular;
            Alt = alternate;

            SwitchMode = switchMode;

            current = Regular;
#pragma warning restore RECS0021
        }

        public void Play ()
        {
            if (current == null || current.State == SoundState.Stopped)
                switch (SwitchMode)
                {
                    case SwitchMode.Random:
                        current = Main.rand.Next(2) == 0 ? Regular : Alt;
                        break;
                    case SwitchMode.Alternate:
                        current = current == null || current == Alt ? Regular : Alt;
                        break;
                }

            current.Play();
        }
        public void Pause()
        {
            current.Pause();
        }
        public void Stop ()
        {
            current.Stop();
        }

        public void Dispose()
        {
            current.Dispose();
        }
    }
}
