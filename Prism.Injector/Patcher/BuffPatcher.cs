using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Prism.Injector.Patcher
{
    static class BuffPatcher
    {
        static DNContext   context;
        static MemberResolver memRes ;

        static ICorLibTypes typeSys;

        static void AddZephyrFishBuffID()
        {
            var buffId_t = memRes.GetType("Terraria.ID.BuffID");

            if (buffId_t.GetField("ZephyrFish") != null)
                return;

            buffId_t.Fields.Add(new FieldDefUser("ZephyrFish", new FieldSig(typeSys.Int32), FieldAttributes.Literal | FieldAttributes.Public | FieldAttributes.Static)
            {
                Constant = new ConstantUser(127, ElementType.I4)
            });
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;

            AddZephyrFishBuffID();
        }
    }
}
