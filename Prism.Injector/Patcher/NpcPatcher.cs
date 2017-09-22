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
            typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.Int32 , typeSys.Single})
                .Wrap(context, "Terraria.PrismInjections", "NPC_SetDefaultsDel_Id"  , "P_OnSetDefaultsById"  );

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
        }

        static void InjectBuffEffectsCall()
        {
            var upSoulDrain = typeDef_NPC.GetMethod("UpdateNPC_SoulDrainDebuff");

            MethodDef invokeEffects;
            var onBuffEffects = context.CreateDelegate("Terraria.PrismInjections", "NPC_BuffEffectsDel", typeSys.Void, out invokeEffects, typeDef_NPC.ToTypeSig());

            var buffEffects = new FieldDefUser("P_OnBuffEffects", new FieldSig(onBuffEffects.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            typeDef_NPC.Fields.Add(buffEffects);

            var unb = upSoulDrain.Body;
            using (var unproc = unb.GetILProcessor())
            {
                var first = unb.Instructions[0];

                unproc.InsertBefore(first, Instruction.Create(OpCodes.Ldsfld, buffEffects));
                unproc.EmitWrapperCall(invokeEffects, first);
            }
        }
        static void InitBuffBHandlerArray(Action<string> log)
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

        internal static void Patch(Action<string> log)
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_NPC = memRes.GetType("Terraria.NPC" );

            WrapMethods();

            AddFieldForBHandler();
            AddFieldsForAudio();

            InsertInitialize();
            InjectBuffEffectsCall();
            InitBuffBHandlerArray(log);
        }
    }
}
