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

        // This is a massive hack for a bug in Player.cs. It removes the ID checks from player loading, so that invalid items are removed instead of resulting in the character being declared invalid.
        // If this gets fixed in the original, this code should be removed.
        static void RemoveBuggyPlayerLoading()
        {
            var loadPlayerBody = player_t.GetMethod("LoadPlayer", MethodFlags.Public | MethodFlags.Static, ts.String, ts.Boolean).Body;
            var processor = loadPlayerBody.GetILProcessor();
            int count = 0;

            //TODO: clean this up
            while (true)
            {
                bool found = false;
                foreach (var i in loadPlayerBody.Instructions)
                {
                    if (i.OpCode != OpCodes.Ldloc_S)
                        continue;
                    var t = i.Next;
                    if (t.OpCode != OpCodes.Callvirt)
                        continue;
                    t = t.Next;
                    var start = t;
                    if (t.OpCode != OpCodes.Stloc_S)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Ldloc_S)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Ldc_I4)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Blt_S)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Ldloc_1)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Ldfld)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Ldloc_S)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Ldelem_Ref)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Ldc_I4_0)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Callvirt)
                        continue;
                    t = t.Next;
                    if (t.OpCode != OpCodes.Br_S)
                        continue;
                    t = t.Next;
                    var end = t;
                    if (t.OpCode != OpCodes.Ldloc_1)
                        continue;
                    count++;
                    while (start.Next != end)
                    {
                        processor.Remove(start.Next);
                    }
                    found = true;
                    break;  // Since iterating through is no longer valid, we break and let the loop start again.
                }
                if (!found)
                {
                    if (count != 6)
                    {
                        Console.WriteLine("Count is " + count.ToString() + " instead of 6; Terraria.Player.LoadPlayer() logic may have changed.");
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
