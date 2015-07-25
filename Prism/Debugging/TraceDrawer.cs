using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Util;
using Terraria;

namespace Prism.Debugging
{
    static class TraceDrawer
    {
        readonly static Color TraceWindowBg    = new Color(  7,  54,  66, 255);
        readonly static Color TraceBgColour    = new Color( 88, 110, 117, 255);
        readonly static Color TraceBgColourAlt = new Color(131, 148, 150, 255);
        readonly static Color TraceText        = new Color(253, 246, 227, 255);

        readonly static char[] SpaceArray = { ' ' };
        readonly static string Space = " ";

        readonly static int TraceMsgPadding = 4;

        static float TraceScroll = 0f;
        static Rectangle OuterWindow = Rectangle.Empty;
        static Rectangle InnerWindow
        {
            get
            {
                return new Rectangle(OuterWindow.X + 2, OuterWindow.Y + 32, OuterWindow.Width - 4, OuterWindow.Height - 34);
            }
        }

        internal static void DrawTrace(SpriteBatch sb, IEnumerable<TraceLine> lines)
        {
            //TODO: clean this up, in some way or another
            if (lines.IsEmpty())
                return;

            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null);

            OuterWindow = new Rectangle(PrismDebug.PADDING_X, Main.screenHeight * 2 / 3 + PrismDebug.PADDING_Y, Main.screenWidth - PrismDebug.PADDING_X * 2, Main.screenHeight / 3 - 2 * PrismDebug.PADDING_Y);

            sb.Draw(TMain.WhitePixel, OuterWindow, TraceWindowBg);

            sb.DrawString(Main.fontMouseText, "Prism Debug Tracer", OuterWindow.TopLeft() + new Vector2(2, 16), TraceText, 0f, new Vector2(0, Main.fontItemStack.LineSpacing / 2f), Vector2.One, SpriteEffects.None, 0f);

            sb.End();

            Viewport screen = sb.GraphicsDevice.Viewport;

            try
            {
                sb.GraphicsDevice.Viewport = new Viewport(InnerWindow);
            }
            catch
            {
                sb.GraphicsDevice.Viewport = screen;
            }

            sb.Begin();

            float curY = 0;
            bool altBg = false;

            var lineAmt = lines.Count();
            List<string> drawText   = new List<string>(lineAmt - 1);
            List<float > fadeAlphas = new List<float >(lineAmt - 1);

            var curLine = new StringBuilder();

            foreach (var l in lines.Take(lineAmt - 1))
            {
                float curW = 0f;
                string[] words = l.Text.Split(SpaceArray);

                for (int i = 0; i < words.Length; i++)
                {
                    string append = (i > 0 ? Space : String.Empty) + words[i];
                    curW += Main.fontItemStack.MeasureString(append).X;

                    if (curW >= InnerWindow.Width)
                    {
                        curLine.AppendLine();
                        append = append.TrimStart();
                        curW = Main.fontItemStack.MeasureString(append).X;
                    }
                    curLine.Append(append);
                }

                fadeAlphas.Add(MathHelper.Clamp(l.Timeleft / 30f, 0f, 1f));
                drawText.Add(curLine.ToString());
                curY += Main.fontItemStack.MeasureString(curLine).Y + TraceMsgPadding;

                curLine.Clear();
            }

            TraceScroll = MathHelper.Clamp(curY - InnerWindow.Height, 0, Single.PositiveInfinity);
            curY = 0f;

            for (int i = 0; i < drawText.Count; i++)
            {
                var size = Main.fontItemStack.MeasureString(drawText[i]);

                sb.Draw(TMain.WhitePixel, new Vector2(0, curY - TraceScroll), null, (altBg ? TraceBgColour : TraceBgColourAlt) * fadeAlphas[i], 0f, Vector2.Zero, new Vector2(InnerWindow.Width, size.Y + TraceMsgPadding), SpriteEffects.None, 0f);
                altBg = !altBg;

                sb.DrawString(Main.fontItemStack, drawText[i], new Vector2(0, curY - TraceScroll), TraceText * fadeAlphas[i], 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

                curY += size.Y + TraceMsgPadding;
            }

            sb.End();

            sb.GraphicsDevice.Viewport = screen;
        }
    }
}
