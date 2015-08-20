using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using T = Terraria;

namespace Prism.Util
{
    public static class Helpers
    {
        public static class Main
        {
            public static Color DiscoColor
            {
                get
                {
                    return new Color(T.Main.DiscoR, T.Main.DiscoG, T.Main.DiscoB);
                }
            }

            public static void RandColorText(string newText, bool force = false)
            {
                T.Main.NewText(newText, (byte)T.Main.DiscoR, (byte)T.Main.DiscoG, (byte)T.Main.DiscoB, force);
            }
        }

        public static void Update(GameTime gt)
        {

        }
    }
}
