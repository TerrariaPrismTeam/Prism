using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class NpcPatcher
    {
        static DNContext   context;
        static MemberResolver  memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_NPC;

        static void WrapMethods()
        {
            typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.Int32 , typeSys.Single}).Wrap(context, "Terraria.PrismInjections", "NPC_SetDefaultsDel_Id"  , "P_OnSetDefaultsById"  );
            typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.String                }).Wrap(context, "Terraria.PrismInjections", "NPC_SetDefaultsDel_Name", "P_OnSetDefaultsByName");

            typeDef_NPC.GetMethod("AI"       , MethodFlags.Public | MethodFlags.Instance                         ).Wrap(context);
            typeDef_NPC.GetMethod("UpdateNPC", MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.Int32 }).Wrap(context);
            typeDef_NPC.GetMethod("NPCLoot"  , MethodFlags.Public | MethodFlags.Instance                         ).Wrap(context);

            typeDef_NPC.GetMethod("AddBuff"  , MethodFlags.Public | MethodFlags.Instance).Wrap(context);
            typeDef_NPC.GetMethod("FindFrame", MethodFlags.Public | MethodFlags.Instance).Wrap(context);
        }
        static void AddFieldForBHandler()
        {
            typeDef_NPC.Fields.Add(new FieldDefUser("P_BHandler"    , new FieldSig(typeSys.Object)                                  , FieldAttributes.Public));
            typeDef_NPC.Fields.Add(new FieldDefUser("P_BuffBHandler", new FieldSig(memRes.ReferenceOf(typeof(object[])).ToTypeSig()), FieldAttributes.Public));
        }
        static void AddFieldsForAudio()
        {
            typeDef_NPC.Fields.Add(new FieldDefUser("P_Music"       , new FieldSig(typeSys.Object), FieldAttributes.Public));
            typeDef_NPC.Fields.Add(new FieldDefUser("P_SoundOnHit"  , new FieldSig(typeSys.Object), FieldAttributes.Public));
            typeDef_NPC.Fields.Add(new FieldDefUser("P_SoundOnDeath", new FieldSig(typeSys.Object), FieldAttributes.Public));
        }
        static void InsertInitialize()
        {
            typeDef_NPC.GetMethod("NewNPC", MethodFlags.Public | MethodFlags.Static,
                new[] { typeSys.Int32, typeSys.Int32, typeSys.Int32, typeSys.Int32,
                typeSys.Single, typeSys.Single, typeSys.Single, typeSys.Single,
                typeSys.Int32 }).Wrap(context);
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
        static void ReplaceSoundHitCalls()
        {
            #region ReflectProjectile
            { // introduce a new scope level here so variables can be reused in the StrikeNPC part
                var reflectProjectile = typeDef_NPC.GetMethod("ReflectProjectile");

                // first statement in the method is "Main.PlaySound(...);"
                // instruction after the "call Main.PlaySound" is a ldc.i4.0 (first one in the method)

                MethodDef invokeSoundHit;
                var onReflProjSoundHit = context.CreateDelegate("Terraria.PrismInjections", "NPC_ReflectProjectile_PlaySoundHitDel", typeSys.Void, out invokeSoundHit, typeDef_NPC.ToTypeSig(), typeSys.Int32);

                var reflectProjectile_PlaySoundHit = new FieldDefUser("P_ReflectProjectile_PlaySoundHit", new FieldSig(onReflProjSoundHit.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_NPC.Fields.Add(reflectProjectile_PlaySoundHit);

                using (var rpproc = reflectProjectile.Body.GetILProcessor())
                {
                    rpproc.RemoveInstructions(reflectProjectile.Body.Instructions.TakeWhile(i => i.OpCode.Code != Code.Ldc_I4_0).ToArray() /* must enumerate this already, invalidoperation will be thrown otherwise */);

                    var first = reflectProjectile.Body.Instructions[0];

                    rpproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, reflectProjectile_PlaySoundHit));
                    rpproc.EmitWrapperCall(invokeSoundHit, first);
                }
            }
            #endregion

            #region StrikeNPC
            {
                var strikeNpc = typeDef_NPC.GetMethod("StrikeNPC");

                MethodDef invokeSoundHit;
                var onStrikeNpcSoundHit = context.CreateDelegate("Terraria.PrismInjections", "NPC_StrikeNPC_PlaySoundHitDel", typeSys.Void, out invokeSoundHit,
                    new[] { typeDef_NPC.ToTypeSig(), typeSys.Int32, typeSys.Single, typeSys.Int32, typeSys.Boolean, typeSys.Boolean, typeSys.Boolean });

                var strikeNpc_PlaySoundHit = new FieldDefUser("P_StrikeNPC_PlaySoundHit", new FieldSig(onStrikeNpcSoundHit.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_NPC.Fields.Add(strikeNpc_PlaySoundHit);

                var snb = strikeNpc.Body;
                using (var snproc = snb.GetILProcessor())
                {
                    OpCode[] toRem =
                    {
                        OpCodes.Ldarg_0,
                        OpCodes.Ldfld, // NPC.soundHit
                        OpCodes.Ldc_I4_0,
                        OpCodes.Ble_S, // <after the call>

                        OpCodes.Ldc_I4_3, // soundHit ID
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Ldfld, // NPC.soundHit
                        OpCodes.Call // Main.PlaySound(int, int, int, int)
                    };

                    var instrs = snb.FindInstrSeqStart(toRem);
                    var instrs_ = snproc.RemoveInstructions(instrs, toRem.Length);

                    Instruction newF;
                    snproc.InsertBefore(instrs_, newF = Instruction.Create(OpCodes.Ldsfld, strikeNpc_PlaySoundHit));
                    snproc.EmitWrapperCall(invokeSoundHit, instrs_);

                    for (int i = 0; i < snb.Instructions.Count; i++)
                        if (snb.Instructions[i].Operand == instrs)
                            snb.Instructions[i].Operand = newF;
                }
            }
            #endregion
        }
        static void ReplaceSoundKilledCalls()
        {
            #region checkDead
            {
                var checkDead = typeDef_NPC.GetMethod("checkDead");

                MethodDef invokeSoundKilled;
                var onCheckDeadSoundKilled = context.CreateDelegate("Terraria.PrismInjections", "NPC_checkDead_PlaySoundKilledDel", typeSys.Void, out invokeSoundKilled, typeDef_NPC.ToTypeSig());

                var checkDead_PlaySoundKilled = new FieldDefUser("P_checkDead_PlaySoundKilled", new FieldSig(onCheckDeadSoundKilled.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_NPC.Fields.Add(checkDead_PlaySoundKilled);

                var cdb = checkDead.Body;
                using (var cdproc = cdb.GetILProcessor())
                {
                    OpCode[] toRem =
                    {
                        OpCodes.Ldarg_0,
                        OpCodes.Ldfld, // NPC.soundHit
                        OpCodes.Ldc_I4_0,
                        OpCodes.Ble_S, // <after the call>

                        OpCodes.Ldc_I4_4, // soundKilled ID
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Ldflda, // Entity.position
                        OpCodes.Ldfld, // Vector2.X
                        OpCodes.Conv_I4,
                        OpCodes.Ldarg_0,
                        OpCodes.Ldfld, // NPC.soundKilled
                        OpCodes.Call // Main.PlaySound(int, int, int, int)
                    };

                    var instrs = cdb.FindInstrSeqStart(toRem);
                    var instrs_ = cdproc.RemoveInstructions(instrs, toRem.Length);

                    Instruction newF;
                    cdproc.InsertBefore(instrs_, newF = Instruction.Create(OpCodes.Ldsfld, checkDead_PlaySoundKilled));
                    cdproc.EmitWrapperCall(invokeSoundKilled, instrs_);

                    for (int i = 0; i < cdb.Instructions.Count; i++)
                        if (cdb.Instructions[i].Operand == instrs)
                            cdb.Instructions[i].Operand = newF;
                }
            }
            #endregion

            #region RealAI
            {
                // this happens AFTER AI has been wrapped, thus RealAI has to be used instead of AI
                
                var realAI = typeDef_NPC.GetMethod("RealAI");

                MethodDef invokeSoundKilled;
                var onRealAISoundKilled = context.CreateDelegate("Terraria.PrismInjections", "NPC_RealAI_PlaySoundKilledDel", typeSys.Void, out invokeSoundKilled, typeDef_NPC.ToTypeSig());

                var realAI_PlaySoundKilled = new FieldDefUser("P_RealAI_PlaySoundKilled", new FieldSig(onRealAISoundKilled.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
                typeDef_NPC.Fields.Add(realAI_PlaySoundKilled);

                OpCode[] toRem =
                {
                    OpCodes.Ldc_I4_4, // soundKilled ID
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldflda, // Entity.position
                    OpCodes.Ldfld, // Vector2.X
                    OpCodes.Conv_I4,
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // NPC.soundKilled
                    OpCodes.Call // Main.PlaySound(int, int, int, int)
                };

                using (var raproc = realAI.Body.GetILProcessor())
                {
                    var first = realAI.Body.FindInstrSeqStart(toRem);
                    first = raproc.RemoveInstructions(first, toRem.Length);

                    raproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, realAI_PlaySoundKilled));
                    raproc.EmitWrapperCall(invokeSoundKilled, first);
                }
            }
            #endregion
        }
        static void InjectBuffEffectsCall()
        {
            var updateNpc = typeDef_NPC.GetMethod("RealUpdateNPC");

            MethodDef invokeEffects;
            var onBuffEffects = context.CreateDelegate("Terraria.PrismInjections", "NPC_BuffEffectsDel", typeSys.Void, out invokeEffects, typeDef_NPC.ToTypeSig());

            var buffEffects = new FieldDefUser("P_OnBuffEffects", new FieldSig(onBuffEffects.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_NPC.Fields.Add(buffEffects);

            OpCode[] toRem =
            {
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Brfalse
            };

            var unb = updateNpc.Body;
            using (var unproc = unb.GetILProcessor())
            {
                Instruction instr;
                var soulDrain = typeDef_NPC.GetField("soulDrain");

                int start = 0;
                while (true)
                {
                    instr = unb.FindInstrSeqStart(toRem, start);

                    if (context.SigComparer.Equals((FieldDef)instr.Next(unproc).Operand, soulDrain))
                        break;
                    else
                        start = unb.Instructions.IndexOf(instr) + 1;
                }

                unproc.InsertBefore(instr, Instruction.Create(OpCodes.Ldsfld, buffEffects));
                unproc.EmitWrapperCall(invokeEffects, instr);
            }
        }
        static void InitBuffBHandlerArray()
        {
            var ctor = typeDef_NPC.GetConstructor();
            var buffBHandler = typeDef_NPC.GetField("P_BuffBHandler");

            using (var cproc = ctor.Body.GetILProcessor())
            {
                var l = ctor.Body.Instructions.Last().Previous(cproc).Previous(cproc);

                cproc.InsertBefore(l, Instruction.Create(OpCodes.Ldarg_0));
                cproc.InsertBefore(l, Instruction.Create(OpCodes.Ldc_I4_5));
                cproc.InsertBefore(l, Instruction.Create(OpCodes.Newarr, typeSys.Object));
                cproc.InsertBefore(l, Instruction.Create(OpCodes.Stfld, buffBHandler));
            }
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_NPC = memRes.GetType("Terraria.NPC" );

            WrapMethods();

            AddFieldForBHandler();
            AddFieldsForAudio();

            InsertInitialize();
            ReplaceSoundHitCalls();
            ReplaceSoundKilledCalls();
            InjectBuffEffectsCall();
            InitBuffBHandlerArray();
        }
    }
}
