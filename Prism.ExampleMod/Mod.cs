using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API;
using Terraria.ID;

namespace Prism.ExampleMod
{
    public class Mod : ModDef
    {
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
    }
}
