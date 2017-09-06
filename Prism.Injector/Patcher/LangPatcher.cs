using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class LangPatcher
    {
        static DNContext context;
        static MemberResolver memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Lang;

        static void UseArrayLenghtsInsteadOfConstants(Action<string> log)
        {
            // GetNPCName
            {
                var gnn = typeDef_Lang.GetMethod("GetNPCName");

                OpCode[] toFind = { OpCodes.Ldc_I4 };

                var firstInstr = gnn.Body.FindInstrSeqStart(toFind);

                if ((int)firstInstr.Operand != 580)
                    log("Warning: ldc.i4 in GetNPCName isn't 580 but " + firstInstr.Operand + ", recheck the MSIL.");

                using (var p = gnn.Body.GetILProcessor())
                {
                    p.InsertBefore(firstInstr, p.Create(OpCodes.Ldsfld, typeDef_Lang.GetField("_npcNameCache")));
                    p.InsertBefore(firstInstr, p.Create(OpCodes.Ldlen));

                    p.Remove(firstInstr);
                }
            }

            // GetItemName
            {
                var gin = typeDef_Lang.GetMethod("GetItemName");

                OpCode[] toFind = { OpCodes.Ldc_I4 };

                var firstInstr = gin.Body.FindInstrSeqStart(toFind);

                if ((int)firstInstr.Operand != 3930)
                    log("Warning: ldc.i4 in GetItemname isn't 3930 but " + firstInstr.Operand + ", recheck the MSIL.");

                using (var p = gin.Body.GetILProcessor())
                {
                    p.InsertBefore(firstInstr, p.Create(OpCodes.Ldsfld, typeDef_Lang.GetField("_itemNameCache")));
                    p.InsertBefore(firstInstr, p.Create(OpCodes.Ldlen));

                    p.Remove(firstInstr);
                }
            }
        }

        internal static void Patch(Action<string> log)
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Lang = memRes.GetType("Terraria.Lang");

            UseArrayLenghtsInsteadOfConstants(log);
        }
    }
}

