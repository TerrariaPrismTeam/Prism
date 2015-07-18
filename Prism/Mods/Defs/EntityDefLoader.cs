using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Terraria;
using Terraria.ID;

namespace Prism.Mods.Defs
{
    static class EntityDefLoader
    {
        /// <summary>
        /// Sets the "ROProperties"...(what the fuck does this even mean poro y u do dis)
        /// </summary>
        /// <typeparam name="TEntityDef"></typeparam>
        /// <param name="def"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        static Dictionary<string, TEntityDef> SetROProperties<TEntityDef>(ModDef def, Dictionary<string, TEntityDef> dict)
            where TEntityDef : EntityDef
        {
            foreach (var kvp in dict)
            {
                kvp.Value.InternalName = kvp.Key;
                kvp.Value.Mod = def.Info;
            }

            return dict;
        }

        /// <summary>
        /// Resets all the item/NPC/tile/projectile/etc def handlers.
        /// </summary>
        internal static void Reset()
        {
            ItemDefHandler.Reset();
        }
		
        /// <summary>
        /// Sets up this EntityDefLoader for loading mods, creating/adding all of the vanilla content defs.
        /// </summary>
        internal static void Setup()
        {
            ItemDefHandler.FillVanilla();
        }

        /// <summary>
        /// Loads a mod and returns all <see cref="LoaderError"/>'s encountered.
        /// </summary>
        /// <param name="mod">The mod to load.</param>
        /// <returns>Enumerable list of LoaderErrors encountered while loading the mod.</returns>
        internal static IEnumerable<LoaderError> Load(ModDef mod)
        {
            mod.ItemDefs = SetROProperties(mod, mod.GetItemDefsI());

            // validate props

            ItemDefHandler.Load(mod.ItemDefs);

            return new List<LoaderError>();
        }
    }
}
