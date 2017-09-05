using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Xna.Framework;
using Prism.Debugging;
using Prism.Util;

namespace Prism
{
    static class AssemblyResolver
    {
        readonly static Assembly[] DllContainingAssemblies =
        {
            Assembly.Load(AssemblyName.GetAssemblyName("Prism.Terraria.dll")), // can't use typeof(...).Assembly here, see comment in Program.Main
                                                                               // NOTE: do not change this element's index (i.e. move it around), it is sometimes referenced as ...[0].
            Assembly.GetExecutingAssembly()
        };
        readonly static string[] DllContainedAssemblies =
        {
            "Newtonsoft.Json",
            "Steamworks.NET",
            "Ionic.Zip.CF",
            "ReLogic"
        };
        readonly static string[] AssemblyExts =
        {
            "dll",
            "exe"
        };
        readonly static Assembly[] CplImplAsm = // assemblies with the same types but different implementation assemblies (Terraria (Prism.Terraria), XNA/FNA, WindowsBase)
        {
            DllContainingAssemblies[0], // reuse code (see the other comment)
            typeof(Vector2         ).Assembly,
            typeof(DependencyObject).Assembly
        };

        readonly static string DOT = ".";

        static bool inCallback = false;

        internal static List<string> searchPaths = new List<string>();

        static Assembly SafeLoadFile(string path)
        {
            try
            {
                return Assembly.LoadFile(path);
            }
            catch
            {
                return null;
            }
        }

        static Assembly ResolveFromResources(Assembly container, string name)
        {
            string[] allNames = container.GetManifestResourceNames();

            string
                fileName = name + ".dll",
                fullName = Array.Find(allNames, e => e.EndsWith(fileName, StringComparison.Ordinal)),

                pdbName = Path.ChangeExtension(fileName, ".pdb"),
                fullPdbName = Array.Find(allNames, e => e.EndsWith(pdbName, StringComparison.Ordinal)); // null if not found

            if (String.IsNullOrEmpty(fullName))
                return null;

            using (Stream dllStream = container.GetManifestResourceStream(fullName))
            {
                byte[] assembly = new byte[dllStream.Length];
                dllStream.Read(assembly, 0, assembly.Length);

                // load PDB if exists
                if (!String.IsNullOrEmpty(fullPdbName))
                    using (Stream pdbStream = container.GetManifestResourceStream(fullPdbName))
                    {
                        byte[] pdb = new byte[pdbStream.Length];
                        pdbStream.Read(pdb, 0, pdb.Length);

                        return Assembly.Load(assembly, pdb);
                    }

                return Assembly.Load(assembly);
            }
        }

        internal static Assembly LoadModAssembly(string path)
        {
            return searchPaths.Select(p => // for every search path
                    AssemblyExts.Select(e => // for every extension
                        SafeLoadFile(Path.Combine(p, path)) // try to load the assembly
                )).Aggregate(Enumerable.Concat) // joins the nested collections back to a normal one
            .FirstOrDefault(a => a != null);
        }

        internal static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (_, rea) =>
            {
                // prevent stack overflow
                if (inCallback)
                    return null;

                inCallback = true;

                string displayName = new AssemblyName(rea.Name).Name;

                // return outside of the 'try', this doesn't need to be logged.
                for (int i = 0; i < CplImplAsm.Length; i++)
                    if (displayName == CplImplAsm[i].GetName().Name)
                    {
                        inCallback = false;

                        return CplImplAsm[i];
                    }
                if (displayName == PrismApi.PrismTerrariaString || displayName == PrismApi.TerrariaString) // if it fails when checking the CplImplAsms or something is somehow linked to vanilla terraria
                {
                    inCallback = false;

                    return DllContainingAssemblies[0];
                }

                Assembly ret = null;

                try
                {
                    // LINQ uses lazy evaluation, thus it doesn't try to load the assembly from
                    // all assemblies in DllContainingAssemblies or from the search paths,
                    // but the loop breaks when the predicate in FirstOrDefault returns true.
                    return ret =
                        (Array.IndexOf(DllContainedAssemblies, displayName) != -1 // is it an assembly that is stored as an embedded resource (in one of the main assemblies)?
                            ? DllContainingAssemblies.Select(c => // for each assebly that contains embedded libraries
                                ResolveFromResources(c, displayName)) // try to load it
                            : searchPaths.Select(p => // for every search path
                                AssemblyExts.Select(e => // for every extension
                                    SafeLoadFile(p + displayName + DOT + e) // try to load the assembly
                            )).Flatten() // joins the nested collections back to a normal one
                        ).FirstOrDefault(a => a != null);
                }
                finally // abusing 'finally' so it will log success or failure, but without c/ping this code before every return.
                        // also quite useful for setting inCallback to false
                {
                    if (!rea.Name.Contains(".resources") /* some assemblies look for a resources assembly, even when it doesn't exist */)
                        if (ret == null)
                            Logging.LogWarning("Could not resolve assembly " + rea.Name);
                        else
                            Logging.LogInfo("Resolved assembly " + rea.Name + ".");

                    inCallback = false;
                }
            };
        }
    }
}
