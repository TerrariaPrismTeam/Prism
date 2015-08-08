using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

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
                    return new Color(TMain.DiscoR, TMain.DiscoG, TMain.DiscoB);
                }
            }

            public static void RandColorText(string newText, bool force = false)
            {
                TMain.NewText(newText, (byte)TMain.DiscoR, (byte)TMain.DiscoG, (byte)TMain.DiscoB, force);
            }
        }

        public static void Update(GameTime gt)
        {

        }
    }
}
