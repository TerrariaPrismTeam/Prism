using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Prism.Util;
using Microsoft.Xna.Framework.Graphics;

namespace Prism.API.Defs
{
    public class BuffDef
    {
        /// <summary>
        /// The internal name used to reference this buff.
        /// </summary>
        public string InternalName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the name displayed on the buff's tooltip.
        /// </summary>
        public string DisplayName
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

        public float Alpha
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

        public ModInfo Mod
        {
            get;
            internal set;
        }

        public BuffDef(string displayName, Func<Texture2D> getTexture = null)
        {
            DisplayName = displayName;
            GetTexture = getTexture ?? Empty<Texture2D>.Func;
            WorksInPvP = false;
            PersistsAfterDeath = false;
            IsWeaponImbuement = false;
            IsDebuff = false;
            Tooltip = default(string);
            DoesNotSave = false;
            HideTimeDisplay = false;
            Alpha = 1.0f;
            IsVanityPet = false;
            IsLightPet = false;
        }
    }
}
