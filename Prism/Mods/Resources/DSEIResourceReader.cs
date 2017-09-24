using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Prism.Util;

namespace Prism.Mods.Resources
{
    // reads a wave file in chuncks and puts it in a DynamicSoundEffectInstance
    // uses less memory, because it's streamed
    class DSEIResourceReader : ResourceReader<DynamicSoundEffectInstance>
    {
        protected override bool ResetStream
        {
            get
            {
                return false;
            }
        }

        protected override unsafe DynamicSoundEffectInstance ReadTypedResource(Stream resourceStream)
        {
            // NOT in a using!
            var r = new BinaryReader(resourceStream);

            WaveHeader hdr;

            byte[] data = r.ReadBytes(sizeof(WaveHeader));
            fixed (byte* pd = data)
            {
                hdr = *(WaveHeader*)pd;
            }

            if (hdr.riff.magic != WaveMagic.RIFF || hdr.wavemagic != WaveMagic.WAVE)
                throw new FileFormatException("Bogus RIFF or WAVE magic bytes");

            uint left = hdr.riff.size - (uint)sizeof(WaveHeader);

            WaveFmt fmt = default(WaveFmt);
            Riff datah = default(Riff);

            for (Riff cur; ; )
            {
                data = r.ReadBytes(sizeof(Riff));
                fixed (byte* pd = data)
                {
                    cur = *(Riff*)pd;
                }

                if (cur.magic == WaveMagic.data)
                {
                    datah = cur;
                    break;
                }

                if (cur.magic == WaveMagic.fmt_)
                {
                    data = r.ReadBytes(sizeof(WaveMagic));
                    fixed (byte* pd = data)
                    {
                        fmt = *(WaveFmt*)pd;
                    }
                }
            }

            if (fmt.format != WaveFormat.PCM)
                throw new NotSupportedException("Non-PCM WAV files aren't supported");

            if (fmt.byterate == 0)
                fmt.byterate = (ushort)(fmt.samplerate * fmt.channels * (fmt.bitdepth >> 3));

            var dsei = new DynamicSoundEffectInstance(fmt.samplerate, (AudioChannels)fmt.channels);

            Action submit = () =>
            {
                if (datah.size == 0)
                    return;

                uint rr = Math.Min((uint)fmt.byterate, datah.size);
                dsei.SubmitBuffer(r.ReadBytes((int)rr));

                datah.size -= rr;
            };

            submit();
            dsei.BufferNeeded += (_, __) => submit();

            return dsei;
        }
    }
}
