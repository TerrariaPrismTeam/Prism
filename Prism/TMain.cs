using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.Debugging;
using Prism.Mods;
using Prism.Mods.Defs;
using Prism.Mods.Hooks;
using Prism.Util;
using Terraria;
using Terraria.ID;
using System.Diagnostics;

namespace Prism
{
    //internal enum TraceWinGrab
    //{
    //    None,
    //    Move,
    //    Scale
    //}

    public sealed class TMain : Main
    {
        readonly static Color TraceWindowBg = new Color(7, 54, 66, 255);
        readonly static Color TraceBgColour = new Color(88, 110, 117, 255);
        readonly static Color TraceBgColourAlt = new Color(131, 148, 150, 255);
        readonly static Color TraceText = new Color(253, 246, 227, 255);
        //static TraceWinGrab CurWinGrab = TraceWinGrab.None;
        static Rectangle OuterWindow = Rectangle.Empty;

        static Rectangle InnerWindow
        {
            get
            {
                return new Rectangle(OuterWindow.X + 2, OuterWindow.Y + 32, OuterWindow.Width - 4, OuterWindow.Height - 34); ;
            }
        }
        readonly static int TraceMsgPadding = 4;
        static float TraceScroll = 0;
        static Texture2D WhitePixel;

        static bool justDrawCrashed = false;
        static Exception lastDrawExn = null;

        internal TMain()
            : base()
        {
            versionNumber += ", Prism v" + PrismApi.Version;
            if (PrismApi.VersionType != VersionType.Normal)
                versionNumber += " " + PrismApi.VersionType;

            SavePath += "\\Prism";

            PlayerPath = SavePath + "\\Players";
            WorldPath  = SavePath + "\\Worlds" ;

            PrismApi.ModDirectory = SavePath + "\\Mods";

            CloudPlayerPath = "players_Prism";
            CloudWorldPath  = "worlds_Prism" ;
        }

        protected override void Initialize()
        {
            Item.OnSetDefaults += ItemDefHandler.OnSetDefaults;
            NPC .OnSetDefaults += NpcDefHandler .OnSetDefaults;

            base.Initialize(); // terraria init and LoadContent happen here

            ModLoader.Load();

            versionNumber += ", mods loaded: " + ModData.Mods.Count;
        }

        protected override void   LoadContent()
        {
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            base.  LoadContent();
        }
        protected override void UnloadContent()
        {
            ModLoader.Unload();

            WhitePixel.Dispose();
            WhitePixel = null;

            base.UnloadContent();
        }

        //public static void UpdateTraceWindowInput()
        //{
        //    if (Main.mouseLeft)
        //    {
        //        if (Main.mouseLeftRelease)
        //        {
        //            Point mScrn = Main.MouseScreen.ToPoint();
        //            if (OuterWindow.Contains(mScrn) && !InnerWindow.Contains(mScrn))
        //            {
        //                if (mScrn.Y < InnerWindow.Y && mScrn.Y > OuterWindow.Y)
        //                {
        //                    CurWinGrab = TraceWinGrab.Move;
        //                }
        //                else if (mScrn.Y > OuterWindow.Bottom - 12 && mScrn.X > OuterWindow.Right - 12)
        //                {
        //                    CurWinGrab = TraceWinGrab.Scale;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (CurWinGrab == TraceWinGrab.Move)
        //            {
        //                OuterWindow.Location = new Point(OuterWindow.Location.X + (Main.mouseX - Main.lastMouseX), OuterWindow.Location.Y + (Main.mouseY - Main.lastMouseY));
        //            }
        //            else if (CurWinGrab == TraceWinGrab.Scale)
        //            {
        //                OuterWindow.Width += (Main.mouseX - Main.lastMouseX);
        //                OuterWindow.Height += (Main.mouseY - Main.lastMouseY);
        //            }
        //        }
        //    }
        //    else if (Main.mouseLeftRelease)
        //    {
        //        CurWinGrab = TraceWinGrab.None;
        //    }
        //}

        static void DrawTrace(SpriteBatch sb, IEnumerable<TraceLine> lines)
        {
            if (lines.IsEmpty())
            {
                sb.End();
                return;
            }

            OuterWindow = new Rectangle(PrismDebug.PADDING_X, (Main.screenHeight * 2 / 3) + (PrismDebug.PADDING_Y), Main.screenWidth - (PrismDebug.PADDING_X * 2), (Main.screenHeight / 3) - (2 * PrismDebug.PADDING_Y));

            sb.Draw(WhitePixel, OuterWindow, TraceWindowBg);

            sb.DrawString(fontMouseText, "Prism Debug Tracer", OuterWindow.TopLeft() + new Vector2(2, 16), TraceText, 0f, new Vector2(0, fontItemStack.LineSpacing / 2f), Vector2.One, SpriteEffects.None, 0f);

            sb.End();

            Viewport entireScreen = sb.GraphicsDevice.Viewport;

            try
            {                
                sb.GraphicsDevice.Viewport = new Viewport(InnerWindow);
            }
            catch
            {
                sb.GraphicsDevice.Viewport = entireScreen;
            }

            sb.Begin();

            float curY = 0;

            int altBg = 0;

            List<string> drawText = new List<string>();
            List<float> fadeAlphas = new List<float>();

            foreach (var l in lines.Take(lines.Count() - 1))
            {
                StringBuilder curLine = new StringBuilder();

                float curW = 0;
                string[] spl = l.Text.Split(new char[] { ' ' });

                for (int i = 0; i < spl.Length; i++)
                {
                    string append = ((i > 0) ? " " : "") + spl[i];
                    curW += fontItemStack.MeasureString(append).X;
                    if (curW >= InnerWindow.Width)
                    {
                        curLine.AppendLine();
                        append = append.TrimStart();
                        curW = fontItemStack.MeasureString(append).X;
                    }
                    curLine.Append(append);
                }

                var size = fontItemStack.MeasureString(curLine);
                fadeAlphas.Add(MathHelper.Clamp((l.Timeleft / 30f), 0, 1));
                drawText.Add(curLine.ToString());
                curY += size.Y + TraceMsgPadding;
            }

            TraceScroll = MathHelper.Clamp(curY - InnerWindow.Height, 0, float.PositiveInfinity);
            curY = 0;

            for (int i = 0; i < drawText.Count; i++)
            {
                var size = fontItemStack.MeasureString(drawText[i]);

                sb.Draw(WhitePixel, new Vector2(0, curY - TraceScroll), null, ((++altBg % 2 == 0) ? TraceBgColour : TraceBgColourAlt) * fadeAlphas[i], 0f, Vector2.Zero, new Vector2(InnerWindow.Width, size.Y + TraceMsgPadding), SpriteEffects.None, 0f);
                sb.DrawString(fontItemStack, drawText[i], new Vector2(0, curY - TraceScroll), TraceText * fadeAlphas[i], 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

                curY += size.Y + TraceMsgPadding;
            }            

            sb.End();

            sb.GraphicsDevice.Viewport = entireScreen;
        }

        protected override void Update(GameTime gt)
        {
            try
            {                
                base.Update(gt);

                HookManager.ModDef.PostUpdate();

                PrismDebug.Update();
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

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null);

                DrawTrace(spriteBatch, PrismDebug.lines);

                justDrawCrashed = false;
                lastDrawExn = null;
            }
            catch (Exception e)
            {
                if (justDrawCrashed)
                    ExceptionHandler.HandleFatal(new AggregateException(lastDrawExn, e)); // drawing state got fucked up
                else
                {
                    justDrawCrashed = true;

                    ExceptionHandler.Handle(lastDrawExn = e);
                }
            }
        }
    }
}
