using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API;
using Prism.API.Audio;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.ExampleMod.Behaviours.NPC;
using Prism.Util;
using Terraria;
using Terraria.ID;

namespace Prism.ExampleMod
{
    sealed class Content : ContentHandler
    {
        protected override Dictionary<string, ItemDef> GetItemDefs()
        {
            return new Dictionary<string, ItemDef>
            {

#pragma warning disable 618
                // Pizza done with JSON method using an external resource
                { "Pizza", new ItemDef("Pizza", GetResource<JsonData>("Resources/Items/Pizza.json"),
                    () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {
                    Mount = new MountRef("NyanCat")
                } },
                // Ant done with JSON method using an embedded resource
                { "Ant", new ItemDef("Ant", GetEmbeddedResource<JsonData>("Resources/Items/Ant.json"),
                    () => GetEmbeddedResource<Texture2D>("Resources/Textures/Items/Ant.png")) },
#pragma warning restore 618
                { "Pizzant", new ItemDef("Pizzant", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizzant.png"))
                {   Description = new ItemDescription("The chaotic forces of italian spices and insects and bread.", expert: true),
                    DamageType = ItemDamageType.Melee,
                    AutoReuse = true,
                    UseTime = 12,
                    ReuseDelay = 0,
                    UseAnimation = 8,
                    MaxStack = 1,
                    Rarity = ItemRarity.Yellow,
                    UseSound = VanillaSfxes.UseItem[1],
                    Damage = 80,
                    Knockback = 4f,
                    Width = 30,
                    Height = 30,
                    TurnPlayerOnUse = false,
                    UseStyle = ItemUseStyle.Stab,
                    HoldStyle = ItemHoldStyle.Default,
                    Value = new CoinValue(1, 34, 1, 67),
                    Scale = 1.1f
                } },
                { "Pizzantzioli", new ItemDef("Pizzantzioli", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizzantzioli.png"))
                {   Description = new ItemDescription("The forces of ants and pizza come together as one.", "The name is Italian for 'KICKING ASS'! YEAH! BROFISSSSST!!1!", expert: true),
                    DamageType = ItemDamageType.Melee,
                    AutoReuse = true,
                    UseTime = 20,
                    ReuseDelay = 0,
                    UseAnimation = 16,
                    MaxStack = 1,
                    Rarity = ItemRarity.Cyan,
                    UseSound = VanillaSfxes.UseItem[1],
                    Damage = 150,
                    Knockback = 10f,
                    Width = 30,
                    Height = 30,
                    TurnPlayerOnUse = false,
                    UseStyle = ItemUseStyle.Swing,
                    HoldStyle = ItemHoldStyle.Default,
                    Value = new CoinValue(2, 51, 3, 9),
                    Scale = 1.1f
                } },
                { "TilePlacer", new ItemDef("ExampleMod tile placer", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {
                    UseAnimation = 15,
                    AutoReuse = true,
                    UseTime = 15,
                    UseStyle = ItemUseStyle.Swing,
                    //MaxStack = 999, // not consumable
                    Width  = 16,
                    Height = 16,
                    CreateTile = new TileRef("TestTile")
                } },
                { "WallPlacer", new ItemDef("ExampleMod wall placer", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {
                    UseAnimation = 15,
                    UseTime = 15,
                    AutoReuse = true,
                    UseStyle = ItemUseStyle.Swing,
                    //MaxStack = 999, // not consumable
                    Width  = 16,
                    Height = 16,
                    CreateWall = new WallRef("TestWall")
                } }
            };
        }
        protected override Dictionary<string, MountDef> GetMountDefs()
        {
            return new Dictionary<string, MountDef>
            {
                { "NyanCat", new MountDef("Nyan Cat", front: new MountTextureData(() => GetEmbeddedResource<Texture2D>("Resources/Textures/Misc/Nyan Cat.png")))
                {
                    Buff = new BuffRef(BuffID.ObsidianSkin), // too lazy to create a separate buff (the player will dismount when the buff runs out - after 60 seconds)
                    // ExtraBuff is set to Buff if it is null

                    AdditionalHeight = 30,
                    MaxFlightTime = 1000,
                    FallDamageMult = 0f,
                    RunSpeed = 13f,
                    Acceleration = 0.075f,
                    JumpHeight = 100,
                    JumpSpeed = 10f,
                    BlockExtraJumps = false,

                    TotalFrames = 3,
                    BodyFrame = 3,

                    AllFrameData = new MountFrameData(3, 12, 0), // use the same animation everywhere
                    SwimSpeed = 5,
                    IdleFrameLoop = true
                } }
            };
        }
        protected override Dictionary<string, NpcDef> GetNpcDefs()
        {
            return new Dictionary<string, NpcDef>
            {
                { "PizzaNPC", new NpcDef("Possessed Pizza", null, 80, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {   FrameCount = 1,
                    Damage = 5,
                    Width = 64,
                    Height = 64,
                    Alpha = 0,
                    Scale = 1f,
                    IgnoreTileCollision = true,
                    Colour = Color.White,
                    Value = NpcValue.Zero,
                    AiStyle = NpcAiStyle.FlyingWeapon,
                    IgnoreGravity = true
                } },
                { "PizzaBoss", new NpcDef("Pizza God", () => new PizzaGodBehaviour(), 66666, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"), () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {   FrameCount = 1,
                    Damage = 166,
                    Defense = 66,
                    Width = 48 * 4,
                    Height = 48 * 4,
                    Alpha = 0,
                    Scale = 4f,
                    IgnoreTileCollision = true,
                    KnockbackResistance = 0f,
                    Colour = Color.White,
                    Value = NpcValue.Zero,
                    AiStyle = NpcAiStyle.None,
                    IsBoss = true,
                    IsSummonableBoss = true,
                    Music = new BgmRef("QueenBee"),
                    IgnoreGravity = true, //!!! BEEEEP BOOOOP
                    SoundOnHit = VanillaSfxes.NpcHit[13],
                    SoundOnDeath = VanillaSfxes.NpcKilled[11]
                } },
                { "PizzaGodJr", new NpcDef("Pizza God Jr.", () => new PizzaGodJrBehaviour(), 266, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {   FrameCount = 1,
                    Damage = 137,
                    Defense = 66,
                    Width = 48 * 3 / 4,
                    Height = 48 * 3 / 4,
                    Alpha = 0,
                    Scale = 0.75f,
                    IgnoreTileCollision = false,
                    Colour = Color.White,
                    Value = NpcValue.Zero,
                    AiStyle = NpcAiStyle.None,
                    IgnoreGravity = false,
                    SoundOnDeath = VanillaSfxes.NpcKilled[11]
                } },
            };
        }
        protected override Dictionary<string, ProjectileDef> GetProjectileDefs()
        {
            return new Dictionary<string, ProjectileDef>
            {
                { "PizzaProjectile", new ProjectileDef("Flying Pizza!", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                }
            };
        }
        protected override Dictionary<string, TileDef> GetTileDefs()
        {
            return new Dictionary<string, TileDef>
            {
                {"TestTile", new TileDef("Test tile", null, () => GetEmbeddedResource<Texture2D>("Resources/Textures/Misc/TestTile.png"))
                {
                    SubtypeData = new TileSubtypeData()
                    {
                        IsBrick = true
                    },
                    Size = new Point(1, 1),
                    LightingData = new TileLightingData()
                    {
                        BlocksLight    = true,
                        BlocksSunlight = true
                    },
                    CollisionData = new TileCollisionData()
                    {
                        IsSolid  = true,
                        IsBouncy = true
                    }
                } }
            };
        }
        protected override Dictionary<string, WallDef> GetWallDefs()
        {
            return new Dictionary<string, WallDef>
            {
                { "TestWall", new WallDef("Test wall", () => GetEmbeddedResource<Texture2D>("Resources/Textures/Misc/TestWall.png"))
                {
                    IsSuitableForHousing = true,
                    IsTransparent = false,
                    Light = true
                } }
            };
        }

        protected override IEnumerable<RecipeDef> GetRecipeDefs()
        {
            var AnyGel = new ItemGroup(new[]
            {
                new ItemRef(ItemID.    Gel),
                new ItemRef(ItemID.PinkGel)
            }, "Any gel");

            return new[]
            {
                new RecipeDef(
                    new ItemRef("Pizza"), 8,
                    new RecipeItems
                    {
                        { AnyGel, 30 }
                    }
                ),
                new RecipeDef(
                    new ItemRef("Ant"), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza"   ),  1 },
                        {              AnyGel    , 20 }
                    }
                ),
                new RecipeDef(
                    new ItemRef("Pizzant"), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza"   ), 1 },
                        { new ItemRef("Ant"     ), 1 },
                        {              AnyGel    , 4 }
                    },
                    new[] { new TileRef(TileID.TinkerersWorkbench) }
                ),
                new RecipeDef(
                    new ItemRef("Pizzantzioli"), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza"   ), 3 },
                        { new ItemRef("Pizzant" ), 1 },
                        {              AnyGel    , 4 }
                    },
                    new[] { new TileRef(TileID.Dirt) }
                ),
                new RecipeDef(
                    new ItemRef("WallPlacer"), 1,
                    new RecipeItems
                    {
                        { AnyGel, 1 }
                    }
                ),
                new RecipeDef(
                    new ItemRef("TilePlacer"), 1,
                    new RecipeItems
                    {
                        { AnyGel, 1 }
                    }
                )
            };
        }

        protected override GameBehaviour CreateGameBehaviour()
        {
            return new Game();
        }
    }
}
