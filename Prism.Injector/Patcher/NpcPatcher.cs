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

        static void WrapMethods()
        {
            typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32 , typeSys.Single).Wrap(context, "Terraria.PrismInjections", "NPC_SetDefaultsDel_Id"  , "P_OnSetDefaultsById"  );
            typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.String                ).Wrap(context, "Terraria.PrismInjections", "NPC_SetDefaultsDel_Name", "P_OnSetDefaultsByName");

            typeDef_NPC.GetMethod("AI"       , MethodFlags.Public | MethodFlags.Instance               ).Wrap(context);
            typeDef_NPC.GetMethod("UpdateNPC", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32).Wrap(context);
            typeDef_NPC.GetMethod("NPCLoot"  , MethodFlags.Public | MethodFlags.Instance               ).Wrap(context);

            typeDef_NPC.GetMethod("AddBuff"  , MethodFlags.Public | MethodFlags.Instance).Wrap(context);
            typeDef_NPC.GetMethod("FindFrame", MethodFlags.Public | MethodFlags.Instance).Wrap(context);
        }
        static void AddFieldForBHandler()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
            typeDef_NPC.Fields.Add(new FieldDefinition("P_BuffBHandler", FieldAttributes.Public, memRes.ReferenceOf(typeof(object[]))));
        }
        static void AddFieldsForAudio()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_Music"       , FieldAttributes.Public, typeSys.Object));
            typeDef_NPC.Fields.Add(new FieldDefinition("P_SoundOnHit"  , FieldAttributes.Public, typeSys.Object));
            typeDef_NPC.Fields.Add(new FieldDefinition("P_SoundOnDeath", FieldAttributes.Public, typeSys.Object));
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
        static void ReplaceSoundHitCalls()
        {
            #region ReflectProjectile
            { // introduce a new scope level here so variables can be reused in the StrikeNPC part
                var reflectProjectile = typeDef_NPC.GetMethod("ReflectProjectile");

                // first statement in the method is "Main.PlaySound(...);"
                // instruction after the "call Main.PlaySound" is a ldc.i4.0 (first one in the method)

                MethodDefinition invokeSoundHit;
                var onReflProjSoundHit = context.CreateDelegate("Terraria.PrismInjections", "NPC_ReflectProjectile_PlaySoundHitDel", typeSys.Void, out invokeSoundHit, typeDef_NPC, typeSys.Int32);

                var reflectProjectile_PlaySoundHit = new FieldDefinition("P_ReflectProjectile_PlaySoundHit", FieldAttributes.Public | FieldAttributes.Static, onReflProjSoundHit);
                typeDef_NPC.Fields.Add(reflectProjectile_PlaySoundHit);

                var rpproc = reflectProjectile.Body.GetILProcessor();

                rpproc.RemoveInstructions(reflectProjectile.Body.Instructions.TakeWhile(i => i.OpCode.Code != Code.Ldc_I4_0).ToArray() /* must enumerate this already, invalidoperation will be thrown otherwise */);

                var first = reflectProjectile.Body.Instructions[0];

                rpproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, reflectProjectile_PlaySoundHit));
                rpproc.EmitWrapperCall(invokeSoundHit, first);
            }
            #endregion

            #region StrikeNPC
            {
                var strikeNpc = typeDef_NPC.GetMethod("StrikeNPC");

                MethodDefinition invokeSoundHit;
                var onStrikeNpcSoundHit = context.CreateDelegate("Terraria.PrismInjections", "NPC_StrikeNPC_PlaySoundHitDel", typeSys.Void, out invokeSoundHit,
                    typeDef_NPC, typeSys.Int32, typeSys.Single, typeSys.Int32, typeSys.Boolean, typeSys.Boolean, typeSys.Boolean);

                var strikeNpc_PlaySoundHit = new FieldDefinition("P_StrikeNPC_PlaySoundHit", FieldAttributes.Public | FieldAttributes.Static, onStrikeNpcSoundHit);
                typeDef_NPC.Fields.Add(strikeNpc_PlaySoundHit);

                var snb = strikeNpc.Body;
                var snproc = snb.GetILProcessor();

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
                instrs = snproc.RemoveInstructions(instrs, toRem.Length);

                snproc.InsertBefore(instrs, Instruction.Create(OpCodes.Ldsfld, strikeNpc_PlaySoundHit));
                snproc.EmitWrapperCall(invokeSoundHit, instrs);
            }
            #endregion
        }
        static void ReplaceSoundKilledCalls()
        {
            #region checkDead
            {
                var checkDead = typeDef_NPC.GetMethod("checkDead");

                MethodDefinition invokeSoundKilled;
                var onCheckDeadSoundKilled = context.CreateDelegate("Terraria.PrismInjections", "NPC_checkDead_PlaySoundKilledDel", typeSys.Void, out invokeSoundKilled, typeDef_NPC);

                var checkDead_PlaySoundKilled = new FieldDefinition("P_checkDead_PlaySoundKilled", FieldAttributes.Public | FieldAttributes.Static, onCheckDeadSoundKilled);
                typeDef_NPC.Fields.Add(checkDead_PlaySoundKilled);

                var cdb = checkDead.Body;
                var cdproc = cdb.GetILProcessor();

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
                instrs = cdproc.RemoveInstructions(instrs, toRem.Length);

                cdproc.InsertBefore(instrs, Instruction.Create(OpCodes.Ldsfld, checkDead_PlaySoundKilled));
                cdproc.EmitWrapperCall(invokeSoundKilled, instrs);
            }
            #endregion

            #region RealAI
            {
                // this happens AFTER AI has been wrapped, thus RealAI has to be used instead of AI

                var realAI = typeDef_NPC.GetMethod("RealAI");

                MethodDefinition invokeSoundKilled;
                var onRealAISoundKilled = context.CreateDelegate("Terraria.PrismInjections", "NPC_RealAI_PlaySoundKilledDel", typeSys.Void, out invokeSoundKilled, typeDef_NPC);

                var realAI_PlaySoundKilled = new FieldDefinition("P_RealAI_PlaySoundKilled", FieldAttributes.Public | FieldAttributes.Static, onRealAISoundKilled);
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

                var raproc = realAI.Body.GetILProcessor();

                var first = realAI.Body.FindInstrSeqStart(toRem);
                first = raproc.RemoveInstructions(first, toRem.Length);

                raproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, realAI_PlaySoundKilled));
                raproc.EmitWrapperCall(invokeSoundKilled, first);
            }
            #endregion
        }
        static void InjectBuffEffectsCall()
        {
            var updateNpc = typeDef_NPC.GetMethod("RealUpdateNPC");

            MethodDefinition invokeEffects;
            var onBuffEffects = context.CreateDelegate("Terraria.PrismInjections", "NPC_BuffEffectsDel", typeSys.Void, out invokeEffects, typeDef_NPC);

            var buffEffects = new FieldDefinition("P_OnBuffEffects", FieldAttributes.Public | FieldAttributes.Static, onBuffEffects);
            typeDef_NPC.Fields.Add(buffEffects);

            OpCode[] toRem =
            {
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Brfalse
            };

            var unb = updateNpc.Body;
            var unproc = unb.GetILProcessor();

            Instruction instr;
            int start = 0;
            while (true)
            {
                instr = unb.FindInstrSeqStart(toRem, start);

                if (instr.Next.Operand == typeDef_NPC.GetField("soulDrain"))
                    break;
                else
                    start = unb.Instructions.IndexOf(instr) + 1;
            }

            unproc.InsertBefore(instr, Instruction.Create(OpCodes.Ldsfld, buffEffects));
            unproc.EmitWrapperCall(invokeEffects, instr);
        }
        static void InitBuffBHandlerArray()
        {
            var ctor = typeDef_NPC.GetConstructor();
            var buffBHandler = typeDef_NPC.GetField("P_BuffBHandler");

            var cproc = ctor.Body.GetILProcessor();

            var l = ctor.Body.Instructions.Last().Previous.Previous;

            cproc.InsertBefore(l, Instruction.Create(OpCodes.Ldarg_0));
            cproc.InsertBefore(l, Instruction.Create(OpCodes.Ldc_I4_5));
            cproc.InsertBefore(l, Instruction.Create(OpCodes.Newarr, typeSys.Object));
            cproc.InsertBefore(l, Instruction.Create(OpCodes.Stfld, buffBHandler));
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
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
