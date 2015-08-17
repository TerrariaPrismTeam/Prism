using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;

namespace Prism.API.Defs
{
    public class RecipeItems : Dictionary<ItemRef, int> { }

    [Flags]
    public enum RecipeLiquids
    {
        None  = 0,
        Water = 1,
        Lava  = 2,
        Honey = 4
    }

    public class RecipeDef
    {
        public static IEnumerable<RecipeDef> Recipes
        {
            get
            {
                return Handler.RecipeDef.recipes;
            }
        }

        public ModInfo Mod
        {
            get;
            internal set;
        }

        public ItemRef CreateItem
        {
            get;
            set;
        }
        public int CreateStack
        {
            get;
            set;
        }

        //TODO: add ItemGroups and change ItemRef to an ItemRef | ItemGroup discriminated union
        public IDictionary<ItemRef, int> RequiredItems
        {
            get;
            set;
        }
        //TODO: add TileGroups and change it to another discriminated union
        public TileRef[] RequiredTiles
        {
            get;
            set;
        }

        public RecipeLiquids RequiredLiquids
        {
            get;
            set;
        }

        public RecipeDef(
            #region arguments
            ItemRef createItem,
            int stack = 1,
            IDictionary<ItemRef, int> reqItems = null,
            TileRef[] reqTiles = null,
            RecipeLiquids reqLiquids = RecipeLiquids.None
            #endregion
            )
        {
            CreateItem = createItem;
            CreateStack = stack;

            RequiredItems = reqItems ?? new Dictionary<ItemRef, int>();
            RequiredTiles = reqTiles ?? Empty<TileRef>.Array;

            RequiredLiquids = reqLiquids;
        }
    }
}
