using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Prism.Injector.Patcher
{
    public static class ItemPatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        public static void WrapSetDefaults()
        {
            var ts = c.PrimaryAssembly.MainModule.TypeSystem;

            var item_t = r.GetType("Terraria.Item");

            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = CecilHelper.CreateDelegate(c, "Terraria.PrismInjections", "Item_OnSetDefaultsDelegate", ts.Void, out invokeOnSetDefaults, item_t, ts.Int32, ts.Boolean);
            c.PrimaryAssembly.MainModule.Types.Add(onSetDefaultsDel);

            var onSetDefaults = new FieldDefinition("OnSetDefaults", FieldAttributes.Public | FieldAttributes.Static, onSetDefaultsDel);

            item_t.Fields.Add(onSetDefaults);

            var setDefaults = item_t.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, ts.Int32, ts.Boolean);
            setDefaults.Name = "RealSetDefaults";

            var newSetDefaults = new MethodDefinition("SetDefaults", setDefaults.Attributes, ts.Void);
            foreach (var p in setDefaults.Parameters)
                newSetDefaults.Parameters.Add(p);

            newSetDefaults.Body.MaxStackSize = 4;
            var nsd_ilproc = newSetDefaults.Body.GetILProcessor();

            // Item.OnSetDefaults(this, Type, noMatCheck);
            /*
            ldsfld class Terraria.PrismInjections.Item_OnSetDefaultsDelegate Terraria.Item::OnSetDefaults
            ldarg.0
            ldarg.1
            ldarg.2
            callvirt instance void Terraria.PrismInjections.Item_OnSetDefaultsDelegate::Invoke(class Terraria.Item, int32, bool)

            ret
            */
            nsd_ilproc.Emit(OpCodes.Ldsfld, onSetDefaults);
            nsd_ilproc.Emit(OpCodes.Ldarg_0);
            nsd_ilproc.Emit(OpCodes.Ldarg_1);
            nsd_ilproc.Emit(OpCodes.Ldarg_2);
            nsd_ilproc.Emit(OpCodes.Callvirt, invokeOnSetDefaults);
            nsd_ilproc.Emit(OpCodes.Ret);

            item_t.Methods.Add(newSetDefaults);
        }

        public static void Patch()
        {
            c = TerrariaPatcher.c;
            r = TerrariaPatcher.r;

            WrapSetDefaults();
        }
    }
}
