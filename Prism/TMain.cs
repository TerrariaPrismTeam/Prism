using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.Debugging;
using Prism.Mods;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism
{
    public sealed class TMain : Main
    {
        readonly static Color TraceBgColour = new Color(0, 43, 54, 175);
        static Texture2D WhitePixel;

        // todo: move these to somewhere e
        static int nextItemID = ItemID.Count;

        static bool justDrawCrashed = false;

        int PizzaAndAntSword_ID, PizzaAntscalibur_ID;
        bool hasPizzaAndAntSword = false, hasPizzaAntscalibur = false;
        Texture2D PizzaAndAntSword, PizzaAntscalibur;

        internal TMain()
            : base()
        {
            versionNumber += ", Prism v" + PrismApi.Version;
            if (PrismApi.VersionType != VersionType.Normal)
                versionNumber += " " + PrismApi.VersionType;

            SavePath += "\\Prism";
            PlayerPath = SavePath + "\\Players";
            WorldPath = SavePath + "\\Worlds";

            PrismApi.ModDirectory = SavePath + "\\Mods";

            CloudPlayerPath = "players_Prism";
            CloudWorldPath = "worlds_Prism";
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

                    if (t == PizzaAndAntSword_ID)
                    {
                        i.name = "Pizza & Ant Sword";
                        i.toolTip = "This is a custom item! Woo!";
                        i.autoReuse = true;
                        i.maxStack = 5;
                        i.rare = 10;
                        i.useSound = 1;
                        i.useStyle = 1;
                        i.damage = 80;
                        i.knockBack = 4;
                        i.useAnimation = 20;
                        i.useTime = 15;
                        i.width = 30;
                        i.height = 30;
                        i.melee = true;
                        i.scale = 1.1f;
                        i.value = Item.sellPrice(0, 50, 0, 0);
                    }
                    else if (t == PizzaAntscalibur_ID)
                    {
                        i.name = "Pizza Antscalibur";
                        i.toolTip = "Contains the mystical power of pizza and ants.";
                        i.toolTip2 = "...also a custom item!";
                        i.autoReuse = true;
                        i.maxStack = 1;
                        i.rare = 10;
                        i.useSound = 1;
                        i.useStyle = 1;
                        i.damage = 150;
                        i.knockBack = 10;
                        i.useAnimation = 16;
                        i.useTime = 20;
                        i.width = 30;
                        i.height = 30;
                        i.melee = true;
                        i.scale = 1.1f;
                        i.value = Item.sellPrice(0, 500, 0, 0);
                    }
                    else
                    {
                        // etc
                    }
                }
                else
                    i.RealSetDefaults(t, nmc);
            };

            base.Initialize(); // terraria init and LoadContent happen here

            // setdefaults tests
            new Item().SetDefaults(PizzaAndAntSword_ID /* Pizza & Ant Sword */);
            new Item().SetDefaults(PizzaAntscalibur_ID /* Pizza Antscalibur */);
        }

        protected override void   LoadContent()
        {
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });

            PizzaAndAntSword_ID = AddItem();
            PizzaAntscalibur_ID = AddItem();

            base.  LoadContent();

            itemTexture[PizzaAndAntSword_ID] = PizzaAndAntSword = Texture2D.FromStream(GraphicsDevice, Assembly.GetExecutingAssembly().GetManifestResourceStream("Prism.Pizza & Ant Sword.png"));
            itemTexture[PizzaAntscalibur_ID] = PizzaAntscalibur = Texture2D.FromStream(GraphicsDevice, Assembly.GetExecutingAssembly().GetManifestResourceStream("Prism.Pizza Antscalibur.png"));
        }
        protected override void UnloadContent()
        {
            WhitePixel.Dispose();
            WhitePixel = null;

            PizzaAndAntSword.Dispose();
            PizzaAndAntSword = null;

            PizzaAntscalibur.Dispose();
            PizzaAntscalibur = null;

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

                if (keyState.IsKeyDown(Keys.Y) && !(hasPizzaAndAntSword && hasPizzaAntscalibur) && !gameMenu && hasFocus)
                {
                    var inv = player[myPlayer].inventory;

                    for (int i = 0; i < inv.Length; i++)
                    {
                        if (inv[i].type == 0)
                        {
                            if (!hasPizzaAndAntSword)
                            {
                                inv[i].SetDefaults(PizzaAndAntSword_ID);
                                hasPizzaAndAntSword = true;
                                while (inv[i].stack < inv[i].maxStack) inv[i].stack++;
                                continue;
                            }
                            else if (!hasPizzaAntscalibur)
                            {
                                inv[i].SetDefaults(PizzaAntscalibur_ID);
                                hasPizzaAntscalibur = true;
                                while (inv[i].stack < inv[i].maxStack) inv[i].stack++;
                                break;
                            }
                        }

                    }
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
