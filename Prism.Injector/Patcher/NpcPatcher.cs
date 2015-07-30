using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    static class NpcPatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        static TypeSystem ts;
        static TypeDefinition npc_t, main_t;

        static void WrapSetDefaults()
        {
            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = CecilHelper.CreateDelegate(c, "Terraria.PrismInjections", "NPC_OnSetDefaultsDelegate", ts.Void, out invokeOnSetDefaults, npc_t, ts.Int32, ts.Single);

            var setDefaults = npc_t.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, ts.Int32, ts.Single);

            var newSetDefaults = WrapperHelper.ReplaceAndHook(setDefaults, invokeOnSetDefaults);

            WrapperHelper.ReplaceAllMethodRefs(c, setDefaults, newSetDefaults);
        }
        static void AddFieldForBHandler()
        {
            npc_t.Fields.Add(new FieldDefinition("BHandler", FieldAttributes.Public, ts.Object));
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
            npc_t  = r.GetType("Terraria.NPC" );
            main_t = r.GetType("Terraria.Main");

            WrapSetDefaults();
            AddFieldForBHandler();
            RemoveVanillaNpcDrawLimitation();
        }
    }
}
