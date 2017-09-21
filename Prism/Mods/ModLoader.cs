
#if !UNIX && !WINDOWS
// wat?
#define WINDOWS
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LitJson;
using Prism.API;
using Prism.Debugging;
using Prism.Mods.Hooks;
using Prism.Mods.Resources;
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

        public static bool Loading
        {
            get;
            private set;
        }
        public static bool Unloading
        {
            get;
            private set;
        }
        public static bool Reloading
        {
            get
            {
                return Loading || Unloading;
            }
        }

        [Conditional("WINDOWS")] // can only display on windows, because WinForms is borked on other platforms for some mysterious reason
        internal static void ShowAllErrors()
        {
            if (errors.Count > 0)
            {
                var errorDump = new StringBuilder(String.Format("Encountered {0} errors while loading mods:\n\n\n", errors.Count));

                for (int i = 0; i < errors.Count; i++)
                    errorDump.Append(String.Format("[{0} of {1}] ", i + 1, errors.Count)).AppendLine(errors[i].ToString());

                var dialog = new ExceptionDialogForm(String.Format("{0} Error(s)", errors.Count), errorDump.ToString());
                dialog.ShowDialog();
            }
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
                .Select(ModInfoFromModPath).FirstOrDefault(mi => mi.HasValue && mi.Value.InternalName == internalName);
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

                var temp = new List<string>(circRefList);

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

            var mod = Activator.CreateInstance(mdType) as ModDef;

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

            mod.ContentHandler = mod.CreateContentHandlerInternally();
            mod.ContentHandler.Adopt(mod.Info);

            errors.AddRange(ResourceLoader .Load(mod));
            errors.AddRange(ContentLoader  .Load(mod));
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
                errors.Add(new LoaderError(info, "This mod is part of a cyclic reference chain, bad reference " /* ':' is added automatically */ , evilMod));
                return null;
            }

            for (int i = 0; i < info.References.Length; i++)
                if (info.References[i] is ModReference)
                {
                    if (LoadMod(info.References[i].Path) == null)
                        errors.Add(new LoaderError(info, "Could not load mod reference " + info.References[i].Name + "."));
                }
                else if (info.References[i].LoadAssembly() == null)
                    errors.Add(new LoaderError(info, "Could not load assembly reference " + info.References[i].Name + "."));

            Assembly asm = null;
            try
            {
                var terAsm = typeof(Terraria.Main).Assembly;
                var curn = Assembly.GetExecutingAssembly().GetName();
                var p = path + Path.DirectorySeparatorChar + info.AssemblyFileName;
                var rasm = Assembly.ReflectionOnlyLoadFrom(p);
                var refs = rasm.GetReferencedAssemblies();

                /*Logging.LogInfo("prism='" + curn.Name + "', #refs=" + refs.Length);
                foreach (var r in refs)
                    Logging.LogInfo(r.Name);*/

                var refn = refs.FirstOrDefault(an => an.Name == curn.Name);
                var reft = refs.FirstOrDefault(an => an.Name == terAsm.GetName().Name);

                var loadAssembly = true;

                if (refn == null)
                {
                    errors.Add(new LoaderError(info, "Mod does not reference Prism."));
                    loadAssembly = false;
                    return null;
                }

                var refVer = refn.Version;
                var refVers = refVer.ToString();
                var curVer = curn.Version;
              //var curVers = curVer.ToString();
                var terVer = reft == null ? null : reft.Version;

                if (terVer != null && terVer != terAsm.GetName().Version) // null -> version-agnostic (e.g. only using prism internals)
                {
                    errors.Add(new LoaderError(info, "Mod was built for a different version of Terraria."));
                    loadAssembly = false;
                }

                if (refVers == AssemblyInfo.DEV_BUILD_VERSION && PrismApi.VersionType != VersionType.DevBuild)
                {
                    errors.Add(new LoaderError(info, "Mod was built with an unstable developer build of Prism for testing purposes and can only be loaded by other developer builds.")); // Make it sound complicated lmao
                    loadAssembly = false;
                }

                if (refVer < curVer)
                    if (PrismApi.VersionType == VersionType.DevBuild)
                        errors.Add(new LoaderError(info, "Mod was built with a release build of Prism and may not work correctly with the latest developer build."));
                    else
                    {
                        bool minorDiff = refVer.Major == curVer.Major && refVer.Minor == curVer.Minor;

                        if (minorDiff) errors.Add(new LoaderError(info, "Warning: mod was built with a previous build of Prism, it might not work completely."));
                        else           errors.Add(new LoaderError(info, "Mod was built with a previous version of Prism and will probably not work correctly."));
                    }


                if (refVer > curVer && PrismApi.VersionType != VersionType.DevBuild)
                {
                    errors.Add(new LoaderError(info, "Mod was built with a newer version of Prism than the installed version. Update to the latest version of Prism (>=" + refVer + ") in order to load this mod."));
                    loadAssembly = false;
                }

                asm = loadAssembly ? Assembly.LoadFrom(p) : null;
            }
            catch (Exception e)
            {
                errors.Add(new LoaderError(info, "Could not load mod assembly", e));
                return null;
            }

            return asm == null ? null : LoadModFromAssembly(asm, info);
        }

        /// <summary>
        /// Loads all mods from the Prism mod directory and returns a list containing any loader errors encountered.
        /// </summary>
        /// <returns>Any <see cref="LoaderError"/>'s encountered while loading.</returns>
        internal static IEnumerable<LoaderError> Load()
        {
            Loading = true;

            Logging.LogInfo("Loading mods...");

            errors.Clear();

            if (!Directory.Exists(PrismApi.ModDirectory))
                Directory.CreateDirectory(PrismApi.ModDirectory);

            EntityDefLoader.SetupEntityHandlers();
            ResourceLoader.Setup();
            ContentLoader .Setup();

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

                            //// Temporary until we have a proper way to see loader errors
                            //if (ExceptionHandler.DetailedExceptions)
                            //    MessageBox.ShowError("An exception has occured:\n" + e);
                        }
                    }
                }
                else
                {
                    errors.Add(new LoaderError(s, String.Format("New entry was not added to ModData.mods while loading mod from path '{0}', see previous errors.", s)));
                    errors.Add(new LoaderError(s, "Mods loaded", ModData.mods));
                }
            }

            HookManager.Create();
            HookManager.CanCallHooks = true;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Loading = false;

            Logging.LogInfo("Mods finished loading.");

            HookManager.ModDef.OnAllModsLoaded();

            for (int i = 0; i < errors.Count; i++)
            {
                var s = errors[i].ToString();
                Logging.LogWarning(s);

#if DEV_BUILD
                Trace.WriteLine(s);
#endif
            }

            if (errors.Count > 0)
            {
                Trace.WriteLine("Some problems occured when loading mods. See the prism.log file for details.");

                ShowAllErrors();
            }

            return errors;
        }

        /// <summary>
        /// Unloads all loaded mods.
        /// </summary>
        internal static void Unload()
        {
            if (HookManager.ModDef != null)
                HookManager.ModDef.OnUnload();

            Unloading = true;

            Logging.LogInfo("Unloading mods...");

            HookManager.CanCallHooks = false;

            HookManager.Clear();

            foreach (var v in ModData.mods.Values)
                v.Unload();

            ModData.mods.Clear();

            EntityDefLoader.ResetEntityHandlers();
            ResourceLoader .Unload();
            ContentLoader  .Reset ();

            errors.Clear();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Logging.LogInfo("Mods finished unloading.");

            Unloading = false;
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
