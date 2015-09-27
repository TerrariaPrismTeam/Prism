using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;

namespace Prism.API.Defs
{
    using ItemUnion = Either<ItemRef, CraftGroup<ItemDef, ItemRef>>;
    using TileUnion = Either<TileRef, CraftGroup<TileDef, TileRef>>;

    public class RecipeItems : Dictionary<Either<ItemRef, CraftGroup<ItemDef, ItemRef>>, int> { }

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

        public IDictionary<ItemUnion, int> RequiredItems
        {
            get;
            set;
        }
        public IEnumerable<TileUnion> RequiredTiles
        {
            get;
            set;
        }

        public RecipeLiquids RequiredLiquids
        {
            get;
            set;
        }

        public RecipeDef(ItemRef createItem, int stack, IDictionary<ItemUnion, int> reqItems,
                         IEnumerable<TileUnion> reqTiles = null, RecipeLiquids reqLiquids = RecipeLiquids.None)
        {
            CreateItem  = createItem;
            CreateStack = stack;

            RequiredItems = reqItems ?? new Dictionary<ItemUnion, int>();
            RequiredTiles = reqTiles ?? Empty<TileUnion>.Array;

            RequiredLiquids = reqLiquids;
        }
        public RecipeDef(ItemRef createItem, int stack, IDictionary<ItemUnion, int> reqItems,
                         IEnumerable<TileRef> reqTiles, RecipeLiquids reqLiquids = RecipeLiquids.None)
            : this(createItem, stack, reqItems, reqTiles.SafeSelect(r => (TileUnion)r), reqLiquids)
        {

        }
        public RecipeDef(ItemRef createItem, int stack, IDictionary<ItemUnion, int> reqItems,
                         IEnumerable<CraftGroup<TileDef, TileRef>> reqTiles, RecipeLiquids reqLiquids = RecipeLiquids.None)
            : this(createItem, stack, reqItems, reqTiles.SafeSelect(Either<TileRef, CraftGroup<TileDef, TileRef>>.NewLeft), reqLiquids)
        {

        }
    }
}
