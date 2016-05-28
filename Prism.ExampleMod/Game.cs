using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.API.Behaviours;
using Terraria;

namespace Prism.ExampleMod
{
    sealed class Game : GameBehaviour
    {
        public static KeyboardState prevKeyState = new KeyboardState();
        public static int meowmaritusHappyFunCount = -1;
        public static int meowmaritusHappyFunTimeBytes = 0x14018E;

        //TODO: put this somewhere in Prism
        public static bool GetKey(Keys k)
        {
            return Main.keyState.IsKeyDown(k);
        }
        public static bool GetKey(Keys k, KeyState onEnterKeyState)
        {
            return onEnterKeyState == Main.keyState[k] && Main.keyState[k] != prevKeyState[k];
        }

        public static Vector2 GetRandomPositionOnScreen()
        {
            return new Vector2(Main.screenPosition.X + (float)Main.rand.NextDouble() * Main.screenWidth, Main.screenPosition.Y + (float)Main.rand.NextDouble() * Main.screenHeight);
        }

        public override void PostUpdate()
        {
            if (Main.gameMenu || !Main.hasFocus || TMain.ChatMode)
                return;

            var p = Main.player[Main.myPlayer];
            Point me = p.Center.ToPoint();

            #region asdfasdfasdf
            if (GetKey(Keys.I, KeyState.Down))
                foreach (var kvp in ExampleMod.Mod.TestItems)
                    Item.NewItem(me.X, me.Y, 1, 1, kvp.Key, kvp.Value, false, 0, true, false);
            #endregion

            #region I dare you to press L
            if (GetKey(Keys.L, KeyState.Down))
                meowmaritusHappyFunCount = (byte)(meowmaritusHappyFunTimeBytes >> 16);

            if (!Main.player[Main.myPlayer].dead)
            {
                if (meowmaritusHappyFunCount-- > 0)
                {
                    NPC.defaultMaxSpawns *= (byte)(meowmaritusHappyFunTimeBytes >> 16);
                    NPC.maxSpawns *= (byte)(meowmaritusHappyFunTimeBytes >> 16);

                    for (int i = 0; i < Main.maxNPCs; i++)
                        if (!Main.npc[i].active)
                        {
                            Main.npc[i] = new NPC();
                            Main.npc[i].SetDefaults((short)meowmaritusHappyFunTimeBytes, -1);
                            Main.npc[i].active = true;
                            Main.npc[i].timeLeft = NPC.activeTime;
                            Main.npc[i].position = GetRandomPositionOnScreen();
                            Main.npc[i].ai[1] = 59f;
                            Main.PlaySound(29, (int)Main.npc[i].Center.X, (int)Main.npc[i].Center.Y, 92);

                            break;
                        }
                }
            }
            #endregion

            #region spawn custom npcs
            if (GetKey(Keys.N, KeyState.Down))
            {
                var pt = GetRandomPositionOnScreen().ToPoint();

                foreach (var kvp in ExampleMod.Mod.TestNpcs)
                    for (int i = 0; i < kvp.Value; i++)
                        NPC.NewNPC(pt.X, pt.Y, kvp.Key);
            }

            if (GetKey(Keys.B, KeyState.Down))
            {
                foreach (var kvp in ExampleMod.Mod.TestBosses)
                    for (int i = 0; i < kvp.Value; i++)
                        NPC.SpawnOnPlayer(Main.myPlayer, kvp.Key);
            }
            #endregion

            prevKeyState = Main.keyState;
        }

        public override void PostDrawBackground(SpriteBatch sb)
        {
            sb.Draw(Main.itemTexture[1], Vector2.Zero, Color.White);
        }
    }
}
