using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace Prism.DebuggingMod
{
    public class DebugMenu
    {
        internal static bool HasBeenOpened = false;
        public static bool IsOpen = false;
        internal static SpriteFont DebugFont;
        internal static int DebugSelection = 0;
        internal static KeyboardState prevKeyState = new KeyboardState();
        public static DebugMenuNode Node = new DebugMenuNode();
        internal static DebugMenuNode SelectedNode = new DebugMenuNode();
        internal static Ctrl Nav;
        internal static Dictionary<Ctrl, bool> NavDirInit;
        internal static Dictionary<Ctrl, float> NavDirTimer;
        internal static readonly float NavDirDurInit = 0.25f;
        internal static readonly float NavDirDurRepeat = 0.05f;
        internal static readonly Dictionary<Ctrl, Keys> NavDirKey = new Dictionary<Ctrl, Keys>()
        {
            { Ctrl.Up, Keys.I }, { Ctrl.Down, Keys.K }, { Ctrl.Left, Keys.J }, { Ctrl.Right, Keys.L }
        };

        private static void Init()
        {
            using (var dbgFont = new Util.RandTempContentFile<SpriteFont>("Prism.Resources.FakeContent.DebugFont.xnb"))
            {
                DebugFont = dbgFont.Load();
            }
            Node = new DebugMenuNode("Prism Debug Menu");
            NavDirInit = new Dictionary<Ctrl, bool>();
            NavDirTimer = new Dictionary<Ctrl, float>();
            for (byte i = (byte)Ctrl.Up; i <= (byte)Ctrl.Right; i++)
            {
                NavDirInit[(Ctrl)i] = true;
                NavDirTimer[(Ctrl)i] = 0;
            }
        }

        public static Color MainDiscoColor //Seriously why doesn't vanilla have this
        {
            get
            {
                return new Color((byte)Main.DiscoR, (byte)Main.DiscoG, (byte)Main.DiscoB);
            }
        }

        public static Rectangle DebugRect
        {
            get
            {
                return new Rectangle(32, 116, Main.screenWidth - 32, Main.screenHeight - 480);
            }
        }

        public static int DbgModMult
        {
            get
            {
                return ((Nav & Ctrl.x10) == Ctrl.x10 ? 10 : 1) * ((Nav & Ctrl.x100) == Ctrl.x100 ? 100 : 1);
            }
        }

        public static bool GetKey(Keys k) //It's SOMEwhere in Prism now I guess...
        {
            return Main.keyState.IsKeyDown(k);
        }
        public static bool GetKey(Keys k, KeyState onEnterKeyState)
        {
            return onEnterKeyState == Main.keyState[k] && Main.keyState[k] != prevKeyState[k];
        }

        private static void UpdateNav(GameTime gt)
        {
            Nav = Ctrl.None;
            UpdateNavDir(gt);
            Nav |= GetKey(Keys.O, KeyState.Down) ? Ctrl.Enter : Ctrl.None;
            Nav |= GetKey(Keys.U, KeyState.Down) ? Ctrl.Back  : Ctrl.None;
            Nav |= GetKey(Keys.LeftShift       ) ? Ctrl.x10   : Ctrl.None;
            Nav |= GetKey(Keys.LeftAlt         ) ? Ctrl.x100  : Ctrl.None;
        }

        private static void UpdateNavDir(GameTime gt)
        {
            for (byte i = (byte)Ctrl.Up; i <= (byte)Ctrl.Right; i *= 2)
            {
                var tick = true;

                if (GetKey(NavDirKey[(Ctrl)i], KeyState.Down))
                {
                    NavDirTimer[(Ctrl)i] = 0;
                    NavDirInit[(Ctrl)i] = true;
                }
                else if (GetKey(NavDirKey[(Ctrl)i]))
                {
                    if (NavDirInit[(Ctrl)i] && NavDirTimer[(Ctrl)i] >= NavDirDurInit)
                    {
                        NavDirTimer[(Ctrl)i] = 0;
                        NavDirInit[(Ctrl)i] = false;
                    }
                    else if (!NavDirInit[(Ctrl)i] && NavDirTimer[(Ctrl)i] >= NavDirDurRepeat)
                    {
                        NavDirTimer[(Ctrl)i] = 0;
                    }
                    else
                    {
                        NavDirTimer[(Ctrl)i] += (float)gt.ElapsedGameTime.TotalSeconds;
                        tick = false;
                    }
                }
                else
                {
                    tick = false;
                }

                if (tick) Nav |= (Ctrl)i;
            }
        }

        /// <summary>
        /// Call before the debug update hook so they can get the input from this
        /// </summary>
        internal static void Update(GameTime gt)
        {
            if (!Main.gameMenu && (GetKey(Keys.LeftShift) || GetKey(Keys.RightShift)) && (GetKey(Keys.LeftAlt) || GetKey(Keys.RightAlt)) && GetKey(Keys.H, KeyState.Down))
            {
                if (!IsOpen)
                {
                    IsOpen = true;
                    Main.PlaySound(10);
                }
                else
                {
                    IsOpen = false;
                    Main.PlaySound(11);
                }
            }

            if (!HasBeenOpened)
            {
                if (IsOpen)
                {
                    Init();
                    Main.NewText("Debug menu initialized.", 0, 255, 255, true);
                    Main.NewText("Press I/K to move the selection up/down.", 0, 255, 255, true);
                    Main.NewText("Press J/L to adjust the selected entry.", 0, 255, 255, true);
                    Main.NewText("Press U/O to expand/collapse nodes.", 0, 255, 255, true);
                    Main.NewText("Hold Shift/Alt while moving the selection or adjusting the selected entry to multiply the action by 10x/100x", 0, 255, 255, true);
                    HasBeenOpened = true;
                }
                else return;
            }

            UpdateNav(gt);            

            if (IsOpen)
            {


                if ((Nav & Ctrl.Up) == Ctrl.Up)
                {
                    DebugSelection -= DbgModMult;
                    Main.PlaySound(12);
                }

                if ((Nav & Ctrl.Down) == Ctrl.Down)
                {
                    DebugSelection += DbgModMult;
                    Main.PlaySound(12);
                }

                if ((Nav & Ctrl.Enter) == Ctrl.Enter && SelectedNode.Count > 0)
                    if (!SelectedNode.IsExpanded)
                    {
                        SelectedNode.Expand();
                        Main.PlaySound(10);
                    }
                    else
                    {
                        foreach (var c in SelectedNode)
                        {
                            DebugSelection = c.Value.VisibleIndex;
                            Main.PlaySound(12);
                            break;
                        }
                    }

                if ((Nav & Ctrl.Back) == Ctrl.Back)
                    if (SelectedNode.Count > 0 && SelectedNode.IsExpanded)
                    {
                        SelectedNode.Collapse();
                        Main.PlaySound(11);
                    }
                    else if (SelectedNode.Parent != null)
                    {
                        SelectedNode = SelectedNode.Parent;
                        DebugSelection = SelectedNode.VisibleIndex;
                        Main.PlaySound(11);
                    }

                DebugSelection = (int)MathHelper.Clamp(DebugSelection, 0, Node.RecursiveChildVisibleCount);
            }
            prevKeyState = Main.keyState;
        }

        internal static void DrawAll(SpriteBatch sb)
        {
            if (Node.Count == 0 || !HasBeenOpened)
                return;

            Viewport screen = sb.GraphicsDevice.Viewport;

            try
            {
                sb.GraphicsDevice.Viewport = new Viewport(DebugRect);
            }
            catch
            {
                sb.GraphicsDevice.Viewport = screen;
            }

            sb.Begin();

            int count = Node.Draw(Main.spriteBatch, DebugFont, new Vector2(0, DebugRect.Height / 2));

            DebugSelection = (int)MathHelper.Clamp(DebugSelection, 0, count);

            sb.End();

            sb.GraphicsDevice.Viewport = screen;
        }
    }
}
