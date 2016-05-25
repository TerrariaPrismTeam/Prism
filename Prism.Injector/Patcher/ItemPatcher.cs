using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Prism.Injector.Patcher
{
    static class ItemPatcher
    {
        static DNContext   context;
        static MemberResolver  memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Item;

        static void WrapSetDefaults()
        {
            typeDef_Item.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.Int32 , typeSys.Boolean }).Wrap(context, "Terraria.PrismInjections", "Item_SetDefaultsDel_Id"  , "P_OnSetDefaultsById"  );
            typeDef_Item.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, new[] { typeSys.String                  }).Wrap(context, "Terraria.PrismInjections", "Item_SetDefaultsDel_Name", "P_OnSetDefaultsByName");
        }
        static void AddFieldForBHandler()
        {
            typeDef_Item.Fields.Add(new FieldDefUser("P_BHandler", new FieldSig(typeSys.Object), FieldAttributes.Public));
        }
        static void AddFieldForSound()
        {
            typeDef_Item.Fields.Add(new FieldDefUser("P_UseSound", new FieldSig(typeSys.Object), FieldAttributes.Public));
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Item = memRes.GetType("Terraria.Item");

            WrapSetDefaults();
            AddFieldForBHandler();
            AddFieldForSound();
        }
    }
}
