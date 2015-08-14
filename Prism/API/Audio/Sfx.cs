using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;

namespace Prism.API.Audio
{
    public delegate bool PlaySoundEvent(SfxEntry entry, Vector2 position, ref int variant, ref float volume, ref float pitch, ref float pan);

    public static partial class Sfx
    {
        const string OBS_REASON = "Please use an overload that takes an SfxEntry instead.";

        internal static Dictionary<string, SfxEntry> VanillaDict = new Dictionary<string, SfxEntry>();

        static Dictionary<KeyValuePair<SfxEntry, int>, SoundEffectInstance> instanceMap = new Dictionary<KeyValuePair<SfxEntry, int>, SoundEffectInstance>();
        static List<SoundEffectInstance> instancePool = new List<SoundEffectInstance>();
        static List<KeyValuePair<SfxEntry, int>> toR = new List<KeyValuePair<SfxEntry, int>>();

        static Tuple<int, float, float, float> CalcParams(SfxEntry e, Vector2 pos, int v, PlaySoundEvent onPlay)
        {
            var volSetting = e.IsAmbient ? (Main.gameInactive ? 0f : Main.ambientVolume) : Main.soundVolume;

            if (volSetting <= 0f)
                return null;

            bool shouldPlay = true;
            float
                vol = 1f, pitch = 0f, pan = 0f;

            if (pos.X <= 0f || pos.Y <= 0f || Single.IsNaN(pos.X) || Single.IsNaN(pos.Y))
                shouldPlay = true;
            else
            {
                Rectangle
                    screen = new Rectangle((int)(Main.screenPosition.X - Main.screenWidth * 2), (int)(Main.screenPosition.Y - Main.screenHeight * 2), Main.screenWidth * 5, Main.screenHeight * 5),
                    sound  = new Rectangle((int)pos.X, (int)pos.Y, 1, 1);

                shouldPlay |= sound.Intersects(screen);

                if (shouldPlay)
                {
                    Vector2 screenCentre = new Vector2(Main.screenPosition.X + Main.screenWidth * 0.5f, Main.screenPosition.Y + Main.screenHeight * 0.5f);

                    pan = (pos.X - screenCentre.X) / (Main.screenWidth * 0.5f);
                    vol = 1f - Vector2.Distance(pos, screenCentre) / (Main.screenWidth * 1.5f);
                }
            }
            pan = MathHelper.Clamp(pan, -1f, 1f);
            vol = Math.Min(vol * volSetting, 1f);

            if (vol <= 0f)
                return null;

            if (onPlay != null && !onPlay(e, pos, ref v, ref vol, ref pitch, ref pan))
                return null;

            vol   = MathHelper.Clamp(vol  ,  0f, 1f);
            pitch = MathHelper.Clamp(pitch, -1f, 1f);
            pan   = MathHelper.Clamp(pan  , -1f, 1f);

            return Tuple.Create(v, vol, pitch, pan);
        }
        static void ApplyParams(SoundEffectInstance inst, Tuple<int, float, float, float> t)
        {
            inst.Volume = t.Item2;
            inst.Pitch  = t.Item3;
            inst.Pan    = t.Item4;
        }
        static void CleanupLingeringInstances()
        {
            SoundEffectInstance inst;

            for (int i = 0; i < instancePool.Count; i++)
            {
                inst = instancePool[i];

                if (inst == null || inst.IsDisposed || inst.State != SoundState.Playing)
                {
                    if (inst != null && !inst.IsDisposed)
                        inst.Dispose();

                    instancePool.RemoveAt(i--);
                }
            }

            toR.Clear();

            foreach (var kvp in instanceMap)
            {
                inst = kvp.Value;

                if (inst == null || inst.IsDisposed || inst.State != SoundState.Playing)
                {
                    if (inst != null && !inst.IsDisposed)
                        inst.Dispose();

                    toR.Add(kvp.Key);
                }
            }

            for (int i = 0; i < toR.Count; i++)
                instanceMap.Remove(toR[i]);

            toR.Clear();
        }

        public static SoundEffectInstance Play(SfxEntry entry, Vector2 position, int variant, PlaySoundEvent onPlay)
        {
            if (Main.dedServ || WorldGen.gen || Main.netMode == 2)
                return null;

            var kvp = new KeyValuePair<SfxEntry, int>(entry, variant);

            var t = CalcParams(entry, position, variant, onPlay);

            if (t == null || t.Item2 <= 0f)
                return null;

            SoundEffectInstance inst;
            var b = entry.PlayBehaviour(variant);
            switch (b)
            {
                case SfxPlayBehaviour.MultipleInstances:
                    inst = entry.GetInstance(variant);

                    instancePool.Add(inst); // don't GC
                    break;
                case SfxPlayBehaviour.PlayIfStopped:
                case SfxPlayBehaviour.PlayIfStoppedUpdateParams:
                    if (instanceMap.ContainsKey(kvp))
                    {
                        var inst_ = instanceMap[kvp];

                        if (inst_.State == SoundState.Stopped)
                            inst = inst_;
                        else
                            return null;
                    }
                    else
                    {
                        inst = entry.GetInstance(variant);

                        instanceMap.Add(kvp, inst);
                    }
                    break;
                case SfxPlayBehaviour.Singleton:
                    if (instanceMap.ContainsKey(kvp))
                    {
                        var inst_ = instanceMap[kvp];

                        inst_.Stop();

                        instanceMap.Remove(kvp);
                    }

                    inst = entry.GetInstance(variant);

                    instanceMap.Add(kvp, inst);
                    break;
                // required, compiler will complain about 'inst' not being assigned to otherwise
                // and this is more foolproof than setting 'inst' to null.
                default:
                    return null;
            }

            ApplyParams(inst, t);

            inst.Play(); // !

            CleanupLingeringInstances();

            return inst;
        }
        public static SoundEffectInstance Play(SfxEntry entry, Vector2 position, int variant, float volMod = Single.NaN, float pitch = Single.NaN, float panMod = Single.NaN)
        {
            return Play(entry, position, variant, (SfxEntry e, Vector2 p, ref int v, ref float vol, ref float pi, ref float pan) =>
            {
                if (!Single.IsNaN(vol))
                    vol *= volMod;
                if (!Single.IsNaN(pi ))
                    pi = pitch;
                if (!Single.IsNaN(pan))
                    pan *= panMod;

                return true;
            });
        }
        public static SoundEffectInstance Play(SfxEntry entry, Vector2 position)
        {
            return Play(entry, position, -1, null);
        }

        public static SoundEffectInstance Play(SfxEntry entry, Point tilePos, int variant, PlaySoundEvent onPlay)
        {
            return Play(entry, tilePos.ToVector2() * 16f, variant, onPlay);
        }
        public static SoundEffectInstance Play(SfxEntry entry, Point tilePos, int variant, float volMod = Single.NaN, float pitch = Single.NaN, float panMod = Single.NaN)
        {
            return Play(entry, tilePos.ToVector2() * 16f, variant, (SfxEntry e, Vector2 p, ref int v, ref float vol, ref float pi, ref float pan) =>
            {
                if (!Single.IsNaN(vol))
                    vol *= volMod;
                if (!Single.IsNaN(pi))
                    pi = pitch;
                if (!Single.IsNaN(pan))
                    pan *= panMod;

                return true;
            });
        }
        public static SoundEffectInstance Play(SfxEntry entry, Point tilePos)
        {
            return Play(entry, tilePos.ToVector2() * 16f, -1, null);
        }

        [Obsolete(OBS_REASON)]
        public static SoundEffectInstance Play(int type, Vector2 position, int style = -1)
        {
            return Play(ById(type), position, style, (SfxEntry e, Vector2 p, ref int v, ref float vol, ref float pitch, ref float pan) =>
            {
                switch (type)
                {
                    case 0: // DigBlock
                        pitch = Main.rand.Next(-10, 11) * 0.01f;
                        break;
                    case 1: // PlayerHit
                        break;
                    case 2:
                        switch (v)
                        {
                            case 37:
                            case 123:
                                vol *= 0.5f;
                                break;
                            case 124:
                            case 125:
                                vol *= 0.65f;
                                break;
                            case 1:
                                var r = Main.rand.Next(3);
                                if (r > 0)
                                    v = 17 + r;
                                break;
                            case 53:
                            case 55:
                                vol *= 0.75f;

                                if (v == 55)
                                    vol *= 0.75f;
                                break;
                        }

                        switch (v)
                        {
                            case 47:
                                pitch = Main.rand.Next(-5, 6) * 0.19f;
                                break;
                            case 53:
                                pitch = Main.rand.Next(-20, -11) * 0.02f;
                                break;
                            case 55:
                                pitch = Main.rand.Next(10, 21) * 0.02f;
                                break;
                            case 26:
                            case 35:
                                vol *= 0.75f;
                                pitch = Main.harpNote;
                                break;
                            default:
                                pitch = Main.rand.Next(-6, 7) * 0.01f;
                                break;
                        }
                        break;
                    case 3:
                        if (v >= 20 && v <= 55)
                            vol *= 0.5f;
                        if (v == 57)
                            vol *= 0.6f;

                        pitch = Main.rand.Next(-10, 11) * 0.01f;
                        break;
                    case 4:
                        if (v >= 23 && v <= 57)
                            vol *= 0.5f;
                        if (v == 61 || v == 62)
                            vol *= 0.6f;

                        pitch = Main.rand.Next(-10, 11) * 0.01f;
                        break;
                    case 6:
                        pitch = Main.rand.Next(-30, 31) * 0.01f;
                        break;
                    case 7:
                    case 16:
                    case 17:
                    case 19:
                    case 27:
                    case 28:
                        pitch = Main.rand.Next(-10, 11) * 0.01f;
                        break;
                    case 8:
                    case 9:
                        pitch = Main.rand.Next(-20, 21) * 0.01f;
                        break;
                    case 14:
                        vol *= 0.4f;
                        break;
                    case 26:
                        pitch = Main.rand.Next(-10, 11) * 0.01f;
                        vol *= 0.9f;
                        break;
                    case 29:
                        if (v == 100 || v == 101)
                            vol *= 0.25f;
                        if ((v >= 93 && v <= 99) || v == 103 || v == 102)
                            vol *= 0.4f;
                        if ((v >= 24 && v <= 87) || v == 92)
                            vol *= 0.5f;
                        if (v == 104)
                            vol *= 0.55f;
                        if (v >= 88 && v <= 91)
                            vol *= 0.7f;

                        pitch = Main.rand.Next(-10, 11) * 0.01f;
                        break;
                    case 30:
                        vol *= 0.75f;

                        pitch = (v == 12 ? Main.rand.Next(-40, 21) : Main.rand.Next(-70, 1)) * 0.01f;
                        break;
                    case 31:
                        vol *= 0.35f;
                        pitch = Main.rand.Next(-40, 21) * 0.01f;
                        break;
                    case 32:
                        vol *= 0.15f;
                        pitch = Main.rand.Next(-70, 26) * 0.01f;
                        break;
                    case 33:
                        vol *= 0.2f;
                        pitch = Main.rand.Next(-10, 31) * 0.01f;
                        break;
                    case 34:
                    case 35:
                        vol *= Math.Min(v / 50f, 1f) * 0.2f; // WTF, red?

                        if (type == 34)
                            pitch = -0.2f;
                        break;
                    case 36:
                        if (v <= 0)
                            pitch += 0.6f;
                        break;
                    case 37:
                    case 38:
                        if (type == 37)
                            vol *= v * 0.05f; // WTF, red?

                        pitch = Main.rand.Next(-40, 41) * 0.01f;
                        break;
                    case 39:
                        vol *= 0.5f;
                        pitch = Main.rand.Next(-30, 31) * 0.01f;
                        break;
                    case 41:
                        // (dist sound-player + 1)^-1
                        vol = 1f / (1f + (p - Main.player[Main.myPlayer].position).Length());
                        pitch = Main.rand.Next(-10, 11) * 0.01f;
                        break;
                }

                return true;
            });
        }
        [Obsolete(OBS_REASON)]
        public static SoundEffectInstance Play(int type, Point tilePos, int style = -1)
        {
            return Play(type, tilePos.ToVector2() * 16f, style);
        }
        [Obsolete(OBS_REASON)]
        public static SoundEffectInstance Play(int type, int x = -1, int y = -1, int style = -1)
        {
            return Play(type, new Vector2(x, y), style);
        }
    }
}
