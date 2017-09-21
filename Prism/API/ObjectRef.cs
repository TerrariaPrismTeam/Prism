using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Mods;

namespace Prism.API
{
    public struct ObjectRef : IEquatable<ObjectRef>
    {
        string name, modName;
        internal ModDef requesting;

        public readonly static ObjectRef Null = new ObjectRef()
        {
            name       = String.Empty,
            modName    = String.Empty,
            requesting = ModData.ModFromAssembly(Assembly.GetExecutingAssembly())
        };

        public string Name
        {
            get
            {
                return name;
            }
        }
        public string ModName
        {
            get
            {
                return modName;
            }
        }

        public ModInfo Mod
        {
            get
            {
                if (String.IsNullOrEmpty(modName) || modName == PrismApi.TerrariaString || modName == PrismApi.VanillaString)
                    return PrismApi.VanillaInfo;

                if (ModData.ModsFromInternalName.ContainsKey(modName))
                    return ModData.ModsFromInternalName[modName].Info;

                throw new InvalidOperationException("Mod  " + modName + " not loaded.");
            }
        }

        public bool IsNull
        {
            get
            {
                return String.IsNullOrEmpty(name);
            }
        }

        internal ObjectRef(string name, string modName, Assembly requesting)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.modName = modName ?? String.Empty;

            this.requesting = ModData.ModFromAssembly(requesting);
        }
        public ObjectRef(string name, string modName = null)
            : this(name, modName, Assembly.GetCallingAssembly())
        {

        }
        public ObjectRef(string name, ModInfo mod)
            : this(name, mod.InternalName)
        {
            requesting = null;
        }

        public bool Equals(ObjectRef other)
        {
            return name == other.name && modName == other.modName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is ObjectRef)
                return Equals((ObjectRef)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Mod.GetHashCode();
        }
        public override string ToString()
        {
            if (String.IsNullOrEmpty(modName) || modName == PrismApi.TerrariaString || modName == PrismApi.VanillaString)
                return "{" + name + "}";

            if (ModData.ModsFromInternalName.ContainsKey(modName))
                return "{" + Mod + "." + name + "}";

            return "{" + modName + "." + name + "}";
        }

        public static bool operator ==(ObjectRef a, ObjectRef b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(ObjectRef a, ObjectRef b)
        {
            return !a.Equals(b);
        }
    }
}
