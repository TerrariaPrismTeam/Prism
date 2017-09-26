using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Util;
using Terraria.ID;

namespace Prism.API.Defs
{
    public struct AmmoGroup
    {
        internal readonly static Dictionary<string, AmmoGroup> groups = new Dictionary<string, AmmoGroup>();

        internal ItemRef icon;

        internal AmmoGroup(ItemRef icon)
        {
            this.icon = icon;
        }
        internal AmmoGroup(int ammoType)
            : this(ammoType == 0 ? null : new ItemRef(ammoType))
        {

        }

        /// <summary>naming convention is lowercase, eg. "arrow", "fallenstar"</summary>
        public static AmmoGroup GetOrMake(string name, ItemRef icon = null)
        {
            AmmoGroup r;
            if (groups.TryGetValue(name, out r))
                return r;

            if (icon == null)
                throw new ArgumentNullException("icon");

            r = new AmmoGroup(icon);
            groups.Add(name, r);
            return r;
        }

        internal static void Reset()
        {
            groups.Clear();
        }
        internal static void FillVanilla()
        {
            foreach (var fi in typeof(AmmoID).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (fi.Name == "Count" || !fi.FieldType.IsPrimitive)
                    continue;

                int v = (int)Convert.ChangeType(fi.GetValue(null), typeof(int));

                if (v <= 0)
                    continue;

                groups.Add(fi.Name.ToLowerInvariant(), new AmmoGroup(v));
            }

            // AmmoID isn't complete >__>
            groups.Add("bone", new AmmoGroup(ItemID.Bone));
            groups.Add("ale" , new AmmoGroup(ItemID.Ale ));

            // "nailfriendly" is stupid
            groups.Add("nail", new AmmoGroup(AmmoID.NailFriendly));
        }

        // TODO: equality etc.

        public readonly static AmmoGroup None = new AmmoGroup(null);

        public static AmmoGroup Arrow        { get { return new AmmoGroup(AmmoID.Arrow       ); } }
        public static AmmoGroup Bullet       { get { return new AmmoGroup(AmmoID.Bullet      ); } }
        public static AmmoGroup CandyCorn    { get { return new AmmoGroup(AmmoID.CandyCorn   ); } }
        public static AmmoGroup Coin         { get { return new AmmoGroup(AmmoID.Coin        ); } }
        public static AmmoGroup Dart         { get { return new AmmoGroup(AmmoID.Dart        ); } }
        public static AmmoGroup FallenStar   { get { return new AmmoGroup(AmmoID.FallenStar  ); } }
        public static AmmoGroup Flare        { get { return new AmmoGroup(AmmoID.Flare       ); } }
        public static AmmoGroup Gel          { get { return new AmmoGroup(AmmoID.Gel         ); } }
        public static AmmoGroup JackOLantern { get { return new AmmoGroup(AmmoID.JackOLantern); } }
        public static AmmoGroup Nail         { get { return new AmmoGroup(AmmoID.NailFriendly); } }
        public static AmmoGroup Rocket       { get { return new AmmoGroup(AmmoID.Rocket      ); } }
        public static AmmoGroup Sand         { get { return new AmmoGroup(AmmoID.Sand        ); } }
        public static AmmoGroup Snowball     { get { return new AmmoGroup(AmmoID.Snowball    ); } }
        public static AmmoGroup Solution     { get { return new AmmoGroup(AmmoID.Solution    ); } }
        public static AmmoGroup Stake        { get { return new AmmoGroup(AmmoID.Stake       ); } }
        public static AmmoGroup StyngerBolt  { get { return new AmmoGroup(AmmoID.StyngerBolt ); } }
        public static AmmoGroup Bone         { get { return new AmmoGroup(ItemID.Bone        ); } }
        public static AmmoGroup Ale          { get { return new AmmoGroup(ItemID.Ale         ); } }
    }
}

