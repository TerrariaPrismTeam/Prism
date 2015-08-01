using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    static class PlayerPatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        static TypeSystem ts;
        static TypeDefinition player_t;

        // Removes the ID checks from player loading, so that invalid items
        // are removed instead of resulting in the character being declared
        // invalid. If this gets fixed in the original, this code should be
        // removed.
        static void RemoveBuggyPlayerLoading()
        {
            OpCode[] seqToRemove =
            {
                    OpCodes.Ldloc_S,
                    OpCodes.Ldc_I4,
                    OpCodes.Blt_S,
                    OpCodes.Ldloc_1,
                    OpCodes.Ldfld,
                    OpCodes.Ldloc_S,
                    OpCodes.Ldelem_Ref,
                    OpCodes.Ldc_I4_0,
                    OpCodes.Callvirt,
                    OpCodes.Br_S,
            };

            var loadPlayerBody = player_t.GetMethod("LoadPlayer", MethodFlags.Public | MethodFlags.Static, ts.String, ts.Boolean).Body;
            var processor = loadPlayerBody.GetILProcessor();
            int count = 0;

            while (true)
            {
                var firstInstr = CecilHelper.FindInstructionSeq(loadPlayerBody, seqToRemove);
                if (firstInstr != null)
                {
                    count++;
                    CecilHelper.RemoveInstructions(processor, firstInstr, seqToRemove.Length);
                }
                else
                {
                    if (count == 0)
                    {
                        Console.WriteLine("PlayerPatcher.RemoveBuggyPlayerLoading() could not find the target instruction sequence; Terraria.Player.LoadPlayer() may have been fixed, and this hack can be removed.");
                    }
                    else if (count != 6)
                    {
                        Console.WriteLine("PlayerPatcher.RemoveBuggyPlayerLoading() removed " + count.ToString() + " instances of the target instruction sequence instead of 6; Terraria.Player.LoadPlayer() logic may have changed, and this hack may be superflous/harmful!");
                    }
                    break;
                }
            }
        }

        internal static void Patch()
        {
            c = TerrariaPatcher.c;
            r = TerrariaPatcher.r;

            ts = c.PrimaryAssembly.MainModule.TypeSystem;
            player_t = r.GetType("Terraria.Player");

            RemoveBuggyPlayerLoading();
        }
    }
}
