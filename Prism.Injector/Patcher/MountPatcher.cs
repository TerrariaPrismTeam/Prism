using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    static class MountPatcher
    {
        static CecilContext context;
        static MemberResolver memRes;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_Mount;

        static void WrapMethods()
        {

        }
        static void AddFieldForBHandler()
        {
            //typeDef_Mount.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
        }
        static void Remove_FromFields()
        {
            foreach (var fd in typeDef_Mount.Fields)
                if (fd.Name[0] == '_')
                    fd.Name = fd.Name.Substring(1);
        }
        static void RemoveTypeLimitations()
        {
            #region GetHeightBoost
            {
                var getHeightBoost = typeDef_Mount.GetMethod("GetHeightBoost");

                var ghbb = getHeightBoost.Body;
                var ghbproc = ghbb.GetILProcessor();

                // the method immediately returns when this.type <= -1 or > 14
                // just remove the check + return
                var instrs = getHeightBoost.Body.Instructions.TakeWhile(i => i.OpCode.Code != Code.Ret);
                instrs = instrs.Concat(new[] { instrs.Last().Next }).ToArray(); // not using ToArray will throw an exception because the collection will change when enumerating over it

                foreach (var i in instrs)
                    ghbproc.Remove(i);
            }
            #endregion

            #region SetMount
            {
                var setMount = typeDef_Mount.GetMethod("SetMount");

                var smb = setMount.Body;
                var smproc = smb.GetILProcessor();

                // if (type == m || type <= -1 || type > 14) return;
                // but the first part should be kept for obvious reasons
                OpCode[] toRem =
                {
                    OpCodes.Ldarg_1,
                    OpCodes.Ldc_I4_M1,
                    OpCodes.Ble,

                    OpCodes.Ldarg_1,
                    OpCodes.Ldc_I4,
                    OpCodes.Blt
                };

                var instr = smb.FindInstrSeqStart(toRem);

                smproc.RemoveInstructions(instr, toRem.Length);

                // because of the state of the IL code right now, it will return either way.
                // to fix this, a br.s <instruction after first ret> has to be injected before the first ret,
                // so the code jumps to the ret iff type == m, and it skips the ret otherwise.

                toRem = new[] { OpCodes.Ret };
                var ret = smb.FindInstrSeqStart(toRem);

                smproc.InsertBefore(ret, Instruction.Create(OpCodes.Br_S, ret.Next));
            }
            #endregion
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_Mount = memRes.GetType("Terraria.Mount");

            WrapMethods();
            AddFieldForBHandler();
            Remove_FromFields();
            RemoveTypeLimitations();
        }
    }
}
