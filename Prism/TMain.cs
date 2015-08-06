using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Audio;
using Prism.Debugging;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Mods.Hooks;
using Terraria;
using Prism.Util;

namespace Prism
{
    public sealed class TMain : Main
    {
        internal static Texture2D WhitePixel;

        static bool justDrawCrashed = false;
        static Exception lastDrawExn = null;

        static bool prevGameMenu = true;

        internal TMain()
            : base()
        {
            versionNumber += ", " + PrismApi.NiceVersionString;

            SavePath += "\\Prism";

            PlayerPath = SavePath + "\\Players";
            WorldPath  = SavePath + "\\Worlds" ;

            PrismApi.ModDirectory = SavePath + "\\Mods";

            CloudPlayerPath = "players_Prism";
            CloudWorldPath  = "worlds_Prism" ;
        }

        protected override void Initialize()
        {
            OnUpdateMusic += Bgm.Update;

            Item      .OnSetDefaults += ItemDefHandler.OnSetDefaults;
            NPC       .OnSetDefaults += NpcDefHandler .OnSetDefaults;
            Projectile.OnSetDefaults += ProjDefHandler.OnSetDefaults;

            base.Initialize(); // terraria init and LoadContent happen here

            EntityDefLoader.SetupEntityHandlers();
            ModLoader.Load();

#if DEV_BUILD
            ModLoader.Debug_ShowAllErrors();
#endif

            ApplyHotfixes();

            versionNumber += ", mods loaded: " + ModData.Mods.Count +
                (ModLoader.errors.Count > 0 ? ", load errors: " + ModLoader.errors.Count : "");
        }

        protected override void   LoadContent()
        {
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            ModLoader.Unload();

            WhitePixel.Dispose();
            WhitePixel = null;

            base.UnloadContent();
        }

        /// <summary>
        /// For those hooks and stuff we just don't have yet...
        /// </summary>
        void ApplyHotfixes()
        {
            foreach (Player p in from plr in player where plr.active == true select plr)
            {
                int prevLength = p.npcTypeNoAggro.Length;
                if (prevLength < Handler.NpcDef.NextTypeIndex)
                    Array.Resize(ref p.npcTypeNoAggro, Handler.NpcDef.NextTypeIndex);
            }
        }

        protected override void Update(GameTime gt)
        {
            try
            {
                HookManager.ModDef.PreUpdate();

                ApplyHotfixes(); //The array is initialized every time new Player() is called. Until we have like InitPlayer or something we just have to ghettohack it like this.

                base.Update(gt);

                if (!gameMenu && prevGameMenu)
                    Helpers.Main.RandColorText("Welcome to " + PrismApi.NiceVersionString + ".", true);

                HookManager.ModDef.PostUpdate();

                PrismDebug.Update();
            }
            catch (Exception e)
            {
                ExceptionHandler.Handle(e);
            }

            prevGameMenu = gameMenu;
        }
        protected override void Draw  (GameTime gt)
        {
            try
            {
                base.Draw(gt);
#if TRACE //logic
                TraceDrawer.DrawTrace(spriteBatch, PrismDebug.lines);
#endif
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
