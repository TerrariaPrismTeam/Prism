using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.API;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;
using Prism.Mods;
using LitJson;


namespace Prism.ExampleMod
{
    public class Mod : ModDef
    {
        public static KeyboardState prevKeyState = new KeyboardState();
        public static int meowmaritusHappyFunCount = -1;
        public static int meowmaritusHappyFunTimeBytes = 0x14018E;

        protected override Dictionary<string, ProjectileDef> GetProjectileDefs()
        {
            return new Dictionary<string, ProjectileDef>
            {
                { "PizzaProjectile", new ProjectileDef(

                ) }
            };
        }

        protected override Dictionary<string, ItemDef> GetItemDefs()
        {
            return new Dictionary<string, ItemDef>
            {
                // Pizza done with JSON method using an external resource
                { "Pizza", new ItemDef("Pizza", GetResource<JsonData>("Resources\\Items\\Pizza.json"),
                    getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizza.png")) },
                /* Pizza done with pure code method
                { "Pizza", new ItemDef("Pizza", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizza.png"),
                    description: new ItemDescription("LOTZA SPA-pizza. It's pizza.", "'MMmmmmmmm'", false, true),
                    useTime: 15,
                    reuseDelay: 0,
                    useAnimation: 15,
                    consumable: true,
                    maxStack: 999,
                    rare: ItemRarity.Lime,
                    useSound: 2,
                    useStyle: ItemUseStyle.Eat,
                    holdStyle: ItemHoldStyle.HeldLightSource,
                    healLife: 400,
                    useTurn: true,
                    value: new CoinValue(50, 10, 2),
                    buff: new BuffDef(BuffID.WellFed, 60 * 60 * 30)
                    ) },
                */
                // Ant done with JSON method using an embedded resource
                { "Ant", new ItemDef("Ant", GetResource<JsonData>("Resources\\Items\\Ant.json"),
                    getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Ant.png")) },
                /* Ant done with pure code method
                { "Ant", new ItemDef("Ant", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Ant.png"),
                    description: new ItemDescription("By ants, for ants.", "'B-but...ants aren't this big!'", false, true),
                    damageType: DamageType.Melee,
                    autoReuse: true,
                    useTime: 6,
                    reuseDelay: 6,
                    useAnimation: 6,
                    maxStack: 1,
                    rare: ItemRarity.LightPurple,
                    useSound: 1,
                    damage: 60,
                    knockback: 4f,
                    width: 30,
                    height: 30,
                    useTurn: true,
                    useStyle: ItemUseStyle.Stab,
                    holdStyle: ItemHoldStyle.Default,
                    value: new CoinValue(0, 40, 8, 25),
                    scale: 1.1f
                    ) },
                */
                { "Pizzant", new ItemDef("Pizzant", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizzant.png"),
                    description: new ItemDescription("The chaotic forces of italian spices and insects and bread.", "", false, true),
                    damageType: DamageType.Melee,
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
                //The *not fucking terrible* way to make a new item def in code (you can actually see the XmlDoc's of the fields this way and also it's not ugly camel-case):
                { "Pizzantzioli", new ItemDef("Pizzantzioli") {
                    Description = new ItemDescription("The forces of ants and pizza come together as one.", "The name is Italian for 'KICKING ASS'! YEAH! BROFISSSSST!!1!", false, true),
                    GetTexture = () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizzantzioli.png"),
                    DamageType = DamageType.Melee,
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
        protected override Dictionary<string, NpcDef > GetNpcDefs ()
        {
            return new Dictionary<string, NpcDef>
            {

                { "PizzaNPC", new NpcDef("Pizza NPC", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizza.png"),
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
                    aiStyle: NpcAiStyle.Plantera,
                    alwaysDraw: true
                    ) }
            };
        }
        
        public override void OnLoad()
        {
            ExceptionHandler.DetailedExceptions = true;

            Recipes.Create(ItemDef.ByName["Pizza", Info.InternalName], 8,
                ItemDef.ByType[ItemID.Gel], 30);

            Recipes.Create(ItemDef.ByName["Ant", Info.InternalName], 1,
                ItemDef.ByName["Pizza", Info.InternalName], 1,
                ItemDef.ByType[ItemID.Gel], 20);

            Recipes.Create(ItemDef.ByName["Pizzant", Info.InternalName], 1,
                ItemDef.ByName["Pizza", Info.InternalName], 1,
                ItemDef.ByName["Ant", Info.InternalName], 1,
                ItemDef.ByType[ItemID.Gel], 4,
                RecipeRequires.Tile, TileID.WorkBenches); // You clearly need a workbench to stab pizza with an ant mandible.

            Recipes.Create(ItemDef.ByName["Pizzantzioli", Info.InternalName], 1,
                ItemDef.ByName["Pizza", Info.InternalName], 3,
                ItemDef.ByName["Pizzant", Info.InternalName], 1,
                ItemDef.ByType[ItemID.Gel], 4,
                RecipeRequires.Tile, TileID.Dirt); // Collect ants from your nearest ant hill.
        }

        public static bool GetKey(Keys k)
        {
            return Main.keyState.IsKeyDown(k);
        }

        public static bool GetKey(Keys k, KeyState onEnterKeyState)
        {
            return (onEnterKeyState == Main.keyState[k] && Main.keyState[k] != prevKeyState[k]);
        }

        public static Vector2 GetRandomPositionOnScreen()
        {
            return new Vector2(Main.screenPosition.X + ((float)Main.rand.NextDouble() * Main.screenWidth), Main.screenPosition.Y + ((float)Main.rand.NextDouble() * Main.screenHeight));
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
                if (meowmaritusHappyFunCount > 0)
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
                else
                {
                    meowmaritusHappyFunCount--;
                }
            }
            #endregion

            #region spawn custom npcs
            if (GetKey(Keys.U, KeyState.Down))
            {
                NPC.SpawnOnPlayer(Main.myPlayer, NpcDef.ByName["PizzaNPC", Info.InternalName].Type);
            }
            #endregion

            prevKeyState = Main.keyState;
        }
    }
}
