using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria.ID;

namespace Prism.DebuggingMod
{
    sealed class Content : ContentHandler
    {        
        protected override GameBehaviour CreateGameBehaviour()
        {
            return new Game();
        }
    }
}
