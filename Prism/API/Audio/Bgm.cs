using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Prism.Mods;
using Prism.Mods.Hooks;
using Prism.Util;
using Terraria;

namespace Prism.API.Audio
{
    public static class Bgm
    {
        const float fade_delta = 0.005f;

        internal static Dictionary<string, BgmEntry> VanillaDicts = new Dictionary<string, BgmEntry>();
        internal static Dictionary<ModInfo, Dictionary<string, BgmEntry>> ByMod = new Dictionary<ModInfo, Dictionary<string, BgmEntry>>();

        static BgmEntry current;

        static IEnumerable<KeyValuePair<string, BgmEntry>> GetEntries()
        {
            return VanillaDicts.Concat(ByMod.Select(kvp => kvp.Value).Flatten());
        }
        static float GetMoonLordSpawnFade(BgmEntry current)
        {
            float ret = 1f;
            if (NPC.MoonLordCountdown > 0)
            {
                ret = NPC.MoonLordCountdown / 3600f;
                ret *= ret; // fade out exponentially

                if (NPC.MoonLordCountdown > 720)
                    ret = MathHelper.Lerp(0f, 1f, ret);
                else // mute completely for the last 12 seconds
                {
                    ret = 0f;
                    current = null;
                }

                // just spawned -> fade back in
                if (NPC.MoonLordCountdown == 1 && current != null /*&& curMusic < MusicAmt*/)
                    current.fade = 0f;
            }

            return ret;
        }

        static bool HandleInactiveWindow(IEnumerable<BgmEntry> allEntries)
        {
            // pause if game window isn't selected
            if (!Main.instance.IsActive)
            {
                foreach (var e in allEntries)
                {
                    if (e.fade > 0f && e.Music.State == SoundState.Playing)
                        e.Music.Pause();
                }

                return true;
            }
            // resume otherwise
            else
                foreach (var e in allEntries)
                    if (e.fade > 0f && e.Music.State == SoundState.Paused)
                        e.Music.Play();

            return false;
        }

        static void UpdateAmbientEntry(BgmEntry current, BgmEntry e)
        {
            var m = e.Music;

            if (e.ShouldPlay())
            {
                // stop if playing & volume is zero
                if (Main.ambientVolume <= 0f)
                {
                    if (m.State == SoundState.Playing)
                        m.Stop();
                }
                // play if not already
                else if (m.State != SoundState.Playing)
                    m.Play();
                else if (m.State == SoundState.Paused)
                    m.Play();
                // fade in if needed
                else
                    e.fade = MathHelper.Clamp(e.fade + fade_delta, 0f, 1f);
            }
            else if (m.State == SoundState.Playing)
            {
                // fade out if needed
                if (current != null && current.fade > 0.25f)
                    e.fade -= fade_delta;
                // stop playing if nothing is
                else if (current == null)
                    e.fade = 0f;

                if (e.fade <= 0f)
                    m.Stop();
            }
            else
                e.fade = 0f;
        }
        static void UpdateCurrentEntry(BgmEntry e)
        {
            var m = e.Music;

            // play if not already
            if (m.State != SoundState.Playing)
                m.Play();
            else // fade in if needed
                e.fade = MathHelper.Clamp(e.fade + fade_delta, 0f, 1f);
        }
        static void UpdateOtherEntry(BgmEntry current, BgmEntry e)
        {
            var m = e.Music;

            if (m.State != SoundState.Playing)
            {
                e.fade = 0f;
                return;
            }

            if (e.fade > 0.25f)
                e.fade -= fade_delta;
            else //if (current == null)
                e.fade = 0f;

            if (e.fade <= 0f)
                m.Stop();
        }

        internal static void Update(Main _)
        {
            if (Main.musicVolume <= 0f || Main.dedServ)
                return;

            var allEntries = GetEntries();
            var allValues = allEntries.Select(kvp => kvp.Value);

            if (current != null)
                if (HandleInactiveWindow(allValues))
                    return;

            //TODO: specify BgmEntry in NpcDef for convenience
            var newCurrent = allEntries.Where(e =>
                    e.Value.Priority == BgmPriority.Title == Main.gameMenu && e.Value.ShouldPlay()
                ).OrderByDescending(e =>
                    e.Value.Priority
                ).FirstOrDefault();

            Main.engine.Update();

            HookManager.ModDef.UpdateMusic(ref newCurrent);

            var newEntry = newCurrent.Value;

            float moonLordSpawnFade = GetMoonLordSpawnFade(newCurrent.Value);

            foreach (var e in allValues)
            {
                if (e.Priority == BgmPriority.Ambient)
                    UpdateAmbientEntry(newEntry, e);
                else if (e == newEntry)
                    UpdateCurrentEntry(e);
                else
                    UpdateOtherEntry(newEntry, e);

                var m = e.Music;
                if (m.State == SoundState.Playing)
                {
                    var nv = e.fade * Main.musicVolume * (e.Priority == BgmPriority.Ambient ? 1f : moonLordSpawnFade);

                    if (nv != m.Volume) // getting volume is way faster than setting using the XACT and XAudio APIs
                        m.Volume = nv;
                }
            }

            current = newEntry;
        }

        public static IBgm VanillaBgmOf(int cueId)
        {
            return new XactBgm(Main.soundBank, Main.waveBank, "Music_" + cueId);
        }

        public static bool AnyNPCsForMusic(params int[] npcTypes)
        {
            return AnyNPCsForMusic(n => Array.IndexOf(npcTypes, n.type) != -1);
        }
        public static bool AnyNPCsForMusic(Predicate<NPC> pred)
        {
            var screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            const int npcPadding = 5000;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                var npcMusicHitbox = new Rectangle((int)(Main.npc[i].position.X + Main.npc[i].width / 2) - npcPadding, (int)(Main.npc[i].position.Y + Main.npc[i].height / 2) - npcPadding, npcPadding * 2, npcPadding * 2);

                if (screen.Intersects(npcMusicHitbox) && Main.npc[i].active && pred(Main.npc[i]))
                    return true;
            }

            return false;
        }
    }
}
