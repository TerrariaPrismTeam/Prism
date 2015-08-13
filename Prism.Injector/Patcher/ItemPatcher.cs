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
        static CecilContext   context;
        static MemberResolver  memRes;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_Item;

        static void WrapSetDefaults()
        {
            typeDef_Item.GetMethod("SetDefaults", MethodFlags.Public | MethodFlags.Instance, typeSys.Int32, typeSys.Boolean).Wrap(context);
        }
        static void AddFieldForBHandler()
        {
            typeDef_Item.Fields.Add(new FieldDefinition("P_BHandler", FieldAttributes.Public, typeSys.Object));
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes = TerrariaPatcher.memRes;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_Item = memRes.GetType("Terraria.Item");

            WrapSetDefaults();
            AddFieldForBHandler();
        }
    }
}
