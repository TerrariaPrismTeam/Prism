using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Mods.DefHandlers
{
    static class Handler
    {
        internal static ItemDefHandler   ItemDef   = new ItemDefHandler();
        internal static NpcDefHandler    NpcDef    = new NpcDefHandler ();
        internal static ProjDefHandler   ProjDef   = new ProjDefHandler();
        internal static TileDefHandler   TileDef   = new TileDefHandler();
        internal static RecipeDefHandler RecipeDef = new RecipeDefHandler();
    }
}
