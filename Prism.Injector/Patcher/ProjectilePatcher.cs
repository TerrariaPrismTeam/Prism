using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Prism.Injector.Patcher
{
    static class ProjectilePatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        static TypeSystem ts;
        static TypeDefinition projectile_t;

        static void WrapSetDefaults()
        {
            MethodDefinition invokeOnSetDefaults;
            var onSetDefaultsDel = CecilHelper.CreateDelegate(c, "Terraria.PrismInjections", "Projectile_OnSetDefaultsDelegate", ts.Void, out invokeOnSetDefaults, projectile_t, ts.Int32);

            var setDefaults = projectile_t.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, ts.Int32);

            var newSetDefaults = WrapperHelper.ReplaceAndHook(setDefaults, invokeOnSetDefaults);

            WrapperHelper.ReplaceAllMethodRefs(c, setDefaults, newSetDefaults);
        }
        static void AddFieldForBHandler()
        {
            projectile_t.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, ts.Object));
        }

        internal static void Patch()
        {
            c = TerrariaPatcher.c;
            r = TerrariaPatcher.r;

            ts = c.PrimaryAssembly.MainModule.TypeSystem;
            projectile_t = r.GetType("Terraria.Projectile");

            WrapSetDefaults();
            AddFieldForBHandler();
        }
    }
}
