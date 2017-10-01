using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Debugging;

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
        /// Loads and returns the <see cref="Assembly" /> associated with this <see cref="ModReference" />.
        /// </summary>
        /// <returns>The <see cref="Assembly" /></returns>
        Assembly LoadAssembly();
    }

    /// <summary>
    /// A reference to any <see cref="Assembly" />.
    /// </summary>
    public struct AssemblyReference : IReference
    {
        readonly static string REFS = "References";

        string path, modPath;

        /// <summary>
        /// Gets the name of the <see cref="Assembly" />.
        /// </summary>
        public string Name
        {
            get
            {
                return IOPath.GetFileNameWithoutExtension(Path);
            }
        }
        /// <summary>
        /// Gets the path to the <see cref="Assembly" /> file.
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="Assembly" /> Reference.
        /// </summary>
        /// <param name="path">The path to the assembly to reference.</param>
        public AssemblyReference(string path, string modPath)
        {
            this.path    = path   ;
            this.modPath = modPath;
        }

        /// <summary>
        /// Loads and returns the <see cref="Assembly" /> associated with this <see cref="AssemblyReference" />.
        /// </summary>
        /// <returns>The <see cref="Assembly" /></returns>
        public Assembly LoadAssembly()
        {
            var paths = new[] { modPath, IOPath.Combine(modPath, REFS) };

            AssemblyResolver.searchPaths.AddRange(paths);

            try
            {
                return AssemblyResolver.LoadModAssembly(path);
            }
            finally // more 'finally' abuse, values will be removed from the search path
                    // after the method returns, and no temporary variable is needed.
            {
                AssemblyResolver.searchPaths.RemoveAll(e => Array.IndexOf(paths, e) != -1);
            }
        }
    }

    /// <summary>
    /// A reference to a Prism mod.
    /// </summary>
    public struct ModReference : IReference
    {
        string name;
        Version minv;

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

        /// <summary>The minimum version of the mod</summary>
        public Version MinimumVersion
        {
            get
            {
                return minv;
            }
        }

        /// <summary>
        /// Constructs a new Mod Reference.
        /// </summary>
        /// <param name="name">The name of the mod</param>
        public ModReference(string name, Version minVer)
        {
            this.name = name  ;
            this.minv = minVer;
        }

        /// <summary>
        /// Loads the mod's <see cref="Assembly" /> if it hasn't been loaded yet. Otherwise it just finds the already-loaded mod and returns it.
        /// </summary>
        /// <returns>The mod's <see cref="Assembly" />.</returns>
        public Assembly LoadAssembly()
        {
            // load if the mod hasn't been loaded already
            string iName = name; // it's a struct, a copy of 'name' is needed
            if (!ModData.mods.Keys.Any(mi => mi.InternalName == iName))
                ModLoader.LoadMod(Path);

            var m = ModData.Mods.FirstOrDefault(kvp => kvp.Key.InternalName == iName).Value;

            if (m == null)
            {
                Logging.LogError("Mod reference '" + m.Info.DisplayName
                        + "'('" + iName + "') not found!");

                return null;
            }

            if (m.Info.VersionAsVersion < MinimumVersion)
            {
                Logging.LogError("Mod reference '" + m.Info.DisplayName
                        + "'('" + iName + "'): required version is "
                        + MinimumVersion + ", actual version is "
                        + m.Info.Version + ": update the mod!");

                return null;
            }

            return m.Assembly;
        }
    }
}

