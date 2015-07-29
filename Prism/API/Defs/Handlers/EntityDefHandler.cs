using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;
using Prism.Mods;

namespace Prism.Defs.Handlers
{
    /// <summary>
    /// Handles all the entity defs.
    /// </summary>
    public abstract class EntityDefHandler<TEntityDef, TBehaviour, TEntity>
        where TEntity : class
        where TBehaviour : EntityBehaviour<TEntity>
        where TEntityDef : EntityDef<TBehaviour, TEntity>, new()
    {
        /// <summary>
        /// The entity type index to which the next entity added will be assigned;
        /// </summary>
        public int NextTypeIndex;       

        public abstract void ExtendVanillaArrays(int amt = 1);
        public abstract TEntity GetVanillaEntityFromID(int id);
        public abstract void CopyEntityToDef(TEntity entity, TEntityDef def);
        public abstract void CopyDefToEntity(TEntityDef def, TEntity entity);        
        public abstract bool CheckTextures(TEntityDef def);
        public abstract void LoadTextures(ref List<LoaderError> err, TEntityDef def);

        public Dictionary<int, TEntityDef> DefsByType = new Dictionary<int, TEntityDef>();
        public Dictionary<string, TEntityDef> VanillaDefsByName = new Dictionary<string, TEntityDef>();

        /// <summary>
        /// Gets the <see cref="System.Type"/> of entity this handler handles.
        /// </summary>
        public Type EntityType
        {
            get
            {
                return typeof(TEntity);
            }
        }       

        public Vanilla.EntityConstData VanillaData
        {
            get
            {
                return Vanilla.Data.Entity[typeof(TEntity)];
            }
        }

        public EntityDefHandler()
        {
            NextTypeIndex = VanillaData.MaxID;
        }
                
        public void FillVanillaDefs()
        {
            for (int id = VanillaData.MinID; id < VanillaData.MaxID; id++)
            {
                TEntity entity = GetVanillaEntityFromID(id);
                TEntityDef def = new TEntityDef();
                CopyEntityToDef(entity, def);
                def.InternalName = Vanilla.Data.Entity[typeof(TEntity)].Name[(short)id];

                DefsByType.Add(id, def);                
                VanillaDefsByName.Add(def.InternalName, def);
            }

            PostFillVanilla();
        }

        public virtual void PostFillVanilla() { }

        public void ResetAll()
        {
            //Rewind the type index back to the end of the vanilla type indeces.
            NextTypeIndex = VanillaData.MaxID;

            DefsByType = (
                //Select the by-type dictionary's keys which are the IDs of the defs...
                from id in DefsByType.Keys 
                //...but only select the vanilla IDs (meaning all of the modded keys are excluded, effectively deleting them)
                where (id < VanillaData.MaxID)
                //..put them into new KeyValuePairs 
                select new KeyValuePair<int, TEntityDef>(id, DefsByType[id])
            )
            .ToDictionary(x => x.Key, x => x.Value); 
            /*
                Fancy query things leave us with a very basic/generic IEnumerable<KeyValuePair<TKey, TValue>>, 
                meaning it is a essentially a list of dictionary entries. However, we don't want a list of the
                entries the dictionary has in it; we want the actual dictionary :p
            */
        }        
        
        public IEnumerable<LoaderError> LoadAll(Dictionary<string, TEntityDef> dict)
        {
            var err = new List<LoaderError>();

            ExtendVanillaArrays(dict.Count);

            foreach (var def in dict.Values)
            {
                if (!CheckTextures(def) && !Main.dedServ)
                    err.Add(new LoaderError(def.Mod, "Textures failed to load."));
                else
                {
                    def.Type = NextTypeIndex;
                    DefsByType.Add(NextTypeIndex, def);

                    if (!Main.dedServ)
                        LoadTextures(ref err, def);

                    NextTypeIndex++;
                }
            }

            return err;
        }
    }
}
