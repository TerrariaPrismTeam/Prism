using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.API;
using Terraria;
using Terraria.ID;

namespace Prism.ExampleMod
{
    public class Mod : ModDef
    {
        bool hasPizza = false;

        protected override Dictionary<string, ItemDef> GetItemDefs()
        {
            return new Dictionary<string, ItemDef>
            {
                { "Pizza", new ItemDef("Pizza", getTex: () => GetResource<Texture2D>("Pizza.png"),
                    useTime: 15,
                    useAnimation: 15,
                    consumable: true,
                    maxStack: 999,
                    rare: ItemRarity.Lime,
                    useSound: 2,
                    useStyle: ItemUseStyle.Eat,
                    holdStyle: ItemHoldStyle.BreathingReed,
                    healLife: 400,
                    value: new ItemValue(50, 10, 2),
                    buff: new ItemBuff(BuffID.WellFed, 60 * 60 * 30)) }
            };
        }

        public override void PostUpdate()
        {
            if (Main.keyState.IsKeyDown(Keys.Y) && !hasPizza && !Main.gameMenu && Main.hasFocus)
            {
                var inv = Main.player[Main.myPlayer].inventory;

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
                    }
                }
            }
        }
    }
}
