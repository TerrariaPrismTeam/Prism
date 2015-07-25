using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.Debugging;
using Prism.Mods;
using Prism.Mods.Defs;
using Prism.Mods.Hooks;
using Prism.Util;
using Terraria;

namespace Prism
{
    public sealed class TMain : Main
    {
        internal static Texture2D WhitePixel;

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

                TraceDrawer.DrawTrace(spriteBatch, PrismDebug.lines);

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
