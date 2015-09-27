using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Prism.Injector.Patcher
{
    static class RecipePatcher
    {
        static CecilContext   context;
        static MemberResolver memRes ;

        static TypeSystem typeSys;
        static TypeDefinition typeDef_Recipe;

        static void AddGroupRecipeField()
        {
            typeDef_Recipe.Fields.Add(new FieldDefinition("P_GroupDef", FieldAttributes.Public, typeSys.Object));
        }
        static void WrapMethods()
        {
            typeDef_Recipe.GetMethod("FindRecipes").Wrap(context);
        }

        internal static void Patch()
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.MainModule.TypeSystem;
            typeDef_Recipe = memRes.GetType("Terraria.Recipe");

            AddGroupRecipeField();
            WrapMethods();
        }
    }
}
