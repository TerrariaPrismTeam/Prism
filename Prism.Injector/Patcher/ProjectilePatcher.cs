using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Prism.Injector.Patcher
{
    static class ProjectilePatcher
    {
        static DNContext   context;
        static MemberResolver memRes ;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Proj;

        static void WrapMethods()
        {
            typeDef_Proj.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.Int32 }).Wrap(context);

            typeDef_Proj.GetMethod("NewProjectile").Wrap(context, "Terraria.PrismInjections", "Projectile_NewProjectileDel", "P_OnNewProjectile");

            typeDef_Proj.GetMethod("AI"       , MethodFlags.Public | MethodFlags.Instance               ).Wrap(context);
            typeDef_Proj.GetMethod("Update"   , MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.Int32 }).Wrap(context);
            typeDef_Proj.GetMethod("Kill"     , MethodFlags.Public | MethodFlags.Instance               ).Wrap(context);
            typeDef_Proj.GetMethod("Colliding", MethodFlags.Public | MethodFlags.Instance               ).Wrap(context);
        }
        static void AddFieldForBHandler()
        {
            typeDef_Proj.Fields.Add(new FieldDefUser("P_BHandler", new FieldSig(typeSys.Object), FieldAttributes.Public));
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Proj = memRes.GetType("Terraria.Projectile");

            WrapMethods();
            AddFieldForBHandler();
        }
    }
}
