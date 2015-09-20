using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Prism.Injector.Patcher
{
    static class TEPatcher
    {
        static CecilContext   context;
        static MemberResolver memRes ;

        static TypeSystem     typeSys;
        static TypeDefinition typeDef_TE;

        static void WrapMethods()
        {
            var teDummy_t = memRes.GetType("Terraria.GameContent.Tile_Entities.TETrainingDummy");

            teDummy_t.GetMethod("ReadExtraData").Wrap(context);
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_TE = memRes.GetType("Terraria.DataStructures.TileEntity");

            WrapMethods();
        }
    }
}
