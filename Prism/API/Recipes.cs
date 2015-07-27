using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Defs;
using Terraria;

namespace Prism.API
{
    public enum CraftReq
    {
        Water,
        Lava,
        Honey,
        Tile //Temporary implementation of required tiles until TileDefs are implemented.
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

            for (int i = 0; i < recipe.Length; i++)
            {
                Type type = recipe[i].GetType();
                if (type == typeof(ItemRef) || type == typeof(ItemDef))
                {
                    if (i == recipe.Length - 1 || recipe[i + 1].GetType() != typeof(int))
                    {
                        throw new ArgumentException("Specifying an ingredient ItemDef or ItemRef requires the amount to be specified subsequentially as an integer.", "recipe");
                    }
                    else if (itemNum >= Recipe.maxRequirements - 1)
                    {
                        throw new ArgumentException("Exceeded the hardcoded maximum number of ingredients while constructing recipe for '" + Recipe.newRecipe.createItem.name + "'.", "recipe");
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
                else if (type == typeof(CraftReq))
                {
                    switch ((CraftReq)recipe[i])
                    {
                        case CraftReq.Water:
                            Recipe.newRecipe.needWater = true;
                            break;
                        case CraftReq.Lava:
                            Recipe.newRecipe.needLava = true;
                            break;
                        case CraftReq.Honey:
                            Recipe.newRecipe.needHoney = true;
                            break;
                        case CraftReq.Tile:
                            // Temporarilly tiles will be defined by CraftReq.Tile
                            if (i == recipe.Length - 1 || recipe[i + 1].GetType() != typeof(int))
                            {
                                throw new ArgumentException("Specifying '" + CraftReq.Tile.ToString() + "' requires a tile type num to be specified subsequentially as an integer (until TileDef/TileRef support is added to Prism).", "recipe");
                            }
                            else if (tileNum >= Recipe.maxRequirements - 1)
                            {
                                throw new ArgumentException("Exceeded the hardcoded maximum number of tile requirements while constructing recipe for '" + Recipe.newRecipe.createItem.name + "'.", "recipe");
                            }

                            Recipe.newRecipe.requiredTile[tileNum] = (ushort)recipe[++i];

                            tileNum++;
                            break;
                    }
                }
                else
                {
                    throw new ArgumentException("Encountered invalid parameter of type '"+ type.ToString() + "' with value '"+ recipe[i].ToString() + "' while constructing recipe for '" + Recipe.newRecipe.createItem.name + "' at index " + i + " of the parameters specified.", "recipe");
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
