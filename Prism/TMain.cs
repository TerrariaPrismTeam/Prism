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
using Prism.Util;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;

namespace Prism
{
    public sealed class TMain : Main
    {
        internal static Texture2D WhitePixel;
        /// <summary>
        /// The amount of time elapsed since the previous frame (in seconds).
        /// </summary>
        public static float ElapsedTime
        {
            get;
            private set;
        }

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

            LocalFavoriteData  = new FavoritesFile(SavePath + "\\favorites.json", false);
            CloudFavoritesData = new FavoritesFile("/favorites_Prism.json", false);

            Configuration = new Preferences(SavePath + "\\config.json", false, false);

            ElapsedTime = 0;
        }

        static void PlayUseSound(Item i, Player p)
        {
            if (i.P_UseSound as SfxRef != null)
            {
                var r = (SfxRef)i.P_UseSound;

                Sfx.Play(r.Resolve(), p.position, r.VariantID);
            }
            else if (i.useSound > 0)
                Sfx.Play(VanillaSfxes.UseItem, p.position, i.useSound);
        }

        static void PlayHitSound   (NPC n)
        {
            if (n.P_SoundOnHit as SfxRef != null)
            {
                var r = (SfxRef)n.P_SoundOnHit;

                Sfx.Play(r.Resolve(), n.position, r.VariantID);
            }
            else if (n.soundHit > 0)
                Sfx.Play(VanillaSfxes.NpcHit, n.position, n.soundHit);
        }
        static void PlayKilledSound(NPC n)
        {
            if (n.P_SoundOnDeath as SfxRef != null)
            {
                var r = (SfxRef)n.P_SoundOnDeath;

                Sfx.Play(r.Resolve(), n.position, r.VariantID);
            }
            else if (n.soundKilled > 0)
                Sfx.Play(VanillaSfxes.NpcKilled, n.position, n.soundKilled);
        }

        static void HookWrappedMethods()
        {
            P_OnUpdateMusic += Bgm.Update;

#pragma warning disable 618
            P_OnPlaySound += (t, x, y, s) => Sfx.Play(t, new Vector2(x, y), s);
#pragma warning restore 618

            Item      .P_OnSetDefaultsById   += ItemDefHandler.OnSetDefaults      ;
            Item      .P_OnSetDefaultsByName += ItemDefHandler.OnSetDefaultsByName;
            NPC       .P_OnSetDefaultsById   += NpcDefHandler .OnSetDefaults      ;
            NPC       .P_OnSetDefaultsByName += NpcDefHandler .OnSetDefaultsByName;
            Projectile.P_OnSetDefaults       += ProjDefHandler.OnSetDefaults      ;

            //Player.P_OnUpdateEquips  += ItemHooks.OnUpdateEquips    ;
            Player.P_OnUpdateArmorSets += ItemHooks.OnUpdateArmourSets;
            Player.P_OnWingMovement    += ItemHooks.WingMovement      ;

            NPC.P_OnNewNPC    += NpcHooks.OnNewNPC   ;
            NPC.P_OnUpdateNPC += NpcHooks.OnUpdateNPC;
            NPC.P_OnAI        += NpcHooks.OnAI       ;
            NPC.P_OnNPCLoot   += NpcHooks.OnNPCLoot  ;

            P_OnDrawNPC += NpcHooks.OnDrawNPC;

            NPC.P_ReflectProjectile_PlaySoundHit += (n, _) => PlayHitSound(n);
            NPC.P_StrikeNPC_PlaySoundHit         += (n, _d, _kb, _hd, _c, _ne, _fn) => PlayHitSound(n);
            NPC.P_checkDead_PlaySoundKilled      += PlayKilledSound;
            NPC.P_RealAI_PlaySoundKilled         += PlayKilledSound;

            Player.P_OnGetFileData += PlayerHooks.OnGetFiledata;
            Player.P_OnItemCheck   += PlayerHooks.OnItemCheck  ;
            Player.OnEnterWorld    += PlayerHooks.OnEnterWorld ;
            Player.P_OnKillMe      += PlayerHooks.OnKillMe     ;
            Player.P_OnUpdate      += PlayerHooks.OnUpdate     ;
            Player.P_OnMidUpdate   += PlayerHooks.OnMidUpdate  ;

            UICharacterSelect.P_OnNewCharacterClick += PlayerHooks.OnNewCharacterClick;

            P_OnDrawPlayer += PlayerHooks.OnDrawPlayer;

            Player.P_ItemCheck_PlayUseSound0     += PlayUseSound;
            Player.P_ItemCheck_PlayUseSound1     += PlayUseSound;
            Player.P_QuickBuff_PlayUseSound      += PlayUseSound;
            Player.P_QuickGrapple_PlayUseSound   += PlayUseSound;
            Player.P_QuickHeal_PlayUseSound      += PlayUseSound;
            Player.P_QuickMana_PlayUseSound      += PlayUseSound;
            Player.P_QuickMount_PlayUseSound     += PlayUseSound;
            Player.P_UpdatePet_PlayUseSound      += PlayUseSound;
            Player.P_UpdatePetLight_PlayUseSound += PlayUseSound;

            Projectile.P_OnAI            += ProjHooks.OnAI           ;
            Projectile.P_OnKill          += ProjHooks.OnKill         ;
            Projectile.P_OnNewProjectile += ProjHooks.OnNewProjectile;
            Projectile.P_OnUpdate        += ProjHooks.OnUpdate       ;

            P_OnDrawProj += ProjHooks.OnDrawProj;
        }

        protected override void Initialize()
        {
            HookWrappedMethods();

            base.Initialize(); // terraria init and LoadContent happen here

            ModLoader.Load();

#if DEV_BUILD
            ModLoader.Debug_ShowAllErrors();
#endif

            ApplyHotfixes();

            versionNumber += ", mods loaded: " + ModData.Mods.Count +
                (ModLoader.errors.Count > 0 ? ", load errors: " + ModLoader.errors.Count : String.Empty);
        }
        protected override void LoadContent()
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

        // See MainPatcher.Patch()...

        //public override bool IsChatAllowedHook()
        //{
        //    return base.IsChatAllowedHook();
        //}

        //public override bool PlayerChatLocalHook()
        //{
        //    return base.PlayerChatLocalHook();
        //}

        protected override void Update(GameTime gt)
        {
            try
            {
                ElapsedTime = (float)gt.ElapsedGameTime.TotalSeconds;

                HookManager.GameBehaviour.PreUpdate();

                ApplyHotfixes(); //The array is initialized every time new Player() is called. Until we have like InitPlayer or something we just have to ghettohack it like this.

                base.Update(gt);

                if (!gameMenu && prevGameMenu)
                    Helpers.Main.RandColorText("Welcome to " + PrismApi.NiceVersionString + ".", true);

                HookManager.GameBehaviour.PostUpdate();

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

#if TRACE
                TraceDrawer.DrawTrace(spriteBatch, PrismDebug.lines);
#endif

                justDrawCrashed = false;
                lastDrawExn = null;
            }
            catch (Exception e)
            {
                if (justDrawCrashed)
                {
                    Logging.LogWarning("Crashed during critical drawing operation, drawing state got messed up.");
                    ExceptionHandler.HandleFatal(new AggregateException(lastDrawExn, e)); // drawing state got fucked up
                }
                else
                {
                    justDrawCrashed = true;

                    ExceptionHandler.Handle(lastDrawExn = e);
                }
            }
        }
    }
}
