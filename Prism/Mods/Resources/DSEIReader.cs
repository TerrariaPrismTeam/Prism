using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Prism.Mods.Resources
{
    // reads a wave file in chuncks and puts it in a DynamicSoundEffectInstance
    // uses less memory, because it's streamed
    class DSEIReader : ResourceReader<DynamicSoundEffectInstance>
    {
        internal readonly static string
           InvalidRiffHeader = "Invalid RIFF header.",
           InvalidWaveHeader = "Invalid WAVE fmt header.",
           InvalidWaveType = "Invalid wave file type, expected PCM.",
           InvalidDataChunck = "Invalid RIFF data chunck.";

        protected override bool ResetStream
        {
            get
            {
                return false;
            }
        }

        static void Expect<T>(T read, T expected, string message = null)
            where T : struct, IEquatable<T>
        {
            if (!read.Equals(expected))
                throw new FormatException(message ?? "Expected " + expected + ", but got " + read + ".");
        }

        protected override DynamicSoundEffectInstance ReadTypedResource(Stream resourceStream)
        {
            var r = new BinaryReader(resourceStream);

            Expect(r.ReadChar(), 'R', InvalidRiffHeader);
            Expect(r.ReadChar(), 'I', InvalidRiffHeader);
            Expect(r.ReadChar(), 'F', InvalidRiffHeader);
            Expect(r.ReadChar(), 'F', InvalidRiffHeader);

            int totalLen = r.ReadInt32();

            Expect(r.ReadChar(), 'W', InvalidWaveHeader);
            Expect(r.ReadChar(), 'A', InvalidWaveHeader);
            Expect(r.ReadChar(), 'V', InvalidWaveHeader);
            Expect(r.ReadChar(), 'E', InvalidWaveHeader);

            Expect(r.ReadChar(), 'f', InvalidWaveHeader);
            Expect(r.ReadChar(), 'm', InvalidWaveHeader);
            Expect(r.ReadChar(), 't', InvalidWaveHeader);
            Expect(r.ReadChar(), ' ', InvalidWaveHeader);

            int bitDepth = r.ReadInt32();
            Expect(r.ReadUInt16(), (ushort)1, InvalidWaveType);
            int channels = r.ReadUInt16();
            int sampleRate = r.ReadInt32();
            int bytePerSecond = r.ReadInt32();
            ushort bytePerSample = r.ReadUInt16();
            Expect(r.ReadUInt16(), (ushort)bitDepth);

            Expect(r.ReadChar(), 'd', InvalidDataChunck);
            Expect(r.ReadChar(), 'a', InvalidDataChunck);
            Expect(r.ReadChar(), 't', InvalidDataChunck);
            Expect(r.ReadChar(), 'a', InvalidDataChunck);

            int rest = r.ReadInt32();

            var dsei = new DynamicSoundEffectInstance(sampleRate, (AudioChannels)channels);

            Action SubmitSec = () =>
            {
                if (rest == 0)
                {
                    //dsei.Voice.Discontinuity();
                    return;
                }

                var toRead = Math.Min(sampleRate, rest);

                var data = r.ReadBytes(toRead);

                rest -= toRead;

                dsei.SubmitBuffer(data);

                return;
            };

            SubmitSec();
            dsei.BufferNeeded += (_, __) => SubmitSec();

            return dsei;
        }
    }
}
