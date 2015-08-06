using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Prism.API.Audio
{
    public interface IBgm : IDisposable
    {
        float Volume
        {
            get;
            set;
        }
        SoundState State
        {
            get;
        }

        void Play ();
        void Pause();
        void Stop ();
    }

    public class XactBgm : IBgm
    {
        bool disposed = false;

        float vol;
        string cueName;

        SoundBank sb;
        WaveBank wb;

        Cue cue;

        public float Volume
        {
            get
            {
                return vol;
            }
            set
            {
                cue.SetVariable("Volume", vol = value);
            }
        }
        public SoundState State
        {
            get
            {
                if (cue.IsPaused)
                    return SoundState.Paused;
                else /* DO NOT REMOVE THE ELSE, STUFF WILL BREAK OTHERWISE */ if (cue.IsPlaying)
                    return SoundState.Playing;

                return SoundState.Stopped;
            }
        }

        public XactBgm(SoundBank soundBank, WaveBank waveBank, string cueName)
        {
            sb = soundBank;
            wb = waveBank ;

            this.cueName = cueName;

            cue = sb.GetCue(cueName);
        }
        ~XactBgm()
        {
            Dispose(false);
        }

        public void Play ()
        {
            if (cue.IsPaused)
            {
                cue.Resume();
                return;
            }

            if (cue.IsPlaying)
                return;

            if (cue.IsStopped)
            {
                cue = sb.GetCue(cueName);
                Volume = vol;
            }

            cue.Play();
        }
        public void Pause()
        {
            if (cue.IsPaused)
                return;

            cue.Pause();
        }
        public void Stop ()
        {
            if (cue.IsStopped)
                return;

            cue.Stop(AudioStopOptions.Immediate);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    sb = null;
                    wb = null;
                    cueName = null;
                }

                cue.Dispose();
                cue = null;

                disposed = true;
            }
        }
    }
    public class XAudioBgm : IBgm
    {
        bool disposed = false;

        SoundEffectInstance sei;

        public SoundState State
        {
            get
            {
                return sei.State;
            }
        }
        public float Volume
        {
            get
            {
                return sei.Volume;
            }
            set
            {
                sei.Volume = value;
            }
        }

        public XAudioBgm(SoundEffectInstance instance)
        {
            sei = instance;
        }
        public XAudioBgm(SoundEffect         effect  )
            : this(effect.CreateInstance())
        {

        }
        ~XAudioBgm()
        {
            Dispose(false);
        }

        public void Pause()
        {
            if (sei.State == SoundState.Paused)
                return;

            sei.Pause();
        }
        public void Play ()
        {
            if (sei.State == SoundState.Playing)
                return;

            sei.Play();
        }
        public void Stop ()
        {
            if (sei.State == SoundState.Stopped)
                return;

            sei.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                sei.Dispose();

                disposed = true;
            }
        }
    }
}
