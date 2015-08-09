using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Microsoft.Xna.Framework;

namespace Prism.Injector.Patcher
{
    static class MainPatcher
    {
        static CecilContext   context;
        static MemberResolver  memRes;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_Main;

        static void RemoveVanillaNpcDrawLimitation()
        {
            OpCode[] seqToRemove =
            {
                // original code:

                // ldsfld class Terraria.NPC[] Terraria.Main::npc
                // ldloc.2
                // ldelem.ref
                // ldfld int32 Terraria.NPC::'type'
                // ldc.i4 540
                // bge <end-of-if-body>

                // in 1.3.0.7, this starts at address 0x006f

                OpCodes.Ldsfld,
                OpCodes.Ldloc_2,
                OpCodes.Ldelem_Ref,
                OpCodes.Ldfld,
                OpCodes.Ldc_I4,
                OpCodes.Bge
            };

            var drawNpcs = typeDef_Main.GetMethod("DrawNPCs", MethodFlags.Instance | MethodFlags.Public, typeSys.Boolean);

            var firstInstr = drawNpcs.Body.FindInstrSeqStart(seqToRemove);
            drawNpcs.Body.GetILProcessor().RemoveInstructions(firstInstr, seqToRemove.Length);
        }
        static void WrapUpdateMusic()
        {
            WrapperHelper.WrapInstanceMethod(context, typeDef_Main, typeSys.Void, "UpdateMusic");
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_Main = memRes.GetType("Terraria.Main");

            RemoveVanillaNpcDrawLimitation();
            WrapUpdateMusic();
        }
    }
}
