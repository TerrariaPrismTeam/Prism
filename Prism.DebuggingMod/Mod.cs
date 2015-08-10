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

namespace Prism.DebuggingMod
{
    public class Mod : ModDef
    {
        protected override ContentHandler CreateContentHandler()
        {
            return new Content();
        }
    }
}
