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

namespace Prism
{
    public sealed class TMain : Main
    {
        readonly static Color TraceBgColour = new Color(0, 43, 54, 175);
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

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

                DrawTrace(spriteBatch, PrismDebug.lines);

                spriteBatch.End();

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
