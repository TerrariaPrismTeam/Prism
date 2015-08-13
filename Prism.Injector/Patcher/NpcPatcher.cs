using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

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
            typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32 , typeSys.Single).Wrap(context, "Terraria.PrismInjections", "NPC_SetDefaultsDel_Id"  , "P_OnSetDefaultsById"  );
            typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.String                ).Wrap(context, "Terraria.PrismInjections", "NPC_SetDefaultsDel_Name", "P_OnSetDefaultsByName");
        }
        static void AddFieldForBHandler()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
        }
        static void AddFieldForMusic()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_Music", FieldAttributes.Public, typeSys.Object));
        }
        static void WrapAI()
        {
            typeDef_NPC.GetMethod("AI", MethodFlags.Public | MethodFlags.Instance).Wrap(context);
        }
        static void InsertInitialize()
        {
            typeDef_NPC.GetMethod("NewNPC", MethodFlags.Public | MethodFlags.Static,
                typeSys.Int32, typeSys.Int32, typeSys.Int32, typeSys.Int32,
                typeSys.Single, typeSys.Single, typeSys.Single, typeSys.Single,
                typeSys.Int32).Wrap(context);
            /*
            MethodDefinition invokeOnNewNPC;
            var onNewNPCDel = context.CreateDelegate("Terraria.PrismInjections", "NPC_OnNewNPCDelegate", typeSys.Int32, out invokeOnNewNPC,
                typeSys.Int32, typeSys.Int32, typeSys.Int32, typeSys.Int32,
                typeSys.Single, typeSys.Single, typeSys.Single, typeSys.Single,
                typeSys.Int32);

            var newNPC = typeDef_NPC.GetMethod("NewNPC", MethodFlags.Public | MethodFlags.Static,
                typeSys.Int32, typeSys.Int32, typeSys.Int32, typeSys.Int32,
                typeSys.Single, typeSys.Single, typeSys.Single, typeSys.Single,
                typeSys.Int32);

            var newNewNPC = newNPC.ReplaceAndHook(invokeOnNewNPC);

            WrapperHelperExtensions.ReplaceAllMethodRefs(context, newNPC, newNewNPC);

            */


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
            WrapAI();
            InsertInitialize();
        }
    }
}
