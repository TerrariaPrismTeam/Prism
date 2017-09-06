using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Prism.Injector.Patcher
{
    static class RecipePatcher
    {
        static DNContext   context;
        static MemberResolver memRes ;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_Recipe;

        static void AddGroupRecipeField()
        {
            typeDef_Recipe.Fields.Add(new FieldDefUser("P_GroupDef", new FieldSig(typeSys.Object), FieldAttributes.Public));
        }
        static void WrapMethods()
        {
            typeDef_Recipe.GetMethod("FindRecipes").Wrap(context);
            typeDef_Recipe.GetMethod("Create"     ).Wrap(context);
        }

        internal static void Patch(Action<string> log)
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;
            typeDef_Recipe = memRes.GetType("Terraria.Recipe");

            AddGroupRecipeField();
            WrapMethods();
        }
    }
}
