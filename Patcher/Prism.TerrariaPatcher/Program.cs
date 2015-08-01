/*
    This build file loads Terraria.exe, patches it with various hacks, then saves it as a new assembly, Prism.Terraria.dll, which Prism uses as a reference.
    this program is invoked from the project file, this just executes everything in Prism.Injector.dll.
    the project is referenced from the main Prism project, so Terraria.exe will be patched before Prism is built.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prism.Injector;
using Prism.Injector.Patcher;
using System.Windows.Forms;

using Patcher = Prism.Injector.Patcher.TerrariaPatcher;

namespace Prism.TerrariaPatcher
{
    static class Program
    {
        public static bool VsBuild = false;

        public static string TerrariaExecutable = "Terraria.exe";
        public static string PrismAssembly = "Prism.Terraria.dll";

        static void Main(string[] args)
        {
            if (!ParseRuntimeArgs(args))
            {
                return;
            }

            if (!File.Exists(TerrariaExecutable))
            {
                Console.Error.WriteLine("Terraria.exe not found. (full path: \"" + TerrariaExecutable + "\")");
                // the file is added in the .gitignore (and so is the patched file)
                if (VsBuild)
                    Console.Error.WriteLine(@"In order to build Prism, you must place a copy of your own Terraria.exe file in the '.\References' directory.");
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
            }
        }

        public static bool ParseRuntimeArgs(string[] unparsedArgs)
        {
            string[] args = unparsedArgs.Select(x => x.Trim().ToLower()).ToArray();           
            if (args[0] == "vsbuild")
            {
                VsBuild = true;
            }
            else if (args.Length >= 1)
            {
                TerrariaExecutable = args[0];

                if (args.Length >= 2)
                {
                    PrismAssembly = args[1];
                }
            }
            else
            {
                if (File.Exists(TerrariaExecutable))
                {
                    var result = MessageBox.Show("A 'Terraria.exe' file was found in the same folder as this patcher. Would you like to patch it now? If you choose not to patch that particular 'Terraria.exe' file you browse for the correct file.", "Terraria.exe detected.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        return true;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                    
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

        public static void PrintConsole(string text, ConsoleColor? colorFG = null, ConsoleColor? colorBG = null)
        {
            ConsoleColor prevFG = Console.ForegroundColor;
            ConsoleColor prevBG = Console.BackgroundColor;            

            Console.ForegroundColor = colorFG ?? prevFG;
            Console.BackgroundColor = colorBG ?? prevBG;

            Console.WriteLine(text);

            Console.ForegroundColor = prevFG;
            Console.BackgroundColor = prevBG;
        }
    }
}
