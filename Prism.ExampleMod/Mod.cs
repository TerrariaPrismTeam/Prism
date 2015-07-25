using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.API;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;
using Prism.Mods;

namespace Prism.ExampleMod
{
    public class Mod : ModDef
    {
        bool
            hasPizza        = false,
            hasAnt          = false,
            hasPizzant      = false,
            hasPizzantzioli = false;

        bool spawnedPizzantzioli = false;

        protected override Dictionary<string, ItemDef> GetItemDefs()
        {
            return new Dictionary<string, ItemDef>
            {
                { "Pizza", new ItemDef("Pizza", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizza.png"),
                    descr: new ItemDescription("LOTZA SPA-pizza. It's pizza.", "'MMmmmmmmm'", false, true),
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
                { "Ant", new ItemDef("Ant", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Ant.png"),
                    descr: new ItemDescription("By ants, for ants.", "'B-but...ants aren't this big!'", false, true),
                    damageType: ItemDamageType.Melee,
                    autoReuse: true,
                    useTime: 12,
                    reuseDelay: 0,
                    useAnimation: 20,
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
                { "Pizzant", new ItemDef("Pizzant", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizzant.png"),
                    descr: new ItemDescription("The chaotic forces of italian spices and insects and bread.", "", false, true),
                    damageType: ItemDamageType.Melee,
                    autoReuse: true,
                    useTime: 15,
                    reuseDelay: 0,
                    useAnimation: 20,
                    maxStack: 1,
                    rare: ItemRarity.Yellow,
                    useSound: 1,
                    damage: 80,
                    knockback: 4f,
                    width: 30,
                    height: 30,
                    useTurn: true,
                    useStyle: ItemUseStyle.Stab,
                    holdStyle: ItemHoldStyle.Default,
                    value: new CoinValue(1, 34, 1, 67),
                    scale: 1.1f
                    ) },
                { "Pizzantzioli", new ItemDef("Pizzantzioli", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizzantzioli.png"),
                    descr: new ItemDescription("The forces of ants and pizza come together as one.", "The name is Italian for 'KICKING ASS'! YEAH!", false, true),
                    damageType: ItemDamageType.Melee,
                    autoReuse: true,
                    useTime: 20,
                    reuseDelay: 0,
                    useAnimation: 16,
                    maxStack: 1,
                    rare: ItemRarity.Cyan,
                    useSound: 1,
                    damage: 150,
                    knockback: 10f,
                    width: 30,
                    height: 30,
                    useTurn: true,
                    useStyle: ItemUseStyle.Swing,
                    holdStyle: ItemHoldStyle.Default,
                    value: new CoinValue(2, 51, 3, 9),
                    scale: 1.1f
                    ) }
            };
        }
        protected override Dictionary<string, NpcDef > GetNpcDefs ()
        {
            return new Dictionary<string, NpcDef>
            {

                { "Pizzantzioli", new NpcDef("Pizza NPC", getTex: () => GetResource<Texture2D>("Resources\\Textures\\Items\\Pizzantzioli.png"),
                    damage: 50,
                    width: 128,
                    height: 128,
                    alpha: 0,
                    scale: 1.0f,
                    noTileCollide: true,
                    color: Color.White,
                    value: new NpcValue(new CoinValue(0, 0, 0, 1)),
                    aiStyle: NpcAiStyle.FlyingHead
                    ) }
            };
        }

        public override void OnLoad()
        {
            RecipeHelper.CreateRecipe(ItemID.Wood, ItemDef.ByName["Pizza", Info.InternalName], 1, 1);
        }

        public override void PostUpdate()
        {
            if (Main.gameMenu || !Main.hasFocus)
                return;

            var p = Main.player[Main.myPlayer];

            #region invedit custom items
            if (Main.keyState.IsKeyDown(Keys.Y) && !(hasPizza && hasAnt && hasPizzant && hasPizzantzioli))
            {
                var inv = p.inventory;

                for (int i = 0; i < inv.Length; i++)
                {
                    if (inv[i].type == 0)
                    {
                        if (!hasPizza)
                        {
                            inv[i].SetDefaults(ItemDef.ByName["Pizza", Info.InternalName].Type);
                            hasPizza = true;
                            inv[i].stack = inv[i].maxStack;
                            continue;
                        }
                        else if (!hasAnt)
                        {
                            inv[i].SetDefaults(ItemDef.ByName["Ant", Info.InternalName].Type);
                            hasAnt = true;
                            inv[i].stack = inv[i].maxStack;
                            continue;
                        }
                        else if (!hasPizzant)
                        {
                            inv[i].SetDefaults(ItemDef.ByName["Pizzant", Info.InternalName].Type);
                            hasPizzant = true;
                            inv[i].stack = inv[i].maxStack;
                            continue;
                        }
                        else if (!hasPizzantzioli)
                        {
                            inv[i].SetDefaults(ItemDef.ByName["Pizzantzioli", Info.InternalName].Type);
                            hasPizzantzioli = true;
                            inv[i].stack = inv[i].maxStack;
                            continue;
                        }
                    }
                }
            }
            #endregion

            #region spawn custom npcs
            if (Main.keyState.IsKeyDown(Keys.U) && !spawnedPizzantzioli)
            {
                NPC.NewNPC((int)p.Center.X, (int)p.Center.Y - 75, NpcDef.ByName["Pizzantzioli", Info.InternalName].Type);

                spawnedPizzantzioli = true;
            }
            #endregion
        }
    }
}
