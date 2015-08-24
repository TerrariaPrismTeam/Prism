using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    static class WorldFilePatcher
    {
        static CecilContext context;
        static MemberResolver memRes;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_WorldFile;

        static void EnlargeSectionArray()
        {
            const sbyte NewSize = 15;

            var saveWorldV2    = typeDef_WorldFile.GetMethod("SaveWorld_Version2"  );
            var saveFileHeader = typeDef_WorldFile.GetMethod("SaveFileFormatHeader");

            var swb = saveWorldV2   .Body;
            var shb = saveFileHeader.Body;

            swb.Instructions[0].Operand = NewSize; // first instruction is ldc.i4(.s) 10
            shb.Instructions[2].Operand = NewSize;
        }
        static void InjectSaveHook()
        {
            var saveWorldV2 = typeDef_WorldFile.GetMethod("SaveWorld_Version2");

            var swb = saveWorldV2.Body;
            var swbproc = swb.GetILProcessor();

            MethodDefinition invokeSaveWorld;
            var saveWorldDel = context.CreateDelegate("Terraria.PrismInjections", "WorldFile_OnSaveWorldDel", typeSys.Void, out invokeSaveWorld, saveWorldV2.Parameters[0].ParameterType, swb.Variables[0].VariableType);

            var onSaveWorld = new FieldDefinition("P_OnSaveWorld", FieldAttributes.Public | FieldAttributes.Static, saveWorldDel);
            typeDef_WorldFile.Fields.Add(onSaveWorld);

            OpCode[] toFind =
            {
                OpCodes.Ldarg_0,
                OpCodes.Call,
                OpCodes.Pop,
                OpCodes.Ldarg_0,
                OpCodes.Ldloc_0,
                OpCodes.Call,
                OpCodes.Pop,
                OpCodes.Ret
            };

            Instruction[] toInject =
            {
                Instruction.Create(OpCodes.Ldsfld, onSaveWorld),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldloc_0),
                Instruction.Create(OpCodes.Callvirt, invokeSaveWorld)
            };

            var instr = swb.FindInstrSeqStart(toFind);

            for (int i = 0; i < toInject.Length; i++)
                swbproc.InsertBefore(instr, toInject[i]);
        }
        static void InjectLoadHook()
        {
            // only hooking to V2 shouldn't be bad: V1 worlds won't have mod data in them, because 1.3 (and prism) only write as V2
            var loadWorldV2 = typeDef_WorldFile.GetMethod("LoadWorld_Version2");

            var lwb = loadWorldV2.Body;
            var lwbproc = lwb.GetILProcessor();

            MethodDefinition invokeLoadWorld;
            var loadWorldDel = context.CreateDelegate("Terraria.PrismInjections", "WorldFile_OnLoadWorldDel", typeSys.Void, out invokeLoadWorld, loadWorldV2.Parameters[0].ParameterType);

            var onLoadWorld = new FieldDefinition("P_OnLoadWorld", FieldAttributes.Public | FieldAttributes.Static, loadWorldDel);
            typeDef_WorldFile.Fields.Add(onLoadWorld);

            OpCode[] toFind =
            {
                OpCodes.Ldarg_0,
                OpCodes.Call,
                OpCodes.Stloc_2, // wtf?
                OpCodes.Ldloc_2,
                OpCodes.Ret
            };

            Instruction[] toInject =
            {
                Instruction.Create(OpCodes.Ldsfld, onLoadWorld),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, invokeLoadWorld)
            };

            var instr = lwb.FindInstrSeqStart(toFind);

            for (int i = 0; i < toInject.Length; i++)
                lwbproc.InsertBefore(instr, toInject[i]);

            // rewire the if-block to go to the injected code instead of the LoadFooter call (after the last if (qsdf) { return 5; })
            for (int i = 0; i < lwb.Instructions.Count; i++)
                if (lwb.Instructions[i].Operand == instr)
                    lwb.Instructions[i].Operand = toInject[0];
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_WorldFile = memRes.GetType("Terraria.IO.WorldFile");

            EnlargeSectionArray();
            InjectSaveHook();
            InjectLoadHook();
        }
    }
}
