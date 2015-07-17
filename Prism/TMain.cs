using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.Debugging;
using Terraria;
using Terraria.ID;

namespace Prism
{
    public sealed class TMain : Main
    {
        readonly static Color TraceBgColour = new Color(0, 43, 54, 175);
        static Texture2D WhitePixel;
        static Texture2D PizzaAndAntSword;

        static int nextItemID = ItemID.Count;

        static bool justDrawCrashed = false;

        int PnAS_ID;

        bool hasPnAS = false;

        internal TMain()
            : base()
        {
            versionNumber += ", Prism v" + AssemblyInfo.VERSION;

            SavePath += "\\Prism";
            PlayerPath = SavePath + "\\Players";
            WorldPath = SavePath + "\\Worlds";
        }

        static int AddItem()
        {
            Array.Resize(ref itemTexture, itemTexture.Length + 1);
            Array.Resize(ref itemAnimations, itemAnimations.Length + 1);
            Array.Resize(ref itemFlameLoaded, itemFlameLoaded.Length + 1);
            Array.Resize(ref itemFlameTexture, itemFlameTexture.Length + 1);
            Array.Resize(ref itemFrame, itemFrame.Length + 1);
            Array.Resize(ref itemFrameCounter, itemFrameCounter.Length + 1);

            Array.Resize(ref Item.bodyType, Item.bodyType.Length + 1);
            Array.Resize(ref Item.claw, Item.claw.Length + 1);
            Array.Resize(ref Item.headType, Item.headType.Length + 1);
            Array.Resize(ref Item.legType, Item.legType.Length + 1);
            Array.Resize(ref Item.staff, Item.staff.Length + 1);

            Array.Resize(ref ItemID.Sets.AnimatesAsSoul, ItemID.Sets.AnimatesAsSoul.Length + 1);
            Array.Resize(ref ItemID.Sets.Deprecated, ItemID.Sets.Deprecated.Length + 1);
            Array.Resize(ref ItemID.Sets.ExoticPlantsForDyeTrade, ItemID.Sets.ExoticPlantsForDyeTrade.Length + 1);
            Array.Resize(ref ItemID.Sets.ExtractinatorMode, ItemID.Sets.ExtractinatorMode.Length + 1);
            Array.Resize(ref ItemID.Sets.gunProj, ItemID.Sets.gunProj.Length + 1);
            Array.Resize(ref ItemID.Sets.ItemIconPulse, ItemID.Sets.ItemIconPulse.Length + 1);
            Array.Resize(ref ItemID.Sets.ItemNoGravity, ItemID.Sets.ItemNoGravity.Length + 1);
            Array.Resize(ref ItemID.Sets.NebulaPickup, ItemID.Sets.NebulaPickup.Length + 1);
            Array.Resize(ref ItemID.Sets.NeverShiny, ItemID.Sets.NeverShiny.Length + 1);
            Array.Resize(ref ItemID.Sets.StaffMinionSlotsRequired, ItemID.Sets.StaffMinionSlotsRequired.Length + 1);

            return nextItemID++;
        }

        protected override void Initialize()
        {
            Item.OnSetDefaults += (Item i, int t, bool nmc) =>
            {
                if (t >= ItemID.Count)
                {
                    i.RealSetDefaults(0, nmc);

                    i.type = i.netID = t;
                    i.width = i.height = 16;
                    i.stack = i.maxStack = 1;

                    if (t == PnAS_ID)
                    {
                        i.name = "Pizza & Ant Sword";
                        i.toolTip = "This is a custom item! Woo!";
                        i.maxStack = 5;
                    }
                    else
                    {
                        // etc
                    }
                }
                else
                    i.RealSetDefaults(t, nmc);
            };

            base.Initialize();

            new Item().SetDefaults(ItemID.Count /* Pizza & Ant Sword */);
        }

        protected override void   LoadContent()
        {
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            PnAS_ID = AddItem();

            base.  LoadContent();

            itemTexture[PnAS_ID] = PizzaAndAntSword = Texture2D.FromStream(GraphicsDevice, Assembly.GetExecutingAssembly().GetManifestResourceStream("Prism.Pizza & Ant Sword.png"));
        }
        protected override void UnloadContent()
        {
            WhitePixel.Dispose();
            WhitePixel = null;

            PizzaAndAntSword.Dispose();
            PizzaAndAntSword = null;

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

                PrismDebug.Update();

                if (keyState.IsKeyDown(Keys.Y) && !hasPnAS && !gameMenu && hasFocus)
                {
                    var inv = player[myPlayer].inventory;

                    for (int i = 0; i < inv.Length; i++)
                    {
                        if (inv[i].type == 0)
                        {
                            inv[i].SetDefaults(PnAS_ID);
                            break;
                        }
                        if (inv[i].type == PnAS_ID && inv[i].stack < inv[i].maxStack)
                        {
                            inv[i].stack++;
                            break;
                        }
                    }

                    hasPnAS = true;
                }
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
            }
            catch (Exception e)
            {
                if (justDrawCrashed)
                    ExceptionHandler.HandleFatal(e); // drawing state got fucked up
                else
                {
                    justDrawCrashed = true;

                    ExceptionHandler.Handle(e);
                }
            }
        }
    }
}
