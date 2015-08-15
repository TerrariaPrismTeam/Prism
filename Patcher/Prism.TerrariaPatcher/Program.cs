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
        public static bool VsBuild = false;

        public static string TerrariaExecutable = "Terraria.exe";
        public static string PrismAssembly = "Prism.Terraria.dll";

        static int Main(string[] args)
        {
            if (!ParseRuntimeArgs(args))
                return 1;

            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!File.Exists(TerrariaExecutable))
            {
                Console.Error.WriteLine("Terraria.exe not found. (full path: \"" + TerrariaExecutable + "\")");
                // the file is added in the .gitignore (and so is the patched file)
                if (VsBuild)
                    Console.Error.WriteLine(@"In order to build Prism, you must place a copy of your own Terraria.exe file in the '.\References' directory.");

                return 1;
            }

            var c = new CecilContext(TerrariaExecutable);

            var d = Path.GetDirectoryName(PrismAssembly);
            if (!Directory.Exists(d))
                Directory.CreateDirectory(d);

            // this will stop the build process if the patcher fails, because a reference in Prism.csproj will be missing
            if (File.Exists(PrismAssembly))
                File.Delete(PrismAssembly);

            try
            {
                Patcher.Patch(c, PrismAssembly);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Something went wrong while patching " + Path.GetFileName(TerrariaExecutable) + ".");
                Console.Error.WriteLine(e);

                return 1;
            }

            return 0;
        }

        public static bool ParseRuntimeArgs(string[] unparsedArgs)
        {
            string[] args = unparsedArgs.Select(x => x.Trim()).ToArray();
            if (args[0].ToLowerInvariant() == "vsbuild")
                VsBuild = true;
            else if (args.Length >= 1)
            {
                TerrariaExecutable = args[0];

                if (args.Length >= 2)
                    PrismAssembly = args[1];
            }
            else
            {
                if (File.Exists(TerrariaExecutable))
                {
                    var result = MessageBox.Show("A 'Terraria.exe' file was found in the same folder as this patcher. Would you like to patch it now? If you choose not to patch that particular 'Terraria.exe' file you browse for the correct file.", "Terraria.exe detected.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        return true;
                    if (result == DialogResult.Cancel)
                        return false; //It's a Yes/No/Cancel box not a fucking Yes/No box quit fucking breaking things randomly
                }

                var fod = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    FileName = "Terraria.exe",
                    Title = "Select your Terraria.exe file"
                };

                var fodResult = fod.ShowDialog();
                if (fodResult == DialogResult.Cancel)
                    return false;

                TerrariaExecutable = fod.FileName;
            }

            return true;
        }
    }
}
