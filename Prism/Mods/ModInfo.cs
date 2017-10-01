using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Prism.API;
using Prism.Util;

namespace Prism.Mods
{
    /// <summary>
    /// Container which holds the information associated with a mod definition.
    /// </summary>
    public struct ModInfo : IEquatable<ModInfo>
    {
        Version verAsVer;

        public static readonly ModInfo Empty = new ModInfo("_", String.Empty, String.Empty, String.Empty, "0.0.0.0", String.Empty, "_", String.Empty, Empty<IReference>.Array);

        /// <summary>
        /// Gets the path associated with this mod definition.
        /// </summary>
        public readonly string ModPath;

        /// <summary>
        /// Gets the internal name associated with this mod definition by which it is referenced from any mod.
        /// </summary>
        public readonly string InternalName;

        /// <summary>
        /// Gets the display name associated with this mod definition.
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// Gets the author associated with this mod definition.
        /// </summary>
        public readonly string Author;

        /// <summary>
        /// Gets the version string associated with this mod definition.
        /// </summary>
        public readonly string Version;

        /// <summary>
        /// Gets the description associated with this mod definition.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Gets the file name of the <see cref="System.Reflection.Assembly"/> associated with this mod definition.
        /// </summary>
        public readonly string AssemblyFileName;

        /// <summary>
        /// Gets the type name associated with this mod definition.
        /// </summary>
        public readonly string ModDefTypeName;

        /// <summary>
        /// Gets the references to the <see cref="System.Reflection.Assembly"/>'s (including other mods) associated with this mod definition.
        /// </summary>
        public readonly IReference[] References;

        /// <summary>
        /// Gets the mod's version as a <see cref="System.Version"/> object.
        /// </summary>
        public Version VersionAsVersion
        {
            get
            {
                return verAsVer ?? (verAsVer = ParseVer(Version));
            }
        }

        /// <summary>
        /// Constructs a new <see cref="ModInfo"/> object.
        /// </summary>
        /// <param name="modPath"><see cref="ModPath"/></param>
        /// <param name="internalName"><see cref="InternalName"/></param>
        /// <param name="displayName"><see cref="DisplayName"/></param>
        /// <param name="author"><see cref="Author"/></param>
        /// <param name="version"><see cref="Version"/></param>
        /// <param name="descr"><see cref="Description"/></param>
        /// <param name="asmFileName"><see cref="AssemblyFileName"/></param>
        /// <param name="modDefTypeName"><see cref="ModDefTypeName"/></param>
        /// <param name="references"><see cref="References"/></param>
        public ModInfo(string modPath, string internalName, string displayName, string author, string version, string descr, string asmFileName, string modDefTypeName, IReference[] references)
        {
            verAsVer = null;

            ModPath = modPath;

            InternalName = internalName;
            DisplayName = displayName;
            Author = author;
            Version = version;
            Description = String.IsNullOrEmpty(descr) ? displayName + " by " + author + " v" + version : descr;

            AssemblyFileName = asmFileName;
            ModDefTypeName = modDefTypeName;
            References = references;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (obj is ModInfo)
                return Equals((ModInfo)obj);

            return false;
        }

        /// <summary>
        /// Gets the hashcode of this <see cref="ModInfo"/>, based on its fields.
        /// </summary>
        /// <returns>The combined hashcodes of the <see cref="InternalName"/>, <see cref="DisplayName"/>, <see cref="Author"/>, and <see cref="Version"/> of this this <see cref="ModInfo"/>.</returns>
        public override int GetHashCode()
        {
            return InternalName.GetHashCode() + DisplayName.GetHashCode() + Author.GetHashCode() + Version.GetHashCode();
        }

        /// <summary>
        /// Gets the basic string representation of this <see cref="ModInfo"/> object.
        /// </summary>
        /// <returns>"{ <see cref="InternalName"/> }"</returns>
        public override string ToString()
        {
            return "{" + InternalName + "}";
        }

        /// <summary>
        /// Gets the pretty string representation of this <see cref="ModInfo"/> object.
        /// </summary>
        /// <returns>Display Name by Author vX.X.X.X</returns>
        public string ToPrettyString()
        {
            return DisplayName + " by " + Author + " v" + Version;
        }

        /// <summary>
        /// Checks the equality of <see cref="ModInfo"/> objects. You can also simply use the '==' and '!=' operators.
        /// </summary>
        /// <param name="other">The other <see cref="ModInfo"/> to compare against.</param>
        /// <returns>True if the <see cref="ModInfo"/>'s have the same <see cref="InternalName"/>, <see cref="DisplayName"/>, <see cref="Author"/>, and <see cref="Version"/></returns>
        public bool Equals(ModInfo other)
        {
            return InternalName == other.InternalName && DisplayName == other.DisplayName && Author == other.Author && Version == other.Version /* ? */;
        }

        public static bool operator ==(ModInfo a, ModInfo b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(ModInfo a, ModInfo b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Gets the <see cref="ModDef" /> of the mod info.
        /// </summary>
        /// <returns>The mod definition of the mod. Null if the ModInfo is vanilla or not found.</returns>
        public ModDef GetDefinition()
        {
            ModDef r;
            if (this == PrismApi.VanillaInfo || !ModData.mods.TryGetValue(this, out r))
                return null;

            return r;
        }

        internal static Version ParseVer(string s)
        {
            Version ret = null;

            if (String.IsNullOrWhiteSpace(s))
                ret = EmptyClass<Version>.Default;
            else if (s[0] == 'r' || s[0] == 'R' || s[0] == 'v' || s[0] == 'V')
            {
                int endIndex = 1;
                for (endIndex = 1; endIndex < s.Length; endIndex++)
                    if (!Char.IsDigit(s[endIndex]))
                        break;

                ret = new Version(Int32.Parse(s.Substring(1, endIndex), CultureInfo.InvariantCulture), 0);
            }
            else
                try
                {
                    var v = s;
                    var revAdd = 0;

                    if (Char.IsLetter(Char.ToUpperInvariant(v[v.Length - 1])))
                    {
                        revAdd = v[v.Length - 1] - 'a' + 1; // a -> 1, instead of 0
                        v = v.Substring(0, v.Length - 1);
                    }

                    ret = new Version(v);

                    if (revAdd != 0)
                        ret = new Version(ret.Major, ret.Minor, ret.Build, ret.Revision + revAdd);
                }
                catch
                {
                    ret = EmptyClass<Version>.Default;
                }

            return ret;
        }
    }
}
