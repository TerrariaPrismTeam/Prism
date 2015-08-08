using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Prism.Injector.Patcher
{
    static class ItemPatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        static TypeSystem ts;
        static TypeDefinition item_t;

        static void WrapSetDefaults()
        {
            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = CecilHelper.CreateDelegate(c, "Terraria.PrismInjections", "Item_OnSetDefaultsDelegate", ts.Void, out invokeOnSetDefaults, item_t, ts.Int32, ts.Boolean);

            var setDefaults = item_t.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, ts.Int32, ts.Boolean);

            var newSetDefaults = WrapperHelper.ReplaceAndHook(setDefaults, invokeOnSetDefaults);

            WrapperHelper.ReplaceAllMethodRefs(c, setDefaults, newSetDefaults);
        }
        static void AddFieldForBHandler()
        {
            item_t.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, ts.Object));
        }

        internal static void Patch()
        {
            c = TerrariaPatcher.c;
            r = TerrariaPatcher.r;

            ts = c.PrimaryAssembly.MainModule.TypeSystem;
            item_t = r.GetType("Terraria.Item");

            WrapSetDefaults();
            AddFieldForBHandler();
        }
    }
}
