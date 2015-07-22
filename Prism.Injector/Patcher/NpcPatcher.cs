using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class NpcPatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        static void WrapSetDefaults()
        {
            var ts = c.PrimaryAssembly.MainModule.TypeSystem;

            var npc_t = r.GetType("Terraria.NPC");

            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = CecilHelper.CreateDelegate(c, "Terraria.PrismInjections", "NPC_OnSetDefaultsDelegate", ts.Void, out invokeOnSetDefaults, npc_t, ts.Int32, ts.Single);
            c.PrimaryAssembly.MainModule.Types.Add(onSetDefaultsDel);

            var onSetDefaults = new FieldDefinition("OnSetDefaults", FieldAttributes.Public | FieldAttributes.Static, onSetDefaultsDel);

            npc_t.Fields.Add(onSetDefaults);

            var setDefaults = npc_t.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, ts.Int32, ts.Single);
            setDefaults.Name = "RealSetDefaults";

            var newSetDefaults = new MethodDefinition("SetDefaults", setDefaults.Attributes, ts.Void);
            foreach (var p in setDefaults.Parameters)
                newSetDefaults.Parameters.Add(p);

            newSetDefaults.Body.MaxStackSize = 4;
            var nsd_ilproc = newSetDefaults.Body.GetILProcessor();

            // NPC.OnSetDefaults(this, Type, scaleOverride);
            /*
            ldsfld class Terraria.PrismInjections.NPC_OnSetDefaultsDelegate Terraria.NPC::OnSetDefaults
            ldarg.0
            ldarg.1
            ldarg.2
            callvirt instance void Terraria.PrismInjections.NPC_OnSetDefaultsDelegate::Invoke(class Terraria.NPC, int32, float32)

            ret
            */
            nsd_ilproc.Emit(OpCodes.Ldsfld, onSetDefaults);
            nsd_ilproc.Emit(OpCodes.Ldarg_0);
            nsd_ilproc.Emit(OpCodes.Ldarg_1);
            nsd_ilproc.Emit(OpCodes.Ldarg_2);
            nsd_ilproc.Emit(OpCodes.Callvirt, invokeOnSetDefaults);
            nsd_ilproc.Emit(OpCodes.Ret);

            npc_t.Methods.Add(newSetDefaults);
        }

        public static void Patch()
        {
            c = TerrariaPatcher.c;
            r = TerrariaPatcher.r;

            WrapSetDefaults();
        }
    }
}
