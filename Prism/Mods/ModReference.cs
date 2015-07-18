using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using IOPath = System.IO.Path;

namespace Prism.Mods
{
    public interface IReference
    {
        string Name
        {
            get;
        }
        string Path
        {
            get;
        }

        Assembly LoadAssembly();
    }

    public struct AssemblyReference : IReference
    {
        public string Name
        {
            get
            {
                return IOPath.GetFileNameWithoutExtension(Path);
            }
        }
        public string Path
        {
            get;
            private set;
        }

        public AssemblyReference(string path)
        {
            Path = path;
        }

        public Assembly LoadAssembly()
        {
            return Assembly.LoadFrom(Path);
        }
    }
    public struct ModReference : IReference
    {
        public string Name
        {
            get;
            private set;
        }
        public string Path
        {
            get
            {
                return PrismApi.ModDirectory + "\\" + Name;
            }
        }

        public ModReference(string name)
        {
            Name = name;
        }

        public Assembly LoadAssembly()
        {
            // load if the mod hasn't been loaded already
            string iName = Name;
            if (!ModData.mods.Keys.Any(mi => mi.InternalName == iName))
                ModLoader.LoadMod(Path);

            return ModData.Mods.First(kvp => kvp.Key.InternalName == iName).Value.Assembly;
        }
    }
}
