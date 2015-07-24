using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prism.Injector;
using Prism.Injector.Patcher;

using Patcher = Prism.Injector.Patcher.TerrariaPatcher;

namespace Prism.TerrariaPatcher
{
    static class Program
    {
        // this build file patches Terraria.exe so the Prism assembly can use that as a reference.
        // this program is invoked from the project file, this just executes everything in Prism.Injector.dll.
        // the project is referenced from the main Prism project, so Terraria.exe will be patched before Prism is built.

        static void Main(string[] args)
        {
            // writing to stderr cancels the MSBuild build process (I think)
            if (args.Length != 2)
                Console.Error.WriteLine("Incorrect usage, two arguments required: Terraria.exe path and output assembly path.");

            if (!File.Exists(args[0]))
            {
                Console.Error.WriteLine("Terraria.exe not found. (full path: \"" + args[0] + "\")");
                // the file is added in the .gitignore (and so is the patched file)
                Console.Error.WriteLine("In order to build Prism, one must provide their own Terraria.exe file and put it in the References subdirectory.");
            }

            var c = new CecilContext(args[0]);

            var d = Path.GetDirectoryName(args[1]);
            if (!Directory.Exists(d))
                Directory.CreateDirectory(d);

            // this will stop the build process if the patcher fails, because a reference in Prism.csproj will be missing
            if (File.Exists(args[1]))
                File.Delete(args[1]);

            try
            {
                Patcher.Patch(c, args[1]);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Something went wrong while patching " + Path.GetFileName(args[0]) + ".");
                Console.Error.WriteLine(e);
            }
        }
    }
}
