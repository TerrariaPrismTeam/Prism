using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Prism.Mods.Resources
{
    class Texture2DResourceReader : ResourceReader<Texture2D>
    {
        protected override Texture2D ReadTypedResource(Stream resourceStream)
        {
            return Texture2D.FromStream(PrismApi.MainInstance.GraphicsDevice, resourceStream);
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

                using (var sr = new StreamReader(ms, true)) // using resourceStream directly will close the stream at the end of the method
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
