using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Prism.Injector.Patcher
{
    static class ItemPatcher
    {
        static CecilContext   context;
        static MemberResolver  memRes;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_Item;

        static void WrapSetDefaults()
        {
            typeDef_Item.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32 , typeSys.Boolean).Wrap(context, "Terraria.PrismInjections", "Item_SetDefaultsDel_Id"  , "P_OnSetDefaultsById"  );
            typeDef_Item.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.String                 ).Wrap(context, "Terraria.PrismInjections", "Item_SetDefaultsDel_Name", "P_OnSetDefaultsByName");
        }
        static void AddFieldForBHandler()
        {
            typeDef_Item.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
        }
        static void AddFieldForSound()
        {
            typeDef_Item.Fields.Add(new FieldDefinition("P_UseSound", FieldAttributes.Public, typeSys.Object));
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_Item = memRes.GetType("Terraria.Item");

            WrapSetDefaults();
            AddFieldForBHandler();
            AddFieldForSound();
        }
    }
}
