using Prism.API;
using Prism.API.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Prism.Mods
{
    public class RecipeHelper : Recipe
    {
        private static new Recipe newRecipe = new Recipe();

        public static void CreateRecipe(int requiredItemId, int createItemId, int requiredStack = 1, int createStack = 1)
        {
            newRecipe.createItem.SetDefaults(requiredItemId);
            newRecipe.createItem.stack = createStack;
            newRecipe.requiredItem[0].SetDefaults(requiredItemId);
            newRecipe.requiredItem[0].stack = requiredStack;
            addRecipe();
        }

        public static void CreateRecipe(ItemDef requiredItem, ItemDef createItem, int requiredStack = 1, int createStack = 1)
        {
            newRecipe.createItem.SetDefaults(createItem.Type);
            newRecipe.createItem.stack = createStack;
            newRecipe.requiredItem[0].SetDefaults(requiredItem.Type);
            newRecipe.requiredItem[0].stack = requiredStack;
            addRecipe();
        }

        public static void CreateRecipe(int requiredItemId, ItemDef createItem, int requiredStack = 1, int createStack = 1)
        {
            newRecipe.createItem.SetDefaults(createItem.Type);
            newRecipe.createItem.stack = createStack;
            newRecipe.requiredItem[0].SetDefaults(requiredItemId);
            newRecipe.requiredItem[0].stack = requiredStack;
            addRecipe();
        }

        public static void CreateRecipe(ItemDef requiredItem, int createItemId, int requiredStack = 1, int createStack = 1)
        {
            newRecipe.createItem.SetDefaults(requiredItem.Type);
            newRecipe.createItem.stack = createStack;
            newRecipe.requiredItem[0].SetDefaults(createItemId);
            newRecipe.requiredItem[0].stack = requiredStack;
            addRecipe();
        }

        private static void addRecipe()
        {
            Main.recipe[numRecipes] = newRecipe;
            newRecipe = new Recipe();
            numRecipes++;
        }
    }
}
