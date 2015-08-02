using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    static class NpcPatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        static TypeSystem ts;
        static TypeDefinition npc_t;

        static void WrapSetDefaults()
        {
            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = CecilHelper.CreateDelegate(c, "Terraria.PrismInjections", "NPC_OnSetDefaultsDelegate", ts.Void, out invokeOnSetDefaults, npc_t, ts.Int32, ts.Single);

            var setDefaults = npc_t.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, ts.Int32, ts.Single);

            var newSetDefaults = WrapperHelper.ReplaceAndHook(setDefaults, invokeOnSetDefaults);

            WrapperHelper.ReplaceAllMethodRefs(c, setDefaults, newSetDefaults);
        }
        static void AddFieldForBHandler()
        {
            npc_t.Fields.Add(new FieldDefinition("BHandler", FieldAttributes.Public, ts.Object));
        }

        internal static void Patch()
        {
            c = TerrariaPatcher.c;
            r = TerrariaPatcher.r;

            ts = c.PrimaryAssembly.MainModule.TypeSystem;
            npc_t  = r.GetType("Terraria.NPC" );

            WrapSetDefaults();
            AddFieldForBHandler();
        }
    }
}
