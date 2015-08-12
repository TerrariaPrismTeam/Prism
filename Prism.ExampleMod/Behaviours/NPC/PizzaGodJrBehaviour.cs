using Prism.API.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.ExampleMod.Behaviours.NPC
{
    public class PizzaGodJrBehaviour : NpcBehaviour
    {

        public override void OnAI()
        {
            Entity.TargetClosest();            
        }

    }
}
