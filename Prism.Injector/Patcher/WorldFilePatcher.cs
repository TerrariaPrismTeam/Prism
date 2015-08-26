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

        static void InjectSaveHook()
        {
            var saveWorld = typeDef_WorldFile.GetMethod("saveWorld", MethodFlags.Public | MethodFlags.Static, typeSys.Boolean, typeSys.Boolean);

            var swb = saveWorld.Body;
            var swbproc = swb.GetILProcessor();

            MethodDefinition invokeSaveWorld;
            var saveWorldDel = context.CreateDelegate("Terraria.PrismInjections", "WorldFile_OnSaveWorldDel", typeSys.Void, out invokeSaveWorld, typeSys.Boolean);

            var onSaveWorld = new FieldDefinition("P_OnSaveWorld", FieldAttributes.Public | FieldAttributes.Static, saveWorldDel);
            typeDef_WorldFile.Fields.Add(onSaveWorld);

            OpCode[] toFind =
            {
                OpCodes.Ldloc_S,
                OpCodes.Call,
                OpCodes.Leave_S
            };

            Instruction[] toInject =
            {
                Instruction.Create(OpCodes.Ldsfld, onSaveWorld),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, invokeSaveWorld)
            };

            var instr = swb.FindInstrSeqStart(toFind).Next.Next;

            for (int i = 0; i < toInject.Length; i++)
                swbproc.InsertBefore(instr, toInject[i]);
        }
        static void InjectLoadHook()
        {
            // only hooking to V2 shouldn't be bad: V1 worlds won't have mod data in them, because 1.3 (and prism) only write as V2
            var loadWorld = typeDef_WorldFile.GetMethod("loadWorld");

            var lwb = loadWorld.Body;
            var lwbproc = lwb.GetILProcessor();

            MethodDefinition invokeLoadWorld;
            var loadWorldDel = context.CreateDelegate("Terraria.PrismInjections", "WorldFile_OnLoadWorldDel", typeSys.Void, out invokeLoadWorld, typeSys.Boolean);

            var onLoadWorld = new FieldDefinition("P_OnLoadWorld", FieldAttributes.Public | FieldAttributes.Static, loadWorldDel);
            typeDef_WorldFile.Fields.Add(onLoadWorld);

            OpCode[] toFind =
            {
                OpCodes.Br_S,
                OpCodes.Ldloc_S,
                OpCodes.Call, // wtf?
                OpCodes.Stloc_S
            };

            Instruction[] toInject =
            {
                Instruction.Create(OpCodes.Ldsfld, onLoadWorld),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, invokeLoadWorld)
            };

            var instr = lwb.FindInstrSeqStart(toFind).Next.Next.Next.Next;

            for (int i = 0; i < toInject.Length; i++)
                lwbproc.InsertBefore(instr, toInject[i]);
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_WorldFile = memRes.GetType("Terraria.IO.WorldFile");

            InjectSaveHook();
            InjectLoadHook();
        }
    }
}
