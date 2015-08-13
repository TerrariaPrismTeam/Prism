using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Prism.API.Behaviours;
using Terraria;

namespace Prism.ExampleMod.Behaviours.NPC
{
    public class PizzaGodJrBehaviour : NpcBehaviour
    {
        public override void OnAI()
        {
            Entity.TargetClosest();

            Lighting.AddLight(Entity.Center, new Vector3(20, 20, 0) * 0.5f * (Entity.scale / 8));
        }
    }
}
