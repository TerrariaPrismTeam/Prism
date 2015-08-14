using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;

namespace Prism.API.Audio
{
    public static partial class Sfx
    {
        internal static Dictionary<string, SfxEntry> VanillaDict = new Dictionary<string, SfxEntry>();

        static Dictionary<SfxEntry, SoundEffectInstance> instanceMap = new Dictionary<SfxEntry, SoundEffectInstance>();
        static List<SoundEffectInstance> instancePool = new List<SoundEffectInstance>();

        static void PlaySoundVanilla(int type, int x, int y, int style)
        {
            int newStyle = style;

            if (Main.dedServ || WorldGen.gen || Main.netMode == 2)
                return;

            bool canBeOffScreen = type >= 30 && type <= 35;
            bool isAmbient = canBeOffScreen || type == 39;
            var volSetting = isAmbient ? (Main.gameInactive ? 0f : Main.ambientVolume) : Main.soundVolume;

            if (volSetting > 0f || canBeOffScreen && type != 39)
            {
                bool shouldPlay = false;
                float vol = 1f;
                float pan = 0f;

                if (x == -1 || y == -1)
                    shouldPlay = true;
                else
                {
                    Rectangle
                        screen = new Rectangle((int)(Main.screenPosition.X - Main.screenWidth * 2), (int)(Main.screenPosition.Y - Main.screenHeight * 2), Main.screenWidth * 5, Main.screenHeight * 5),
                        sound = new Rectangle(x, y, 1, 1);

                    if (sound.Intersects(screen))
                        shouldPlay = true;

                    if (shouldPlay)
                    {
                        Vector2 screenCentre = new Vector2(Main.screenPosition.X + Main.screenWidth * 0.5f, Main.screenPosition.Y + Main.screenHeight * 0.5f);

                        pan = (x - screenCentre.X) / (Main.screenWidth * 0.5f);
                        float dx = x - screenCentre.X;
                        float dy = y - screenCentre.Y;
                        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                        vol = 1f - dist / (Main.screenWidth * 1.5f);
                    }
                }
                pan = MathHelper.Clamp(pan, -1f, 1f);
                vol = Math.Min(vol * volSetting, 1f);

                if (vol <= 0f && !isAmbient)
                    return;

                int n;
                if (type == 0)
                {
                    n = Main.rand.Next(3);
                    Main.soundInstanceDig[n].Stop();
                    Main.soundInstanceDig[n] = Main.soundDig[n].CreateInstance();
                    Main.soundInstanceDig[n].Volume = vol;
                    Main.soundInstanceDig[n].Pan = pan;
                    Main.soundInstanceDig[n].Pitch = Main.rand.Next(-10, 11) * 0.01f;
                    Main.soundInstanceDig[n].Play();
                }
                else if (type == 1)
                {
                    n = Main.rand.Next(3);
                    Main.soundInstancePlayerHit[n].Stop();
                    Main.soundInstancePlayerHit[n] = Main.soundPlayerHit[n].CreateInstance();
                    Main.soundInstancePlayerHit[n].Volume = vol;
                    Main.soundInstancePlayerHit[n].Pan = pan;
                    Main.soundInstancePlayerHit[n].Play();
                }
                else if (type == 2)
                {
                    if (newStyle == 123)
                        vol *= 0.5f;
                    if (newStyle == 124 || newStyle == 125)
                        vol *= 0.65f;

                    if (newStyle == 1)
                    {
                        int s = Main.rand.Next(3);
                        if (s == 1)
                            newStyle = 18;
                        if (s == 2)
                            newStyle = 19;
                    }
                    else if (newStyle == 55 || newStyle == 53)
                    {
                        vol *= 0.75f;
                        if (newStyle == 55)
                            vol *= 0.75f;
                        if (Main.soundInstanceItem[newStyle].State == SoundState.Playing)
                            return;
                    }
                    else if (newStyle == 37)
                        vol *= 0.5f;

                    if (newStyle != 9 && newStyle != 10 && newStyle != 24 && newStyle != 26 && newStyle != 34 && newStyle != 43 && newStyle != 103)
                        Main.soundInstanceItem[newStyle].Stop();

                    Main.soundInstanceItem[newStyle] = Main.soundItem[newStyle].CreateInstance();
                    Main.soundInstanceItem[newStyle].Volume = vol;
                    Main.soundInstanceItem[newStyle].Pan = pan;

                    if (newStyle == 47)
                        Main.soundInstanceItem[newStyle].Pitch = Main.rand.Next(-5, 6) * 0.19f;
                    else if (newStyle == 53)
                        Main.soundInstanceItem[newStyle].Pitch = Main.rand.Next(-20, -11) * 0.02f;
                    else if (newStyle == 55)
                        Main.soundInstanceItem[newStyle].Pitch = -Main.rand.Next(-20, -11) * 0.02f;
                    else
                        Main.soundInstanceItem[newStyle].Pitch = Main.rand.Next(-6, 7) * 0.01f;

                    if (newStyle == 26 || newStyle == 35)
                    {
                        Main.soundInstanceItem[newStyle].Volume = vol * 0.75f;
                        Main.soundInstanceItem[newStyle].Pitch = Main.harpNote;
                    }
                    Main.soundInstanceItem[newStyle].Play();
                }
                else if (type == 3)
                {
                    if (newStyle >= 20 && newStyle <= 54)
                        vol *= 0.5f;

                    if (newStyle != 57 || Main.soundInstanceNPCHit[newStyle].State != SoundState.Playing)
                    {
                        if (newStyle == 57)
                            vol *= 0.6f;
                        if (newStyle == 55 || newStyle == 56)
                            vol *= 0.5f;

                        Main.soundInstanceNPCHit[newStyle].Stop();
                        Main.soundInstanceNPCHit[newStyle] = Main.soundNPCHit[newStyle].CreateInstance();
                        Main.soundInstanceNPCHit[newStyle].Volume = vol;
                        Main.soundInstanceNPCHit[newStyle].Pan = pan;
                        Main.soundInstanceNPCHit[newStyle].Pitch = Main.rand.Next(-10, 11) * 0.01f;
                        Main.soundInstanceNPCHit[newStyle].Play();
                    }
                }
                else if (type == 4)
                {
                    if (newStyle >= 23 && newStyle <= 57)
                        vol *= 0.5f;
                    if (newStyle == 61)
                        vol *= 0.6f;
                    if (newStyle == 62)
                        vol *= 0.6f;

                    if (newStyle != 10 || Main.soundInstanceNPCKilled[newStyle].State != SoundState.Playing)
                    {
                        Main.soundInstanceNPCKilled[newStyle] = Main.soundNPCKilled[newStyle].CreateInstance();
                        Main.soundInstanceNPCKilled[newStyle].Volume = vol;
                        Main.soundInstanceNPCKilled[newStyle].Pan = pan;
                        Main.soundInstanceNPCKilled[newStyle].Pitch = Main.rand.Next(-10, 11) * 0.01f;
                        Main.soundInstanceNPCKilled[newStyle].Play();
                    }
                }
                else if (type == 5)
                {
                    Main.soundInstancePlayerKilled.Stop();
                    Main.soundInstancePlayerKilled = Main.soundPlayerKilled.CreateInstance();
                    Main.soundInstancePlayerKilled.Volume = vol;
                    Main.soundInstancePlayerKilled.Pan = pan;
                    Main.soundInstancePlayerKilled.Play();
                }
                else if (type == 6)
                {
                    Main.soundInstanceGrass.Stop();
                    Main.soundInstanceGrass = Main.soundGrass.CreateInstance();
                    Main.soundInstanceGrass.Volume = vol;
                    Main.soundInstanceGrass.Pan = pan;
                    Main.soundInstanceGrass.Pitch = Main.rand.Next(-30, 31) * 0.01f;
                    Main.soundInstanceGrass.Play();
                }
                else if (type == 7)
                {
                    Main.soundInstanceGrab.Stop();
                    Main.soundInstanceGrab = Main.soundGrab.CreateInstance();
                    Main.soundInstanceGrab.Volume = vol;
                    Main.soundInstanceGrab.Pan = pan;
                    Main.soundInstanceGrab.Pitch = Main.rand.Next(-10, 11) * 0.01f;
                    Main.soundInstanceGrab.Play();
                }
                else if (type == 8)
                {
                    Main.soundInstanceDoorOpen.Stop();
                    Main.soundInstanceDoorOpen = Main.soundDoorOpen.CreateInstance();
                    Main.soundInstanceDoorOpen.Volume = vol;
                    Main.soundInstanceDoorOpen.Pan = pan;
                    Main.soundInstanceDoorOpen.Pitch = Main.rand.Next(-20, 21) * 0.01f;
                    Main.soundInstanceDoorOpen.Play();
                }
                else if (type == 9)
                {
                    Main.soundInstanceDoorClosed.Stop();
                    Main.soundInstanceDoorClosed = Main.soundDoorClosed.CreateInstance();
                    Main.soundInstanceDoorClosed.Volume = vol;
                    Main.soundInstanceDoorClosed.Pan = pan;
                    Main.soundInstanceDoorOpen.Pitch = Main.rand.Next(-20, 21) * 0.01f;
                    Main.soundInstanceDoorClosed.Play();
                }
                else if (type == 10)
                {
                    Main.soundInstanceMenuOpen.Stop();
                    Main.soundInstanceMenuOpen = Main.soundMenuOpen.CreateInstance();
                    Main.soundInstanceMenuOpen.Volume = vol;
                    Main.soundInstanceMenuOpen.Pan = pan;
                    Main.soundInstanceMenuOpen.Play();
                }
                else if (type == 11)
                {
                    Main.soundInstanceMenuClose.Stop();
                    Main.soundInstanceMenuClose = Main.soundMenuClose.CreateInstance();
                    Main.soundInstanceMenuClose.Volume = vol;
                    Main.soundInstanceMenuClose.Pan = pan;
                    Main.soundInstanceMenuClose.Play();
                }
                else if (type == 12)
                {
                    Main.soundInstanceMenuTick.Stop();
                    Main.soundInstanceMenuTick = Main.soundMenuTick.CreateInstance();
                    Main.soundInstanceMenuTick.Volume = vol;
                    Main.soundInstanceMenuTick.Pan = pan;
                    Main.soundInstanceMenuTick.Play();
                }
                else if (type == 13)
                {
                    Main.soundInstanceShatter.Stop();
                    Main.soundInstanceShatter = Main.soundShatter.CreateInstance();
                    Main.soundInstanceShatter.Volume = vol;
                    Main.soundInstanceShatter.Pan = pan;
                    Main.soundInstanceShatter.Play();
                }
                else if (type == 14)
                {
                    if (style == 489)
                    {
                        n = Main.rand.Next(21, 24);
                        Main.soundInstanceZombie[n] = Main.soundZombie[n].CreateInstance();
                        Main.soundInstanceZombie[n].Volume = vol * 0.4f;
                        Main.soundInstanceZombie[n].Pan = pan;
                        Main.soundInstanceZombie[n].Play();
                    }
                    else
                    {
                        n = Main.rand.Next(3);
                        Main.soundInstanceZombie[n] = Main.soundZombie[n].CreateInstance();
                        Main.soundInstanceZombie[n].Volume = vol * 0.4f;
                        Main.soundInstanceZombie[n].Pan = pan;
                        Main.soundInstanceZombie[n].Play();
                    }
                }
                else if (type == 15)
                {
                    if (Main.soundInstanceRoar[newStyle].State == SoundState.Stopped)
                    {
                        Main.soundInstanceRoar[newStyle] = Main.soundRoar[newStyle].CreateInstance();
                        Main.soundInstanceRoar[newStyle].Volume = vol;
                        Main.soundInstanceRoar[newStyle].Pan = pan;
                        Main.soundInstanceRoar[newStyle].Play();
                    }
                }
                else if (type == 16)
                {
                    Main.soundInstanceDoubleJump.Stop();
                    Main.soundInstanceDoubleJump = Main.soundDoubleJump.CreateInstance();
                    Main.soundInstanceDoubleJump.Volume = vol;
                    Main.soundInstanceDoubleJump.Pan = pan;
                    Main.soundInstanceDoubleJump.Pitch = Main.rand.Next(-10, 11) * 0.01f;
                    Main.soundInstanceDoubleJump.Play();
                }
                else if (type == 17)
                {
                    Main.soundInstanceRun.Stop();
                    Main.soundInstanceRun = Main.soundRun.CreateInstance();
                    Main.soundInstanceRun.Volume = vol;
                    Main.soundInstanceRun.Pan = pan;
                    Main.soundInstanceRun.Pitch = Main.rand.Next(-10, 11) * 0.01f;
                    Main.soundInstanceRun.Play();
                }
                else if (type == 18)
                {
                    Main.soundInstanceCoins = Main.soundCoins.CreateInstance();
                    Main.soundInstanceCoins.Volume = vol;
                    Main.soundInstanceCoins.Pan = pan;
                    Main.soundInstanceCoins.Play();
                }
                else if (type == 19)
                {
                    if (Main.soundInstanceSplash[newStyle].State == SoundState.Stopped)
                    {
                        Main.soundInstanceSplash[newStyle] = Main.soundSplash[newStyle].CreateInstance();
                        Main.soundInstanceSplash[newStyle].Volume = vol;
                        Main.soundInstanceSplash[newStyle].Pan = pan;
                        Main.soundInstanceSplash[newStyle].Pitch = Main.rand.Next(-10, 11) * 0.01f;
                        Main.soundInstanceSplash[newStyle].Play();
                    }
                }
                else if (type == 20)
                {
                    n = Main.rand.Next(3);
                    Main.soundInstanceFemaleHit[n].Stop();
                    Main.soundInstanceFemaleHit[n] = Main.soundFemaleHit[n].CreateInstance();
                    Main.soundInstanceFemaleHit[n].Volume = vol;
                    Main.soundInstanceFemaleHit[n].Pan = pan;
                    Main.soundInstanceFemaleHit[n].Play();
                }
                else if (type == 21)
                {
                    n = Main.rand.Next(3);
                    Main.soundInstanceTink[n].Stop();
                    Main.soundInstanceTink[n] = Main.soundTink[n].CreateInstance();
                    Main.soundInstanceTink[n].Volume = vol;
                    Main.soundInstanceTink[n].Pan = pan;
                    Main.soundInstanceTink[n].Play();
                }
                else if (type == 22)
                {
                    Main.soundInstanceUnlock.Stop();
                    Main.soundInstanceUnlock = Main.soundUnlock.CreateInstance();
                    Main.soundInstanceUnlock.Volume = vol;
                    Main.soundInstanceUnlock.Pan = pan;
                    Main.soundInstanceUnlock.Play();
                }
                else if (type == 23)
                {
                    Main.soundInstanceDrown.Stop();
                    Main.soundInstanceDrown = Main.soundDrown.CreateInstance();
                    Main.soundInstanceDrown.Volume = vol;
                    Main.soundInstanceDrown.Pan = pan;
                    Main.soundInstanceDrown.Play();
                }
                else if (type == 24)
                {
                    Main.soundInstanceChat = Main.soundChat.CreateInstance();
                    Main.soundInstanceChat.Volume = vol;
                    Main.soundInstanceChat.Pan = pan;
                    Main.soundInstanceChat.Play();
                }
                else if (type == 25)
                {
                    Main.soundInstanceMaxMana = Main.soundMaxMana.CreateInstance();
                    Main.soundInstanceMaxMana.Volume = vol;
                    Main.soundInstanceMaxMana.Pan = pan;
                    Main.soundInstanceMaxMana.Play();
                }
                else if (type == 26)
                {
                    n = Main.rand.Next(3, 5);
                    Main.soundInstanceZombie[n] = Main.soundZombie[n].CreateInstance();
                    Main.soundInstanceZombie[n].Volume = vol * 0.9f;
                    Main.soundInstanceZombie[n].Pan = pan;
                    Main.soundInstanceZombie[n].Pitch = Main.rand.Next(-10, 11) * 0.01f;
                    Main.soundInstanceZombie[n].Play();
                }
                else if (type == 27)
                {
                    if (Main.soundInstancePixie.State == SoundState.Playing)
                    {
                        Main.soundInstancePixie.Volume = vol;
                        Main.soundInstancePixie.Pan = pan;
                        Main.soundInstancePixie.Pitch = Main.rand.Next(-10, 11) * 0.01f;
                    }
                    else
                    {
                        Main.soundInstancePixie.Stop();
                        Main.soundInstancePixie = Main.soundPixie.CreateInstance();
                        Main.soundInstancePixie.Volume = vol;
                        Main.soundInstancePixie.Pan = pan;
                        Main.soundInstancePixie.Pitch = Main.rand.Next(-10, 11) * 0.01f;
                        Main.soundInstancePixie.Play();
                    }
                }
                else if (type == 28)
                {
                    if (Main.soundInstanceMech[newStyle].State != SoundState.Playing)
                    {
                        Main.soundInstanceMech[newStyle] = Main.soundMech[newStyle].CreateInstance();
                        Main.soundInstanceMech[newStyle].Volume = vol;
                        Main.soundInstanceMech[newStyle].Pan = pan;
                        Main.soundInstanceMech[newStyle].Pitch = Main.rand.Next(-10, 11) * 0.01f;
                        Main.soundInstanceMech[newStyle].Play();
                    }
                }
                else if (type == 29)
                {
                    if (newStyle >= 24 && newStyle <= 87)
                        vol *= 0.5f;
                    if (newStyle >= 88 && newStyle <= 91)
                        vol *= 0.7f;
                    if (newStyle >= 93 && newStyle <= 99)
                        vol *= 0.4f;
                    if (newStyle == 92)
                        vol *= 0.5f;
                    if (newStyle == 103)
                        vol *= 0.4f;
                    if (newStyle == 104)
                        vol *= 0.55f;
                    if (newStyle == 100 || newStyle == 101)
                        vol *= 0.25f;
                    if (newStyle == 102)
                        vol *= 0.4f;

                    if (Main.soundInstanceZombie[newStyle].State != SoundState.Playing)
                    {
                        Main.soundInstanceZombie[newStyle] = Main.soundZombie[newStyle].CreateInstance();
                        Main.soundInstanceZombie[newStyle].Volume = vol;
                        Main.soundInstanceZombie[newStyle].Pan = pan;
                        Main.soundInstanceZombie[newStyle].Pitch = Main.rand.Next(-10, 11) * 0.01f;
                        Main.soundInstanceZombie[newStyle].Play();
                    }
                }
                else if (type == 30) // ambient
                {
                    newStyle = Main.rand.Next(10, 12);
                    if (Main.rand.Next(300) == 0)
                    {
                        newStyle = 12;

                        if (Main.soundInstanceZombie[newStyle].State == SoundState.Playing)
                            return;
                    }

                    Main.soundInstanceZombie[newStyle] = Main.soundZombie[newStyle].CreateInstance();
                    Main.soundInstanceZombie[newStyle].Volume = vol * 0.75f;
                    Main.soundInstanceZombie[newStyle].Pan = pan;
                    if (newStyle != 12)
                        Main.soundInstanceZombie[newStyle].Pitch = Main.rand.Next(-70, 1) * 0.01f;
                    else
                        Main.soundInstanceZombie[newStyle].Pitch = Main.rand.Next(-40, 21) * 0.01f;

                    Main.soundInstanceZombie[newStyle].Play();
                }
                else if (type == 31) // ambient
                {
                    newStyle = 13;
                    Main.soundInstanceZombie[newStyle] = Main.soundZombie[newStyle].CreateInstance();
                    Main.soundInstanceZombie[newStyle].Volume = vol * 0.35f;
                    Main.soundInstanceZombie[newStyle].Pan = pan;
                    Main.soundInstanceZombie[newStyle].Pitch = Main.rand.Next(-40, 21) * 0.01f;
                    Main.soundInstanceZombie[newStyle].Play();
                }
                else if (type == 32) // ambient
                {
                    if (Main.soundInstanceZombie[newStyle].State != SoundState.Playing)
                    {
                        Main.soundInstanceZombie[newStyle] = Main.soundZombie[newStyle].CreateInstance();
                        Main.soundInstanceZombie[newStyle].Volume = vol * 0.15f;
                        Main.soundInstanceZombie[newStyle].Pan = pan;
                        Main.soundInstanceZombie[newStyle].Pitch = Main.rand.Next(-70, 26) * 0.01f;
                        Main.soundInstanceZombie[newStyle].Play();
                    }
                }
                else if (type == 33) // ambient
                {
                    newStyle = 15;
                    if (Main.soundInstanceZombie[newStyle].State != SoundState.Playing)
                    {
                        Main.soundInstanceZombie[newStyle] = Main.soundZombie[newStyle].CreateInstance();
                        Main.soundInstanceZombie[newStyle].Volume = vol * 0.2f;
                        Main.soundInstanceZombie[newStyle].Pan = pan;
                        Main.soundInstanceZombie[newStyle].Pitch = Main.rand.Next(-10, 31) * 0.01f;
                        Main.soundInstanceZombie[newStyle].Play();
                    }
                }
                else if (type == 34) // ambient
                {
                    float volMult = Math.Min(newStyle / 50f, 1f);

                    vol *= volMult * 0.2f;

                    if (vol <= 0f || x == -1 || y == -1)
                        if (Main.soundInstanceLiquid[0].State == SoundState.Playing)
                            Main.soundInstanceLiquid[0].Stop();

                        else if (Main.soundInstanceLiquid[0].State == SoundState.Playing)
                        {
                            Main.soundInstanceLiquid[0].Volume = vol;
                            Main.soundInstanceLiquid[0].Pan = pan;
                            Main.soundInstanceLiquid[0].Pitch = -0.2f;
                        }
                        else
                        {
                            Main.soundInstanceLiquid[0] = Main.soundLiquid[0].CreateInstance();
                            Main.soundInstanceLiquid[0].Volume = vol;
                            Main.soundInstanceLiquid[0].Pan = pan;
                            Main.soundInstanceLiquid[0].Play();
                        }
                }
                else if (type == 35) // ambient
                {
                    float volMult = Math.Min(newStyle / 50f, 1f);

                    vol *= volMult * 0.65f;

                    if (vol <= 0f || x == -1 || y == -1)
                        if (Main.soundInstanceLiquid[1].State == SoundState.Playing)
                            Main.soundInstanceLiquid[1].Stop();

                        else if (Main.soundInstanceLiquid[1].State == SoundState.Playing)
                        {
                            Main.soundInstanceLiquid[1].Volume = vol;
                            Main.soundInstanceLiquid[1].Pan = pan;
                            Main.soundInstanceLiquid[1].Pitch = -0f;
                        }
                        else
                        {
                            Main.soundInstanceLiquid[1] = Main.soundLiquid[1].CreateInstance();
                            Main.soundInstanceLiquid[1].Volume = vol;
                            Main.soundInstanceLiquid[1].Pan = pan;
                            Main.soundInstanceLiquid[1].Play();
                        }
                }
                else if (type == 36)
                {
                    if (style == -1)
                        newStyle = 0;

                    Main.soundInstanceRoar[newStyle] = Main.soundRoar[newStyle].CreateInstance();
                    Main.soundInstanceRoar[newStyle].Volume = vol;
                    Main.soundInstanceRoar[newStyle].Pan = pan;
                    if (style == -1)
                        Main.soundInstanceRoar[newStyle].Pitch += 0.6f;

                    Main.soundInstanceRoar[newStyle].Play();
                }
                else if (type == 37)
                {
                    n = Main.rand.Next(57, 59);
                    vol *= style * 0.05f;
                    Main.soundInstanceItem[n] = Main.soundItem[n].CreateInstance();
                    Main.soundInstanceItem[n].Volume = vol;
                    Main.soundInstanceItem[n].Pan = pan;
                    Main.soundInstanceItem[n].Pitch = Main.rand.Next(-40, 41) * 0.01f;
                    Main.soundInstanceItem[n].Play();
                }
                else if (type == 38)
                {
                    n = Main.rand.Next(5);
                    Main.soundInstanceCoin[n] = Main.soundCoin[n].CreateInstance();
                    Main.soundInstanceCoin[n].Volume = vol;
                    Main.soundInstanceCoin[n].Pan = pan;
                    Main.soundInstanceCoin[n].Pitch = Main.rand.Next(-40, 41) * 0.002f;
                    Main.soundInstanceCoin[n].Play();
                }
                else if (type == 39) // ambient
                {
                    Main.soundInstanceDrip[style] = Main.soundDrip[style].CreateInstance();
                    Main.soundInstanceDrip[style].Volume = vol * 0.5f;
                    Main.soundInstanceDrip[style].Pan = pan;
                    Main.soundInstanceDrip[style].Pitch = Main.rand.Next(-30, 31) * 0.01f;
                    Main.soundInstanceDrip[style].Play();
                }
                else if (type == 40)
                {
                    Main.soundInstanceCamera.Stop();
                    Main.soundInstanceCamera = Main.soundCamera.CreateInstance();
                    Main.soundInstanceCamera.Volume = vol;
                    Main.soundInstanceCamera.Pan = pan;
                    Main.soundInstanceCamera.Play();
                }
                else if (type == 41)
                {
                    Main.soundInstanceMoonlordCry = Main.soundNPCKilled[10].CreateInstance();
                    Main.soundInstanceMoonlordCry.Volume = 1f / (1f + (new Vector2(x, y) - Main.player[Main.myPlayer].position).Length());
                    Main.soundInstanceMoonlordCry.Pan = pan;
                    Main.soundInstanceMoonlordCry.Pitch = Main.rand.Next(-10, 11) * 0.01f;
                    Main.soundInstanceMoonlordCry.Play();
                }
            }
        }
        internal static void OnPlaySound(int type, int x, int y, int style)
        {

        }
    }
}
