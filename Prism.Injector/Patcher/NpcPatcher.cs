using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    static class NpcPatcher
    {
        static CecilContext   context;
        static MemberResolver  memRes;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_NPC;

        static void WrapSetDefaults()
        {
            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = context.CreateDelegate("Terraria.PrismInjections", "NPC_OnSetDefaultsDelegate", typeSys.Void, out invokeOnSetDefaults, typeDef_NPC, typeSys.Int32, typeSys.Single);

            var setDefaults = typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32, typeSys.Single);

            var newSetDefaults = WrapperHelper.ReplaceAndHook(setDefaults, invokeOnSetDefaults);

            WrapperHelper.ReplaceAllMethodRefs(context, setDefaults, newSetDefaults);
        }
        static void AddFieldForBHandler()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
        }
        static void AddFieldForMusic()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_Music", FieldAttributes.Public, typeSys.Object));
        }
        static void InsertInitialize()
        {
            MethodDefinition invokeOnNewNPC;
            var onNewNPCDel = context.CreateDelegate("Terraria.PrismInjections", "NPC_OnNewNPCDelegate", typeSys.Int32, out invokeOnNewNPC,
                typeSys.Int32, typeSys.Int32, typeSys.Int32, typeSys.Int32,
                typeSys.Single, typeSys.Single, typeSys.Single, typeSys.Single,
                typeSys.Int32);

            var newNPC = typeDef_NPC.GetMethod("NewNPC", MethodFlags.Public | MethodFlags.Static,
                typeSys.Int32, typeSys.Int32, typeSys.Int32, typeSys.Int32,
                typeSys.Single, typeSys.Single, typeSys.Single, typeSys.Single,
                typeSys.Int32);

            var newNewNPC = WrapperHelper.ReplaceAndHook(newNPC, invokeOnNewNPC);

            WrapperHelper.ReplaceAllMethodRefs(context, newNPC, newNewNPC);

            //? I tried the code below, but it somehow borked the IL code (InvalidProgramException, ILSpy wouldn't disassemble (even in IL mode)),
            //? so I did this.

            /*MethodDefinition invokeOnNewNPC;
            var onNewNPCDel = CecilHelper.CreateDelegate(context, "Terraria.PrismInjections", "NPC_OnNewNPCDelegate", typeSys.Void, out invokeOnNewNPC, typeSys.Int32);

            var onNewNPC_f = new FieldDefinition("P_OnNewNPC", FieldAttributes.Public | FieldAttributes.Static, onNewNPCDel);

            typeDef_NPC.Fields.Add(onNewNPC_f);

            var newNPC = typeDef_NPC.GetMethod("NewNPC", MethodFlags.Public | MethodFlags.Static,
                typeSys.Int32 , typeSys.Int32 , typeSys.Int32 , typeSys.Int32 ,
                typeSys.Single, typeSys.Single, typeSys.Single, typeSys.Single,
                typeSys.Int32);

            var nnilp = newNPC.Body.GetILProcessor();

            OpCode[] toFind =
            {
                OpCodes.Ldloc_0,
                OpCodes.Ret,
                //OpCodes.Ldc_I4,
                //OpCodes.Ret
            };
            Instruction[] insertBefore =
            {
                nnilp.Create(OpCodes.Ldsfld, onNewNPC_f),
                nnilp.Create(OpCodes.Ldloc_0),
                nnilp.Create(OpCodes.Callvirt, invokeOnNewNPC),
            };

            var ldloc0 = CecilHelper.FindInstructionSeq(newNPC.Body, toFind);
            var prev = ldloc0.Previous;

            for (int i = insertBefore.Length - 1; i >= 0; i--)
                nnilp.InsertAfter(prev, insertBefore[i]);*/
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_NPC  = memRes.GetType("Terraria.NPC" );

            WrapSetDefaults();
            AddFieldForBHandler();
            AddFieldForMusic();
            InsertInitialize();
        }
    }
}
