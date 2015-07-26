using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Defs;
using Terraria;

namespace Prism.API
{
    public class Recipes
    {
        public static int Create(Dictionary<int, int> ingredients, ItemDef product, int amt = 1)
        {
            return Create(ingredients, product.Type, amt);
        }

        public static int Create(Dictionary<ItemDef, int> ingredients, ItemDef product, int amt = 1)
        {
            return Create(ingredients, product.Type, amt);
        }

        public static int Create(Dictionary<ItemDef, int> ingredients, int product, int amt = 1)
        {
            Recipe r = new Recipe();
            r.createItem.SetDefaults(product);
            r.createItem.stack = amt;
            r.requiredItem = new Item[ingredients.Count + 1];
            int req = 0;
            foreach (var i in ingredients)
            {
                var item = new Item();
                item.SetDefaults(i.Key.Type);
                item.stack = i.Value;
                r.requiredItem[req++] = item;
            }
            r.requiredItem[r.requiredItem.Length - 1] = new Item();
            r.requiredItem[r.requiredItem.Length - 1].SetDefaults(0);
            Array.Resize(ref Main.recipe, Main.recipe.Length + 1);
            Main.recipe[Recipe.numRecipes++] = r;
            return Recipe.numRecipes - 1;
        }

        public static int Create(Dictionary<int, int> ingredients, int product, int amt = 1)
        {
            Recipe r = new Recipe();
            r.createItem.SetDefaults(product);
            r.createItem.stack = amt;
            r.requiredItem = new Item[ingredients.Count + 1];
            int req = 0;
            foreach (var i in ingredients)
            {
                var item = new Item();
                item.SetDefaults(i.Key);
                item.stack = i.Value;
                r.requiredItem[req++] = item;
            }
            r.requiredItem[r.requiredItem.Length - 1] = new Item();
            r.requiredItem[r.requiredItem.Length - 1].SetDefaults(0);
            Array.Resize(ref Main.recipe, Main.recipe.Length + 1);
            Main.recipe[Recipe.numRecipes++] = r;
            return Recipe.numRecipes - 1;
        }        
    }
}
