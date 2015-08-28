using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Prism.Injector;

using Patcher = Prism.Injector.Patcher.TerrariaPatcher;

/*
    This build file loads Terraria.exe, patches it with various hacks, then saves it as a new assembly, Prism.Terraria.dll, which Prism uses as a reference.
    this program is invoked from the project file, this just executes everything in Prism.Injector.dll.
    the project is referenced from the main Prism project, so Terraria.exe will be patched before Prism is built.
*/

namespace Prism.TerrariaPatcher
{
    static class Program
    {
        public static bool MsBuild = false;

        public static string TerrariaExecutable = "Terraria.exe";
        public static string PrismAssembly = "Prism.Terraria.dll";

        public readonly static bool IsWindows = Environment.OSVersion.Platform <= PlatformID.WinCE;

        public readonly static string
            TerrFoundMessage = "A 'Terraria.exe' file was found in the same folder as this patcher. Would you like to patch it now?" +
                "If you choose not to patch that particular 'Terraria.exe' file you browse for the correct file.";

        [STAThread]
        static int Main(string[] args)
        {
            var toRem = new List<string>();

            try
            {
                if (!ParseRuntimeArgs(args))
                    return 1;

                if (MsBuild)
                    Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (!File.Exists(TerrariaExecutable))
                {
                    Console.WriteLine("Terraria.exe not found. (full path: \"" + Path.GetFullPath(TerrariaExecutable) + "\")");
                    // the file is added in the .gitignore (and so is the patched file)
                    if (MsBuild)
                        Console.WriteLine("In order to build Prism, you must place a copy of your own Terraria.exe file in the '."
                            + Path.DirectorySeparatorChar /* be cross-platform */ + "References' directory.");

                    return 1;
                }

                var d = Path.GetDirectoryName(TerrariaExecutable);

                // just copy the files so the assembly resolving works, they'll be removed when it finished
                if (!MsBuild)
                {
                    var  fs = new[] { "Newtonsoft.Json", "Steamworks.NET", "Ionic.Zip.CF" }.Select(n => Path.Combine(d, n + ".dll"));
                    var ufs = new[] { "WindowsBase"    , "FNA"                            }.Select(n => Path.Combine(d, n + ".dll"));

                    foreach (var s in fs.Concat(IsWindows ? new string[0] : ufs))
                    {
                        var t = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(s));

                        if (File.Exists(s) && !File.Exists(t) /* don't do a useless copy (and worse, remove it afterwards, even when it could be needed later) */)
                        {
                            File.Copy(s, t);
                            toRem.Add(t);
                        }
                    }
                }

                var c = new CecilContext(TerrariaExecutable);

                d = Path.GetDirectoryName(PrismAssembly);
                if (!Directory.Exists(d))
                    Directory.CreateDirectory(d);

                // this will stop the build process if the patcher fails, because a reference in Prism.csproj will be missing
                if (MsBuild && File.Exists(PrismAssembly))
                    File.Delete(PrismAssembly);

                try
                {
                    Console.Write("Patching, please wait... ");

                    Patcher.Patch(c, PrismAssembly);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong while patching " + Path.GetFileName(TerrariaExecutable) + ":");
                    Console.WriteLine(e);

                    return 1;
                }

                Console.WriteLine("Patching finished.");

                return 0;
            }
            finally
            {
                if (!MsBuild)
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
                }

                foreach (var s in toRem)
                    File.Delete(s);
            }
        }

        public static bool ParseRuntimeArgs(string[] unparsedArgs)
        {
            string[] args = unparsedArgs.Select(x => x.Trim()).ToArray();
            if (args.Length > 0)
            {
                int off = 0;

                if (args[0].ToUpperInvariant() == "MSBUILD")
                {
                    MsBuild = true;
                    off++;
                }

                TerrariaExecutable = args[0 + off];

                if (args.Length >= 2 + off)
                    PrismAssembly = args[1 + off];
                else
                    PrismAssembly = Path.Combine(Path.GetDirectoryName(args[0 + off]), "Prism.Terraria.dll");
            }
            else
            {
                if (File.Exists(TerrariaExecutable))
                    if (IsWindows)
                    {
                        var result = MessageBox.Show(TerrFoundMessage, "Terraria.exe detected.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                            return true;
                        if (result == DialogResult.Cancel)
                            return false; //It's a Yes/No/Cancel box not a fucking Yes/No box quit fucking breaking things randomly
                    }
                    else
                    {
                        Console.Write(TerrFoundMessage + " (y/anything else): ");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                            return true;
                    }

                if (IsWindows)
                {
                    var fod = new OpenFileDialog()
                    {
                        CheckFileExists = true,
                        FileName = "Terraria.exe",
                        Title = "Select your Terraria.exe file",
                        AddExtension = true,
                        DefaultExt = ".exe",
                        Filter = "Terraria|Terraria.exe" // only match the string "Terraria.exe" (case-insensitive, because it's Windows)
                    };

                    var fodResult = fod.ShowDialog();
                    if (fodResult == DialogResult.Cancel)
                        return false;

                    TerrariaExecutable = fod.FileName;
                    PrismAssembly = Path.Combine(Path.GetDirectoryName(TerrariaExecutable), "Prism.Terraria.dll");
                }
                else
                {
                TRY:
                    Console.Write("Please drop your Terraria.exe file on this window and press enter: ");

                    var s = Console.ReadLine();

                    if (String.IsNullOrEmpty(s) || !File.Exists(s))
                    {
                        Console.Write("Invalid file. Try again? (y/anything else): ");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                            goto TRY;
                        else
                            return false;
                    }
                    else
                    {
                        TerrariaExecutable = Console.ReadLine();
                        PrismAssembly = Path.Combine(Path.GetDirectoryName(args[0]), "Prism.Terraria.dll");
                    }
                }
            }

            return true;
        }
    }
}
