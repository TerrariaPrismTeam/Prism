using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Defs;
using Terraria;

namespace Prism.API
{
    public enum RecipeRequires
    {
        Water,
        Lava,
        Honey,
        Tile // TEMPORARY
    }

    public class Recipes
    {
        public static int Create(ItemRef result, int amt, params object[] recipe)
        {
            return Create(result.Resolve().Type, amt, recipe);
        }

        public static int Create(ItemDef result, int amt, params object[] recipe)
        {
            return Create(result.Type, amt, recipe);
        }

        public static int Create(int result, int amt, params object[] recipe)
        {
            Recipe.newRecipe.createItem.SetDefaults(result);
            Recipe.newRecipe.createItem.stack = amt;

            int itemNum = 0;
            int tileNum = 0;

            for (int i=0; i<recipe.Length; i++)
            {
                Type type = recipe[i].GetType();
                if (type == typeof(ItemRef) || type == typeof(ItemDef))
                {
                    if (itemNum >= Recipe.maxRequirements - 1)
                    {
                        throw new ArgumentException("Attempted to add too many required items to recipe", "recipe");
                    }

                    if (type == typeof(ItemRef))
                    {
                        recipe[i] = ((ItemRef)recipe[i]).Resolve();
                    }

                    Recipe.newRecipe.requiredItem[itemNum].SetDefaults(((ItemDef)recipe[i]).Type);
                    Recipe.newRecipe.requiredItem[itemNum].stack = (int)recipe[++i];

                    itemNum++;
                }
                /* TODO TileRef / TileDef
                else if (type == typeof(TileRef) || type == typeof(TileDef))
                {
                    if (tileNum >= Recipe.maxRequirements - 1)
                    {
                        throw new ArgumentException("Attempted to add too many required tiles to recipe", "recipe");
                    }
                    
                    if (type == typeof(TileRef))
                    {
                        recipe[i] = ((TileRef)recipe[i]).Resolve();
                    }

                    Recipe.newRecipe.requiredTile[tileNum] = ((TileDef)recipe[i]).Type;

                    tileNum++;
                }
                */
                else if (type == typeof(RecipeRequires))
                {
                    switch ((RecipeRequires)recipe[i])
                    {
                        case RecipeRequires.Water:
                            Recipe.newRecipe.needWater = true;
                            break;
                        case RecipeRequires.Lava:
                            Recipe.newRecipe.needLava = true;
                            break;
                        case RecipeRequires.Honey:
                            Recipe.newRecipe.needHoney = true;
                            break;
                        case RecipeRequires.Tile:
                            // Temporarilly tiles will be defined by RecipeRequires.Tile
                            if (tileNum >= Recipe.maxRequirements - 1)
                            {
                                throw new ArgumentException("Attempted to add too many required tiles to recipe", "recipe");
                            }

                            Recipe.newRecipe.requiredTile[tileNum] = (ushort)recipe[++i];

                            tileNum++;
                            break;
                    }
                }
                else
                {
                    throw new ArgumentException("Recipe definition contained invalid data at index " + i + " of type '" + type.ToString() + "'", "recipe");
                }
            }

            Recipe.AddRecipe();

            ItemDef.ByType[result].Recipes.Add(Main.recipe[Recipe.numRecipes - 1]);

            return Recipe.numRecipes - 1;
        }

        internal static void AddVanillaRecipeReferences()
        {
            for (int i=0; i<Recipe.numRecipes; i++)
            {
                ItemDef.ByType[Main.recipe[i].createItem.type].Recipes.Add(Main.recipe[i]);
            }
        }
    }
}
