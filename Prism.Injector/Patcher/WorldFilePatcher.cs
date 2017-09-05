using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class WorldFilePatcher
    {
        static DNContext context;
        static MemberResolver memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_WorldFile;

        static void InjectSaveHook()
        {
            var saveWorld = typeDef_WorldFile.GetMethod("saveWorld", MethodFlags.Public | MethodFlags.Static, typeSys.Boolean.ToTypeDefOrRef(), typeSys.Boolean.ToTypeDefOrRef());

            MethodDef invokeSaveWorld;
            var saveWorldDel = context.CreateDelegate("Terraria.PrismInjections", "WorldFile_OnSaveWorldDel", typeSys.Void, out invokeSaveWorld, typeSys.Boolean);

            var onSaveWorld = new FieldDefUser("P_OnSaveWorld", new FieldSig(saveWorldDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
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

            /*var swb = saveWorld.Body;
            using (var swbproc = swb.GetILProcessor())
            {
                var instr = swb.FindInstrSeqStart(toFind).Next(swbproc).Next(swbproc);

                for (int i = 0; i < toInject.Length; i++)
                    swbproc.InsertBefore(instr, toInject[i]);
            }*/
        }
        static void InjectLoadHook()
        {
            // only hooking to V2 shouldn't be bad: V1 worlds won't have mod data in them, because 1.3 (and prism) only write as V2
            var loadWorld = typeDef_WorldFile.GetMethod("loadWorld");

            MethodDef invokeLoadWorld;
            var loadWorldDel = context.CreateDelegate("Terraria.PrismInjections", "WorldFile_OnLoadWorldDel", typeSys.Void, out invokeLoadWorld, typeSys.Boolean);

            var onLoadWorld = new FieldDefUser("P_OnLoadWorld", new FieldSig(loadWorldDel.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
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

            /*var lwb = loadWorld.Body;
            using (var lwbproc = lwb.GetILProcessor())
            {
                var instr = lwb.FindInstrSeqStart(toFind).Next(lwbproc).Next(lwbproc).Next(lwbproc).Next(lwbproc);

                for (int i = 0; i < toInject.Length; i++)
                    lwbproc.InsertBefore(instr, toInject[i]);
            }*/
        }
        static void EnlargeFrameImportantArray()
        {
            var saveHeader = typeDef_WorldFile.GetMethod("SaveFileFormatHeader");

            if ((int)saveHeader.Body.Instructions[0].Operand != 461)
                Console.WriteLine("WARNING! max tile type is not 461, SaveFileFormatHeader might've changed!");

            using (var shp = saveHeader.Body.GetILProcessor())
            {
                var first = saveHeader.Body.Instructions[0];

                // replace constant (Main.maxTileSets) with the actual length of the array
                shp.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, memRes.GetType("Terraria.Main").GetField("tileFrameImportant")));
                shp.InsertAfter(saveHeader.Body.Instructions[0] /* do NOT use 'first' here */, Instruction.Create(OpCodes.Ldlen));

                shp.Remove(first);
                //saveHeader.Body.Instructions[0].Operand = 0x7FFF; // should be enough & cannot be more: is read as a short in LoadFileFormatHeader
            }
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_WorldFile = memRes.GetType("Terraria.IO.WorldFile");

            InjectSaveHook();
            InjectLoadHook();
            EnlargeFrameImportantArray();
        }
    }
}
