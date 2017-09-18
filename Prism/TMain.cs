using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Audio;
using Prism.Debugging;
using Prism.IO;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Mods.Hooks;
using Prism.Util;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Map;

namespace Prism
{
    public sealed class TMain : Main
    {
        internal static Texture2D WhitePixel, UnknownItemTexture;

        static bool justDrawCrashed = false;
        static Exception lastDrawExn = null;

        static bool prevGameMenu = true;

        /// <summary>
        /// The amount of time elapsed since the previous frame (in seconds).
        /// </summary>
        public static float ElapsedTime
        {
            get;
            private set;
        }

        public static bool ChatMode
        {
            get
            {
                return drawingPlayerChat || editSign || editChest || blockInput;
            }
        }
        public static bool NotInChatMode
        {
            get
            {
                return !drawingPlayerChat && !editSign && !editChest && !blockInput;
            }
        }

        internal TMain()
        {
            versionNumber += ", " + PrismApi.NiceVersionString;

            SavePath += "/Prism";

            PlayerPath = SavePath + "/Players";
            WorldPath  = SavePath + "/Worlds" ;

            PrismApi.ModDirectory = SavePath + "/Mods";

            CloudPlayerPath = "players_Prism";
            CloudWorldPath  = "worlds_Prism" ;

            LocalFavoriteData  = new FavoritesFile(SavePath + "/favorites.json", false);
            CloudFavoritesData = new FavoritesFile("/favorites_Prism.json", false);

            Configuration = new Preferences(SavePath + "/config.json", false, false);

            ElapsedTime = 0;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT && graphics.GraphicsProfile == GraphicsProfile.Reach && GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef))
                graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        static void PlayUseSound(Item i, Player p)
        {
            if (i.P_UseSound as SfxRef != null)
            {
                var r = (SfxRef)i.P_UseSound;

                Sfx.Play(r.Resolve(), p.position, r.VariantID);
            }
            /*else if (i.useSound > 0)
                Sfx.Play(VanillaSfxes.UseItem, p.position, i.useSound);*/
        }

        static void PlayHitSound   (NPC n)
        {
            if (n.P_SoundOnHit as SfxRef != null)
            {
                var r = (SfxRef)n.P_SoundOnHit;

                Sfx.Play(r.Resolve(), n.position, r.VariantID);
            }
            /*else if (n.soundHit > 0)
                Sfx.Play(VanillaSfxes.NpcHit, n.position, n.soundHit);*/
        }
        static void PlayKilledSound(NPC n)
        {
            if (n.P_SoundOnDeath as SfxRef != null)
            {
                var r = (SfxRef)n.P_SoundOnDeath;

                Sfx.Play(r.Resolve(), n.position, r.VariantID);
            }
            /*else if (n.soundKilled > 0)
                Sfx.Play(VanillaSfxes.NpcKilled, n.position, n.soundKilled);*/
        }

        static void OnUpdateKeyboard(Main _, GameTime __)
        {
            HookManager.GameBehaviour.OnUpdateKeyboard();
        }
        static void OnPreDrawM(GameTime _)
        {
            HookManager.GameBehaviour.PreDraw(spriteBatch);
        }
        // TODO: postdraw?
        static void OnPostScreenClear()
        {
            HookManager.GameBehaviour.PostScreenClear();
        }
        static void OnDrawBackground(Main m)
        {
            /*if (HookManager.GameBehaviour.PreDrawBackground(spriteBatch))
            {
                m.RealDrawBackground();

                HookManager.GameBehaviour.PostDrawBackground(spriteBatch);
            }*/
        }
        static bool IsChatAllowed()
        {
            return HookManager.GameBehaviour.IsChatAllowed();
        }
        static bool OnLocalChat()
        {
            return HookManager.GameBehaviour.OnLocalChat();
        }

        static void HookWrappedMethods()
        {
            P_OnUpdateAudio += Bgm.Update;
          //P_OnUpdateKeyboard += OnUpdateKeyboard;

            OnPreDraw += OnPreDrawM;
            P_OnDrawBackground += OnDrawBackground ;
            P_OnPostScrClDraw  += OnPostScreenClear;

          //P_OnP_IsChatAllowed  += IsChatAllowed; //Prismception

          //P_OnP_LocalChat += OnLocalChat;

#pragma warning disable 618
          //P_OnPlaySound += (t, x, y, s) => Sfx.Play(t, new Vector2(x, y), s);
#pragma warning restore 618

            Item      .P_OnSetDefaultsById   += ItemDefHandler.OnSetDefaults      ;
            NPC       .P_OnSetDefaultsById   += NpcDefHandler .OnSetDefaults      ;
            Projectile.P_OnSetDefaults       += ProjDefHandler.OnSetDefaults      ;

            Player.P_OnUpdateEquips    += ItemHooks.OnUpdateEquips    ;
            Player.P_OnUpdateArmorSets += ItemHooks.OnUpdateArmourSets;
            Player.P_OnWingMovement    += ItemHooks.WingMovement      ;
            Player.P_OnPlaceThing      += TileHooks.OnPlaceThing      ;
            Player.P_OnPreShoot        += ItemHooks.PreShoot          ;

            NPC.P_OnNewNPC    += NpcHooks.OnNewNPC   ;
            NPC.P_OnUpdateNPC += NpcHooks.OnUpdateNPC;
            NPC.P_OnAI        += NpcHooks.OnAI       ;
            NPC.P_OnNPCLoot   += NpcHooks.OnNPCLoot  ;
            NPC.P_OnFindFrame += NpcHooks.OnFindFrame;

            P_OnDrawNPC += NpcHooks.OnDrawNPC;

          //NPC.P_ReflectProjectile_PlaySoundHit += (n, _) => PlayHitSound(n);
          //NPC.P_StrikeNPC_PlaySoundHit         += (n, _d, _kb, _hd, _c, _ne, _fn) => PlayHitSound(n);
          //NPC.P_checkDead_PlaySoundKilled      += PlayKilledSound;
          //NPC.P_RealAI_PlaySoundKilled         += PlayKilledSound;

            NPC.P_OnAddBuff     += NpcHooks.OnAddBuff    ;
            NPC.P_OnBuffEffects += NpcHooks.OnBuffEffects;

            Player.P_OnGetFileData += PlayerHooks.OnGetFiledata;
            Player.P_OnItemCheck   += PlayerHooks.OnItemCheck  ;
            Player.P_OnKillMe      += PlayerHooks.OnKillMe     ;
            Player.P_OnUpdate      += PlayerHooks.OnUpdate     ;
            Player.P_OnMidUpdate   += PlayerHooks.OnMidUpdate  ;
            Player.P_OnUpdateBuffs += PlayerHooks.OnUpdateBuffs;
            Player.P_OnAddBuff     += PlayerHooks.OnAddBuff    ;
            Player.Hooks.OnEnterWorld += PlayerHooks.OnEnterWorld;

            UICharacterSelect.P_OnNewCharacterClick += PlayerHooks.OnNewCharacterClick;

            P_OnDrawPlayer += PlayerHooks.OnDrawPlayer;

          //Player.P_ItemCheck_PlayUseSound0     += PlayUseSound;
          //Player.P_ItemCheck_PlayUseSound1     += PlayUseSound;
          //Player.P_QuickBuff_PlayUseSound      += PlayUseSound;
          //Player.P_QuickGrapple_PlayUseSound   += PlayUseSound;
          //Player.P_QuickHeal_PlayUseSound      += PlayUseSound;
          //Player.P_QuickMana_PlayUseSound      += PlayUseSound;
          //Player.P_QuickMount_PlayUseSound     += PlayUseSound;
          //Player.P_UpdatePet_PlayUseSound      += PlayUseSound;
          //Player.P_UpdatePetLight_PlayUseSound += PlayUseSound;

            Projectile.P_OnAI            += ProjHooks.OnAI           ;
            Projectile.P_OnKill          += ProjHooks.OnKill         ;
            Projectile.P_OnNewProjectile += ProjHooks.OnNewProjectile;
            Projectile.P_OnUpdate        += ProjHooks.OnUpdate       ;
            Projectile.P_OnColliding     += ProjHooks.OnColliding    ;

            P_OnDrawProj += ProjHooks.OnDrawProj;

            Mount.P_OnDismount      += MountHooks.OnDismount     ;
            Mount.P_OnDraw          += MountHooks.OnDraw         ;
            Mount.P_OnHover         += MountHooks.OnHover        ;
            Mount.P_OnJumpHeight    += MountHooks.OnJumpHeight   ;
            Mount.P_OnJumpSpeed     += MountHooks.OnJumpSpeed    ;
            Mount.P_OnSetMount      += MountHooks.OnSetMount     ;
            Mount.P_OnUpdateEffects += MountHooks.OnUpdateEffects;
            Mount.P_OnUpdateFrame   += MountHooks.OnUpdateFrame  ;

            Player.P_OnSavePlayer += SaveDataHandler.SavePlayer;
            Player.P_OnLoadPlayer += SaveDataHandler.LoadPlayer;

            WorldFile.P_OnSaveWorld += SaveDataHandler.SaveWorld;
            WorldFile.P_OnLoadWorld += SaveDataHandler.LoadWorld;

          //Recipe.P_OnFindRecipes += RecipeHooks.FindRecipes;
          //Recipe.P_OnCreate      += RecipeHooks.Create     ;
        }

        protected override void Initialize()
        {
            HookWrappedMethods();

            base.Initialize(); // terraria init and LoadContent happen here

            ModLoader.Load();

            Handler.DefaultColourLookupLength = MapHelper.colorLookup.Length;

            ApplyHotfixes();

            versionNumber += ", mods loaded: " + ModData.Mods.Count +
                (ModLoader.errors.Count > 0 ? ", mods loading errors: " + ModLoader.errors.Count : String.Empty);
        }
        protected override void LoadContent()
        {
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Prism.Resources.UnknownItem.png"))
            {
                UnknownItemTexture = Texture2D.FromStream(GraphicsDevice, s);
            }

            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            ModLoader.Unload();

            UnknownItemTexture.Dispose();
            UnknownItemTexture = null;

            WhitePixel.Dispose();
            WhitePixel = null;

            base.UnloadContent();
        }

        /// <summary>
        /// For those hooks and stuff we just don't have yet...
        /// </summary>
        void ApplyHotfixes()
        {
            foreach (Player p in from plr in player where plr.active select plr)
                if (p.npcTypeNoAggro.Length < Handler.NpcDef.NextTypeIndex)
                    Array.Resize(ref p.npcTypeNoAggro, Handler.NpcDef.NextTypeIndex);

            if (WorldGen.tileCounts.Length < tileSetsLoaded.Length)
                Array.Resize(ref WorldGen.tileCounts, tileSetsLoaded.Length);
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

                HookManager.GameBehaviour.UpdateDebug(gt);

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
                HookManager.GameBehaviour.PreScreenClear();

                base.Draw(gt);

                HookManager.GameBehaviour.PostDraw(spriteBatch);

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

