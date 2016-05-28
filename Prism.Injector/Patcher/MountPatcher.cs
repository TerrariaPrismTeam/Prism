using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class MountPatcher
    {
        static DNContext context;
        static MemberResolver memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Mount;

        static void WrapMethods()
        {
            typeDef_Mount.GetMethod("SetMount"     ).Wrap(context);
            typeDef_Mount.GetMethod("Dismount"     ).Wrap(context);
            typeDef_Mount.GetMethod("Draw"         ).Wrap(context);
            typeDef_Mount.GetMethod("UpdateEffects").Wrap(context);
            typeDef_Mount.GetMethod("UpdateFrame"  ).Wrap(context);
            typeDef_Mount.GetMethod("Hover"        ).Wrap(context);
            typeDef_Mount.GetMethod("JumpHeight"   ).Wrap(context);
            typeDef_Mount.GetMethod("JumpSpeed"    ).Wrap(context);
        }

        static void AddFieldForBHandler()
        {
            typeDef_Mount.Fields.Add(new FieldDefUser("P_BHandler", new FieldSig(typeSys.Object), FieldAttributes.Public));
        }
        static void Remove_FromFields()
        {
            foreach (var fd in typeDef_Mount.Fields)
                if (fd.Name.String[0] == '_')
                    fd.Name = fd.Name.Substring(1);
        }
        static void RemoveTypeLimitations()
        {
            #region GetHeightBoost
            {
                var getHeightBoost = typeDef_Mount.GetMethod("GetHeightBoost");

                var ghbb = getHeightBoost.Body;
                using (var ghbproc = ghbb.GetILProcessor())
                {
                    // the method immediately returns when this.type <= -1 or > 14
                    // just remove the check + return
                    var instrs = getHeightBoost.Body.Instructions.TakeWhile(i => i.OpCode.Code != Code.Ret);
                    instrs = instrs.Concat(new[] { instrs.Last().Next(ghbproc) }).ToArray(); // not using ToArray will throw an exception because the collection will change when enumerating over it

                    foreach (var i in instrs)
                        ghbproc.Remove(i);
                }
            }
            #endregion

            #region SetMount
            {
                var setMount = typeDef_Mount.GetMethod("RealSetMount");

                var smb = setMount.Body;
                using (var smproc = smb.GetILProcessor())
                {
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

                    smproc.InsertBefore(ret, Instruction.Create(OpCodes.Br_S, ret.Next(smproc)));
                }
            }
            #endregion
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Mount = memRes.GetType("Terraria.Mount");

            WrapMethods();
            AddFieldForBHandler();
            Remove_FromFields();
            RemoveTypeLimitations();
        }
    }
}
