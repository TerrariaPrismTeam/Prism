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
        static CecilContext   c;
        static MemberResolver r;

        static TypeSystem ts;
        static TypeDefinition main_t;
		
        static void RemoveNetModeCheckFromChat()
        {
            OpCode[] searchSeq = new OpCode[]
                {
                                    //IL_2a83: ldsflda valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                                    //IL_2a88: ldc.i4.s 13
                                    //IL_2a8a: call instance bool [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState::IsKeyDown(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.Keys)
                                    //IL_2a8f: brfalse IL_2b20
                /*
                    Remove these 3:
                */                                      
                OpCodes.Ldsfld,     //IL_2a94: ldsfld int32 Terraria.Main::netMode
                OpCodes.Ldc_I4_1,   //IL_2a99: ldc.i4.1
                OpCodes.Bne_Un,     //IL_2a9a: bne.un IL_2b20

                OpCodes.Ldsflda,    //IL_2a9f: Main.ldsflda valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                OpCodes.Ldc_I4,     //IL_2aa4: ldc.i4 164
                OpCodes.Call,       //IL_2aa9: call instance bool [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState::IsKeyDown(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.Keys)
                OpCodes.Brtrue_S,   //IL_2aae: brtrue.s IL_2b20
                                      
                OpCodes.Ldsflda,    //IL_2ab0: ldsflda valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState Terraria.Main::keyState
                OpCodes.Ldc_I4,     //IL_2ab5: ldc.i4 165
                OpCodes.Call,       //IL_2aba: call instance bool [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.KeyboardState::IsKeyDown(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Input.Keys)
                OpCodes.Brtrue_S,   //IL_2abf: brtrue.s IL_2b20
                
                OpCodes.Ldsfld,     //IL_2ac1: ldsfld bool Terraria.Main::hasFocus
                OpCodes.Brfalse_S   //IL_2ac6: brfalse.s IL_2b20
            };

            var mainUpdate = main_t.GetMethod("Update").Body; //Neither the access modifiers or the arg types are specified but its Terraria ffs what other "Update" method could we possibly be looking for in Main?

            var proc = mainUpdate.GetILProcessor();

            var firstLoc = CecilHelper.FindInstructionSeq(mainUpdate, searchSeq);

            if (firstLoc != null)
            {
                CecilHelper.RemoveInstructions(proc, firstLoc, 3);
            }
        }

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

            var drawNpcs = main_t.GetMethod("DrawNPCs", MethodFlags.Instance | MethodFlags.Public, ts.Boolean);

            var firstInstr = CecilHelper.FindInstructionSeq(drawNpcs.Body, seqToRemove);
            CecilHelper.RemoveInstructions(drawNpcs.Body.GetILProcessor(), firstInstr, seqToRemove.Length);
        }

        internal static void Patch()
        {
            c = TerrariaPatcher.c;
            r = TerrariaPatcher.r;

            ts = c.PrimaryAssembly.MainModule.TypeSystem;
            main_t = r.GetType("Terraria.Main");

            RemoveNetModeCheckFromChat();
            RemoveVanillaNpcDrawLimitation();
        }
    }
}
