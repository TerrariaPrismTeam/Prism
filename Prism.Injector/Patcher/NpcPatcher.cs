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

        static void WrapSetDefaults()
        {
            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = CecilHelper.CreateDelegate(context, "Terraria.PrismInjections", "NPC_OnSetDefaultsDelegate", typeSys.Void, out invokeOnSetDefaults, typeDef_NPC, typeSys.Int32, typeSys.Single);

            var setDefaults = typeDef_NPC.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32, typeSys.Single);

            var newSetDefaults = WrapperHelper.ReplaceAndHook(setDefaults, invokeOnSetDefaults);

            WrapperHelper.ReplaceAllMethodRefs(context, setDefaults, newSetDefaults);
        }
        static void AddFieldForBHandler()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
        }
        static void AddFieldForMusic()
        {
            typeDef_NPC.Fields.Add(new FieldDefinition("P_Music", FieldAttributes.Public, typeSys.Object));
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
        }
    }
}
