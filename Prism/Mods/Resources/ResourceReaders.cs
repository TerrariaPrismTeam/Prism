using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using LitJson;
using Prism.Util;

namespace Prism.Mods.Resources
{
    class Texture2DResourceReader : ResourceReader<Texture2D>
    {
        protected override Texture2D ReadTypedResource(Stream resourceStream)
        {
            return Texture2D.FromStream(Main.instance.GraphicsDevice, resourceStream);
        }
    }
    class SoundEffectResourceReader : ResourceReader<SoundEffect>
    {
        protected override SoundEffect ReadTypedResource(Stream resourceStream)
        {
            return SoundEffect.FromStream(resourceStream);
        }
    }
    class StringResourceReader : ResourceReader<string>
    {
        protected override string ReadTypedResource(Stream resourceStream)
        {
            // .__.
            using (var ms = new MemoryStream((int)resourceStream.Length))
            {
                resourceStream.CopyTo(ms);
                ms.Position = 0;

                using (var sr = new StreamReader(ms, true)) // using resourceStream directly will close the stream at the end of the method
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
    class JsonDataResourceReader : ResourceReader<JsonData>
    {
        protected override JsonData ReadTypedResource(Stream resourceStream)
        {
            return JsonMapper.ToObject((string)ResourceLoader.ResourceReaders[typeof(string)].ReadResource(resourceStream));
        }
    }
    class SoundBankResouceReader : ResourceReader<SoundBank>
    {
        protected override SoundBank ReadTypedResource(Stream resourceStream)
        {
            using (var tmp = new RandTempFile(resourceStream, extension: ".xsb", contentFate: StreamFate.DoNothing))
            {
                return new SoundBank(Main.engine, tmp.FilePath);
            }
        }
    }
    class WaveBankResourceReader : ResourceReader<WaveBank>
    {
        protected override WaveBank ReadTypedResource(Stream resourceStream)
        {
            using (var tmp = new RandTempFile(resourceStream, extension: ".xwb", contentFate: StreamFate.DoNothing))
            {
                return new WaveBank(Main.engine, tmp.FilePath);
            }
        }
    }
    class EffectResourceReader : ContentResourceReader<Effect> { }
}
