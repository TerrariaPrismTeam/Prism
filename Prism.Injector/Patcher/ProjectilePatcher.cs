using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Prism.Injector.Patcher
{
    static class ProjectilePatcher
    {
        static CecilContext   context;
        static MemberResolver memRes ;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_Proj;

        static void WrapMethods()
        {
            typeDef_Proj.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32).Wrap(context);

            typeDef_Proj.GetMethod("NewProjectile").Wrap(context, "Terraria.PrismInjections", "Projectile_NewProjectileDel", "P_OnNewProjectile");
            typeDef_Proj.GetMethod("AI"    , MethodFlags.Public | MethodFlags.Instance               ).Wrap(context);
            typeDef_Proj.GetMethod("Update", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32).Wrap(context);
            typeDef_Proj.GetMethod("Kill"  , MethodFlags.Public | MethodFlags.Instance               ).Wrap(context);
        }
        static void AddFieldForBHandler()
        {
            typeDef_Proj.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_Proj = memRes.GetType("Terraria.Projectile");

            WrapMethods();
            AddFieldForBHandler();
        }
    }
}
