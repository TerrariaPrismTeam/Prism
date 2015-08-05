using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LitJson;
using Prism.API;
using Prism.Mods.Hooks;
using Prism.Mods.Resources;
using System.Text;
using Prism.WinFormDialogs;

namespace Prism.Mods
{
    /// <summary>
    /// Handles the loading of mods.
    /// </summary>
    static class ModLoader
    {
        internal static string DebugModDir = null;

        internal static List<LoaderError> errors = new List<LoaderError>();
        static List<string> circRefList = new List<string>();

        internal static void Debug_ShowAllErrors()
        {
#if DEV_BUILD
            if (errors.Count > 0)
            {
                StringBuilder errorDump = new StringBuilder(string.Format("Encountered {0} errors while loading mods:\n\n\n", errors.Count));

                for (int i = 0; i < errors.Count; i++)
                {
                    errorDump.AppendLine(string.Format("[{0} of {1}] ", i + 1, errors.Count) + errors[i].ToString());
                }

                var dialog = new ExceptionDialogForm(string.Format("{0} Error(s)", errors.Count), errorDump.ToString());
                dialog.ShowDialog();
            }
#endif
        }

        /// <summary>
        /// Loads and returns the mod info from the specified path and adds any errors encountered to the internal <see cref="errors"/> list.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <returns><see cref="ModInfo"/> if loaded successfully, null if failed to load</returns>
        static ModInfo? ModInfoFromModPath(string path)
        {
            var fn = path + Path.DirectorySeparatorChar + PrismApi.JsonManifestFileName;

            if (!Directory.Exists(Path.GetDirectoryName(fn)) || !File.Exists(fn))
            {
                errors.Add(new LoaderError(path, "Mod directory not found"));
                return null;
            }

            return ModData.ParseModInfo(JsonMapper.ToObject(File.ReadAllText(fn)), path);
        }

        /// <summary>
        /// Gets a <see cref="ModInfo"/> object from the mod's internal name.
        /// </summary>
        /// <param name="internalName">The internal name of the mod info.</param>
        /// <returns>The ModInfo object whose internal name is that of internalName if able to locate it. Null if unable to locate it.</returns>
        static ModInfo? ModInfoFromInternalName(string internalName)
        {
            return Directory.EnumerateDirectories(PrismApi.ModDirectory)
                .Select(ModInfoFromModPath)
                .Where(mi => mi.HasValue && mi.Value.InternalName == internalName)
                .FirstOrDefault();
        }

        /// <summary>
        /// Recursively checks and returns whether a mod has a circular mod reference and outputs the name of the circular reference if applicable.
        /// </summary>
        /// <param name="info">The <see cref="ModInfo"/> of the mod to check</param>
        /// <param name="evilMod">Outputs the <see cref="ModReference.Name"/> causing the circular references if applicable.</param>
        /// <returns>True if circular referencing is found. False if not.</returns>
        static bool CheckForCircularReferences(ModInfo info, out string evilMod)
        {
            evilMod = String.Empty;

            foreach (string refIName in info.References.OfType<ModReference>().Select(mr => mr.Name))
            {
                if (circRefList.Contains(refIName))
                {
                    evilMod = refIName;
                    return true;
                }

                List<string> temp = new List<string>(circRefList);

                circRefList.Add(refIName);

                var info_ = ModInfoFromInternalName(refIName);
                if (!info_.HasValue)
                {
                    errors.Add(new LoaderError(info, "Mod reference could not be found", refIName));
                    return false;
                }

                if (CheckForCircularReferences(info_.Value, out evilMod))
                    return true;

                circRefList = temp;
            }

            return false;
        }

        /// <summary>
        /// Loads a mod from an <see cref="System.Reflection.Assembly"/> and returns its <see cref="ModDef"/>.
        /// </summary>
        /// <param name="asm">The mod's <see cref="System.Reflection.Assembly"/></param>
        /// <param name="info">The mod's <see cref="ModInfo"/></param>
        /// <returns>The <see cref="ModDef"/> of the mod</returns>
        static ModDef LoadModFromAssembly(Assembly asm, ModInfo info)
        {
            var mdType = asm.GetType(info.ModDefTypeName, false);

            if (mdType == null)
            {
                errors.Add(new LoaderError(info, "ModDef type not found", info.ModDefTypeName));
                return null;
            }

            ModDef mod = Activator.CreateInstance(mdType) as ModDef;

            if (mod == null)
            {
                errors.Add(new LoaderError(info, "Could not instantiate ModDef implementation", mdType));
                return null;
            }

            mod.Assembly = asm ;
            mod.Info     = info;

            // required by the entity def loader
            ModData.mods                .Add(mod.Info             , mod);
            ModData.modsFromInternalName.Add(mod.Info.InternalName, mod);

            errors.AddRange(ResourceLoader .Load(mod));
            errors.AddRange(EntityDefLoader.Load(mod));

            return mod;
        }

        /// <summary>
        /// Loads a mod from the specified path and returns its <see cref="ModDef"/>.
        /// </summary>
        /// <param name="path">The specified path</param>
        /// <returns>The <see cref="ModDef"/> of the mod or null if something went wrong</returns>
        internal static ModDef LoadMod(string path)
        {
            var info_n = ModInfoFromModPath(path);
            if (!info_n.HasValue) // does not exist (or is a debugged mod)
                return null;

            var info = info_n.Value;

            if (ModData.mods.ContainsKey(info)) // mod already loaded when resolving dependencies (see ~15 lines from here)
                return ModData.mods[info];

            circRefList.Clear();
            string evilMod;
            if (CheckForCircularReferences(info, out evilMod))
            {
                errors.Add(new LoaderError(info, "This mod is part of a cyclic reference chain", evilMod));
                return null;
            }

            for (int i = 0; i < info.References.Length; i++)
            {
                if (info.References[i] is ModReference)
                    LoadMod(info.References[i].Path);
                else
                    info.References[i].LoadAssembly();
            }

            Assembly asm = null;
            try
            {
                var curn = Assembly.GetExecutingAssembly().GetName();
                var p = path + Path.DirectorySeparatorChar + info.AssemblyFileName;
                var rasm = Assembly.ReflectionOnlyLoadFrom(p);		
                var refs = rasm.GetReferencedAssemblies();                
                var refn = refs.FirstOrDefault(an => an.Name == curn.Name);

                var loadAssembly = true;

                if (refn == null)
                {
                    errors.Add(new LoaderError(info, "Mod does not reference Prism."));
                    loadAssembly = false;
                }

                if (refn.Version.ToString() == AssemblyInfo.DEV_BUILD_VERSION)
                {
#if !DEV_BUILD
                    errors.Add(new LoaderError(info, "Mod was built with an unstable developer build of Prism for testing purposes and can only be loaded by builds in the master branch of the Prism repository.")); //Make it sound complicated lmao
                    loadAssembly = false;
#endif
                }

                if (refn.Version < curn.Version)
                {
#if DEV_BUILD
                    errors.Add(new LoaderError(info, "Mod was built with a release build of Prism and may not work correctly with the latest developer build."));
#else
                    errors.Add(new LoaderError(info, "Mod was built with a previous version of Prism and may not work correctly."));
#endif
                }

                if (refn.Version > curn.Version)
                {
                    errors.Add(new LoaderError(info, "Mod was built with a newer version of Prism than the installed version. Update to the latest version of Prism in order to load this mod."));
                    loadAssembly = false;
                }

                asm = Assembly.LoadFrom(p);
            }
            catch (Exception e)
            {
                errors.Add(new LoaderError(info, "Could not load mod assembly", e));
                return null;
            }          

            return LoadModFromAssembly(asm, info);
        }

        /// <summary>
        /// Loads all mods from the Prism mod directory and returns a list containing any loader errors encountered.
        /// </summary>
        /// <returns>Any <see cref="LoaderError"/>'s encountered while loading.</returns>
        internal static IEnumerable<LoaderError> Load()
        {
            errors.Clear();

            if (!Directory.Exists(PrismApi.ModDirectory))
                Directory.CreateDirectory(PrismApi.ModDirectory);

            ResourceLoader.Setup();

            var dirs = Directory.EnumerateDirectories(PrismApi.ModDirectory);

            // The mod .dll in the DebugModDir should be one in the output directory of the Visual Studio mod project.
            // Because the VS debugger knows which .dll it should use (and has a .pdb file), Edit & Continue and other
            // hackery should be automatically activated.
            foreach (string s in DebugModDir == null ? dirs : new[] { DebugModDir }.Concat(dirs))
            {
                var prevModCount = ModData.mods.Count;
                var d = LoadMod(s);
                var newModCount = ModData.mods.Count;

                if (newModCount > prevModCount) //Make sure a new mod was actually added to ModData.mods so we don't get a "Sequence is empty" error on ModData.mods.Last()
                {
                    if (d == null)
                    {
                        var i = ModData.mods.Last().Key;

                        ModData.mods                .Remove(i);
                        ModData.modsFromInternalName.Remove(i.InternalName);
                    }
                    if (d != null)
                    {
                        //ModData.mods                .Add(d.Info             , d);
                        //ModData.modsFromInternalName.Add(d.Info.InternalName, d);

                        try
                        {
                            d.OnLoad();
                        }
                        catch (Exception e)
                        {
                            errors.Add(new LoaderError(d.Info, "An exception occured in ModDef.OnLoad()", e));

                            // Temporary until we have a proper way to see loader errors
                            if (ExceptionHandler.DetailedExceptions)
                                MessageBox.Show("An exception has occured:\n" + e, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                    errors.Add(new LoaderError(s, String.Format("New entry was not added to ModData.mods while loading mod from path '{0}': Something went wrong in ModLoader.LoadMod()", s), ModData.mods));
            }

            HookManager.Create();
            HookManager.CanCallHooks = true;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            HookManager.ModDef.OnAllModsLoaded();

            return errors;
        }

        /// <summary>
        /// Unloads all loaded mods.
        /// </summary>
        internal static void Unload()
        {
            if (HookManager.ModDef != null)
            {
                HookManager.ModDef.OnUnload();
                // other hookmgrs here
            }

            HookManager.CanCallHooks = false;

            HookManager.Clear();

            foreach (var v in ModData.mods.Values)
                v.Unload();

            EntityDefLoader.ResetEntityHandlers ();
            ResourceLoader .Unload();

            ModData.mods.Clear();

            errors.Clear();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Just calls <see cref="Unload"/> then <see cref="Load"/>
        /// </summary>
        /// <returns>The result of <see cref="Load"/> (Any <see cref="LoaderError"/>'s encountered while loading)</returns>
        internal static IEnumerable<LoaderError> Reload()
        {
            Unload();

            return Load();
        }
    }
}
