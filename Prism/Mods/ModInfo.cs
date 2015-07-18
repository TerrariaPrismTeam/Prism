using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Prism.Mods
{
    public struct ModInfo : IEquatable<ModInfo>
    {
        public string ModPath;

        public string InternalName;
        public string DisplayName;
        public string Author;
        public string Version;
        public string Description;

        public string AssemblyFileName;
        public string ModDefTypeName;
        public IReference[] References;

        public Version VersionAsVersion
        {
            get
            {
                if (Version.StartsWith("r", StringComparison.InvariantCultureIgnoreCase))
                {
                    int endIndex = 1;
                    for (endIndex = 1; endIndex < Version.Length; endIndex++)
                        if (!Char.IsDigit(Version[endIndex]))
                            break;

                    return new Version(Int32.Parse(Version.Substring(1, endIndex), CultureInfo.InvariantCulture), 0);
                }
                try
                {
                    return new Version(Version);
                }
                catch { }

                return new Version();
            }
        }

        public ModInfo(string modPath, string internalName, string displayName, string author, string version, string descr, string asmFileName, string modDefTypeName, IReference[] references)
        {
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
        public override int GetHashCode()
        {
            return InternalName.GetHashCode() + DisplayName.GetHashCode() + Author.GetHashCode() + Version.GetHashCode();
        }
        public override string ToString()
        {
            return "{" + InternalName + "}";
        }

        public string ToPrettyString()
        {
            return DisplayName + " by " + Author + " v" + Version;
        }

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
    }
}
