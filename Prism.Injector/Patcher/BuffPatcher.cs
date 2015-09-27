using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Prism.Injector.Patcher
{
    static class BuffPatcher
    {
        static CecilContext   context;
        static MemberResolver memRes ;

        static TypeSystem typeSys;

        static void AddZephyrFishBuffID()
        {
            var buffId_t = memRes.GetType("Terraria.ID.BuffID");

            if (buffId_t.GetField("ZephyrFish") != null)
                return;

            buffId_t.Fields.Add(new FieldDefinition("ZephyrFish", FieldAttributes.Literal | FieldAttributes.Public | FieldAttributes.Static, typeSys.Int32)
            {
                Constant = 127
            });
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;

            AddZephyrFishBuffID();
        }
    }
}
