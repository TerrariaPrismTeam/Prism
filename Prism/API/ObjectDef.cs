using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Util;

namespace Prism.API
{
    public abstract class ObjectDef
    {
        /// <summary>
        /// Gets the internal name used to reference this object from within any Prism mod.
        /// </summary>
        public string InternalName
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets Information about the mod to which this entiobjectty belongs.
        /// </summary>
        public ModInfo Mod
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the internal type index of this object.
        /// </summary>
        /// <remarks>Only use this after all mods are loaded.</remarks>
        public int Type
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the name of the object which will show up in-game.
        /// <para/>
        /// E.g.: An item's name in inventory, an NPC's name displayed on mouse hover, an NPC's name displayed in player death messages, etc.
        /// <para/>
        /// Note: Although there exists one way to see a Projectile's name (in player death messages), they don't have a display name property in the vanilla game
        /// </summary>
        public ObjectName DisplayName
        {
            get;
            set;
        }

        public bool IsVanilla
        {
            get
            {
                return Mod == null || Mod == PrismApi.VanillaInfo;
            }
        }

        protected ObjectDef(ObjectName displayName)
        {
            InternalName = String.Empty;

            DisplayName  = displayName;
        }

        public override string ToString()
        {
            return "{" + (String.IsNullOrEmpty(InternalName) ? DisplayName.CultureInvariantString : InternalName) + ", Mod=" + Mod + "}";
        }

        public static implicit operator ObjectRef(ObjectDef d)
        {
            return new ObjectRef(d.InternalName, d.Mod);
        }

    }
    public abstract class ObjectDef<TBehaviour> : ObjectDef
    {
        /// <summary>
        /// Gets or sets the parameterless constructor that instantiates the matching EntityBehaviour class of the EntityRef.
        /// </summary>
        public Func<TBehaviour> CreateBehaviour
        {
            get;
            set;
        }

        protected ObjectDef(ObjectName displayName, Func<TBehaviour> newBehaviour = null)
            : base(displayName)
        {
            CreateBehaviour = newBehaviour ?? Empty<TBehaviour>.Func;
        }
    }
}
