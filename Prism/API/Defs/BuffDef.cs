using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Prism.API.Behaviours;
using Prism.Util;

namespace Prism.API.Defs
{
    public partial class BuffDef : ObjectDef<BuffBehaviour>
    {
        /// <summary>
        /// Gets or sets whether this buff is a debuff.
        /// </summary>
        public bool IsDebuff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text on this buff's tooltip.
        /// </summary>
        public string Tooltip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this buff works against other players. Set to false to prevent debuff cheesing ;)
        /// </summary>
        public bool WorksInPvP
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this buff remains after dying with it.
        /// </summary>
        public bool PersistsAfterDeath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether this buff counts as a weapon imbuement. If so, Quick Buff won't use it and it will replace other imbuements upon application.
        /// </summary>
        public bool IsWeaponImbuement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this buffs saves with your character file.
        /// </summary>
        public bool DoesNotSave
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether the buff's remaining time is hidden.
        /// </summary>
        public bool HideTimeDisplay
        {
            get;
            set;
        }

        public bool IsVanityPet
        {
            get;
            set;
        }
        public bool IsLightPet
        {
            get;
            set;
        }

        public Func<Texture2D> GetTexture
        {
            get;
            set;
        }

        public BuffDef(string displayName, Func<BuffBehaviour> newBehaviour = null, Func<Texture2D> getTexture = null)
            : base(displayName, newBehaviour)
        {
            GetTexture = getTexture ?? Empty<Texture2D>.Func;

            Tooltip = String.Empty;
        }

        public static implicit operator BuffRef(BuffDef def)
        {
            return new BuffRef(def.InternalName, def.Mod);
        }
        public static explicit operator BuffDef(BuffRef r)
        {
            return r.Resolve();
        }
    }
}
