using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using IOPath = System.IO.Path;

namespace Prism.Mods
{
    /// <summary>
    /// Interface with which mods can reference assemblies/other mods.
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// Gets the local name of the reference.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the path to the referenced file.
        /// </summary>
        string Path
        {
            get;
        }

        /// <summary>
        /// Loads and returns the <see cref="System.Reflection.Assembly"/> associated with this <see cref="ModReference"/>.
        /// </summary>
        /// <returns>The <see cref="System.Reflection.Assembly"/></returns>
        Assembly LoadAssembly();
    }

    /// <summary>
    /// A reference to any <see cref="System.Reflection.Assembly"/>.
    /// </summary>
    public struct AssemblyReference : IReference
    {
        string path;

        /// <summary>
        /// Gets the name of the <see cref="System.Reflection.Assembly"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return IOPath.GetFileNameWithoutExtension(Path);
            }
        }
        /// <summary>
        /// Gets the path to the <see cref="System.Reflection.Assembly"/> file.
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="System.Reflection.Assembly"/> Reference.
        /// </summary>
        /// <param name="path">The path to the assembly to referencce.</param>
        public AssemblyReference(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Loads and returns the <see cref="System.Reflection.Assembly"/> associated with this <see cref="AssemblyReference"/>.
        /// </summary>
        /// <returns>The <see cref="System.Reflection.Assembly"/></returns>
        public Assembly LoadAssembly()
        {
            return Assembly.LoadFrom(Path);
        }
    }

    /// <summary>
    /// A reference to a Prism mod.
    /// </summary>
    public struct ModReference : IReference
    {
        string name;

        /// <summary>
        /// The name of the mod.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }
        /// <summary>
        /// The path to the mod's files.
        /// </summary>
        public string Path
        {
            get
            {
                return PrismApi.ModDirectory + "\\" + Name;
            }
        }

        /// <summary>
        /// Constructs a new Mod Reference.
        /// </summary>
        /// <param name="name">The name of the mod</param>
        public ModReference(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Loads the mod's <see cref="System.Reflection.Assembly"/> if it hasn't been loaded yet. Otherwise it just finds the already-loaded mod and returns it.
        /// </summary>
        /// <returns>The mod's <see cref="System.Reflection.Assembly"/>.</returns>
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
