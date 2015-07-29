using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.API;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;

namespace Prism.ExampleMod
{
    public class Mod : ModDef
    {
        public static KeyboardState prevKeyState = new KeyboardState();
        public static int meowmaritusHappyFunCount = -1;
        public static int meowmaritusHappyFunTimeBytes = 0x14018E;

        protected override Dictionary<string, ItemDef      > GetItemDefs      ()
        {
            return new Dictionary<string, ItemDef>
            {
                // Pizza done with JSON method using an external resource
                { "Pizza", new ItemDef("Pizza", GetResource<JsonData>("Resources/Items/Pizza.json"),
                    () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png")) },
                // Ant done with JSON method using an embedded resource
                { "Ant", new ItemDef("Ant", GetEmbeddedResource<JsonData>("Resources/Items/Ant.json"),
                    () => GetEmbeddedResource<Texture2D>("Resources/Textures/Items/Ant.png")) },
                { "Pizzant", new ItemDef("Pizzant", () => GetResource<Texture2D>("Resources/Textures/Items/Pizzant.png"),
                    description: new ItemDescription("The chaotic forces of italian spices and insects and bread.", expert: true),
                    damageType: ItemDamageType.Melee,
                    autoReuse: true,
                    useTime: 12,
                    reuseDelay: 0,
                    useAnimation: 8,
                    maxStack: 1,
                    rare: ItemRarity.Yellow,
                    useSound: 1,
                    damage: 80,
                    knockback: 4f,
                    width: 30,
                    height: 30,
                    useTurn: false,
                    useStyle: ItemUseStyle.Stab,
                    holdStyle: ItemHoldStyle.Default,
                    value: new CoinValue(1, 34, 1, 67),
                    scale: 1.1f
                ) },
                //The *not fucking terrible* way to make a new item def in code (you can actually see the XmlDoc's of the fields this way and also it's not ugly camelCase):
                { "Pizzantzioli", new ItemDef("Pizzantzioli") {
                    Description = new ItemDescription("The forces of ants and pizza come together as one.", "The name is Italian for 'KICKING ASS'! YEAH! BROFISSSSST!!1!", expert: true),
                    GetTexture = () => GetResource<Texture2D>("Resources/Textures/Items/Pizzantzioli.png"),
                    DamageType = ItemDamageType.Melee,
                    AutoReuse = true,
                    UseTime = 20,
                    ReuseDelay = 0,
                    UseAnimation = 16,
                    MaxStack = 1,
                    Rarity = ItemRarity.Cyan,
                    UseSound = 1,
                    Damage = 150,
                    Knockback = 10f,
                    Width = 30,
                    Height = 30,
                    TurnPlayerOnUse = false,
                    UseStyle = ItemUseStyle.Swing,
                    HoldStyle = ItemHoldStyle.Default,
                    Value = new CoinValue(2, 51, 3, 9),
                    Scale = 1.1f
                } }
            };
        }
        protected override Dictionary<string, NpcDef       > GetNpcDefs       ()
        {
            return new Dictionary<string, NpcDef>
            {
                { "PizzaNPC", new NpcDef("Pizza NPC", getTex: () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"),
                    lifeMax: 10000,
                    frameCount: 1,
                    damage: 1,
                    width: 128,
                    height: 128,
                    alpha: 0,
                    scale: 1.0f,
                    noTileCollide: true,
                    color: Color.White,
                    value: new NpcValue((CoinValue)0),
                    aiStyle: NpcAiStyle.FlyingHead
                    ) }
            };
        }
        protected override Dictionary<string, ProjectileDef> GetProjectileDefs()
        {
            return new Dictionary<string, ProjectileDef>
            {
                { "PizzaProjectile", new ProjectileDef("Pizza", getTex: () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png")
                    ) }
            };
        }

        protected override IEnumerable<RecipeDef> GetRecipeDefs()
        {
            return new[]
            {
                new RecipeDef(
                    new ItemRef("Pizza", Info.InternalName), 8,
                    new RecipeItems
                    {
                        { new ItemRef(ItemID.Gel), 30 }
                    }
                ),
                new RecipeDef(
                    new ItemRef("Ant", Info.InternalName), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza", Info.InternalName),  1 },
                        { new ItemRef(ItemID.Gel                ), 20 }
                    }
                ),
                new RecipeDef(
                    new ItemRef("Pizzant", Info.InternalName), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza", Info.InternalName), 1 },
                        { new ItemRef("Ant"  , Info.InternalName), 1 },
                        { new ItemRef(ItemID.Gel                ), 4 }
                    },
                    new[] { TileID.TinkerersWorkbench }
                ),
                new RecipeDef(
                    new ItemRef("Pizzantzioli", Info.InternalName), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza"  , Info.InternalName), 3 },
                        { new ItemRef("Pizzant", Info.InternalName), 1 },
                        { new ItemRef(ItemID.Gel                  ), 4 }
                    },
                    new[] { TileID.Dirt }
                )
            };
        }

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
            if (Main.gameMenu || !Main.hasFocus)
                return;

            var p = Main.player[Main.myPlayer];

            #region CHEATERRRRRRRRRR
            if (GetKey(Keys.G))
            {
                Point me = Main.player[Main.myPlayer].Center.ToPoint();
                Item.NewItem(me.X, me.Y, 1, 1, ItemID.Gel, 10, false, 0, true, false);
            }
            #endregion

            #region imma tell on u O.o
            if (GetKey(Keys.I, KeyState.Down))
            {
                Point me = Main.player[Main.myPlayer].Center.ToPoint();
                Item.NewItem(me.X, me.Y, 1, 1, ItemID.Gel, 10, false, 0, true, false);
            }
            #endregion

            #region I dare you to press L
            if (GetKey(Keys.L, KeyState.Down))
            {
                meowmaritusHappyFunCount = (byte)(meowmaritusHappyFunTimeBytes >> 16);
            }

            if (!Main.player[Main.myPlayer].dead)
            {
                if (meowmaritusHappyFunCount-- > 0)
                {
                    NPC.defaultMaxSpawns *= (byte)(meowmaritusHappyFunTimeBytes >> 16);
                    NPC.maxSpawns *= (byte)(meowmaritusHappyFunTimeBytes >> 16);

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i] != null && !Main.npc[i].active)
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
            }
            #endregion

            #region spawn custom npcs
            if (GetKey(Keys.N, KeyState.Down))
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i] != null && !Main.npc[i].active)
                    {
                        Main.npc[i] = new NPC();
                        Main.npc[i].SetDefaults(NpcDef.ByName["PizzaNPC", Info.InternalName].Type);
                        Main.npc[i].active = true;
                        Main.npc[i].timeLeft = NPC.activeTime;
                        Main.npc[i].position = GetRandomPositionOnScreen();

                        break;
                    }
                }
            }
            #endregion

            prevKeyState = Main.keyState;
        }
    }
}
