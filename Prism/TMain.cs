using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Debugging;
using Terraria;

namespace Prism
{
    public sealed class TMain : Main
    {
        readonly static Color TraceBgColour = new Color(0, 43, 54, 175);
        static Texture2D WhitePixel;

        internal TMain()
            : base()
        {
            versionNumber += ", Prism v" + AssemblyInfo.VERSION;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void   LoadContent()
        {
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            base.  LoadContent();
        }
        protected override void UnloadContent()
        {
            WhitePixel.Dispose();
            WhitePixel = null;

            base.UnloadContent();
        }

        static void DrawTrace(SpriteBatch sb, IEnumerable<TraceLine> lines)
        {
            if (lines.IsEmpty())
                return;

            var b = new StringBuilder();

            foreach (var l in lines.Take(lines.Count() - 1))
                b.AppendLine(l.Text);

            b.Append(lines.Last().Text);

            var size = fontMouseText.MeasureString(b);
            sb.Draw(WhitePixel, Vector2.Zero, null, TraceBgColour, 0f, Vector2.Zero, new Vector2(screenWidth, size.Y), SpriteEffects.None, 0f);
            sb.DrawString(fontMouseText, b, Vector2.Zero, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }

        protected override void Update(GameTime gt)
        {
            try
            {
                base.Update(gt);

                PrismDebug.Update();

                if (mouseLeft && mouseLeftRelease)
                    Trace.WriteLine("mouse click");
            }
            catch (Exception e)
            {
                ExceptionHandler.Handle(e);
            }
        }
        protected override void Draw  (GameTime gt)
        {
            try
            {
                base.Draw(gt);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

                DrawTrace(spriteBatch, PrismDebug.lines);

                spriteBatch.End();
            }
            catch (Exception e)
            {
                ExceptionHandler.Handle(e);
            }
        }
    }
}
