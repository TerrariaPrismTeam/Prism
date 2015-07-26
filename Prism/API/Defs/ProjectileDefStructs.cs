using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Prism.API.Defs
{
    /// <summary>
    /// Vanilla Projectile AI Styles
    /// </summary>
    public enum ProjectileAiStyle
    {
        None = 0
        //There are 124 values for this...
    }

    //public enum ProjFlags
    //{
    //    Counterweight,
    //    Arrow,
    //    Bobber,
    //    Trap
    //}

    public enum ProjectileDamageType
    {
        None,
        Melee,
        Ranged,
        Magic
    }

    /// <summary>
    /// ?TrailingMode?
    /// </summary>
    public enum TrailingMode
    {

        None = -1,
        /// <summary>
        /// ?TrailingMode0?
        /// </summary>
        TrailingMode_0 = 0,
        /// <summary>
        /// ?TrailingMode1?
        /// </summary>
        TrailingMode_1 = 1,
        /// <summary>
        /// ?TrailingMode2?
        /// </summary>
        TrailingMode_2 = 2
    }
}
