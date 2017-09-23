// TODO: someone on Windows: create a csproj for this

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

static class Program
{
    readonly static Tuple<string, string>[] resources =
    {
        Tuple.Create("ReLogic.ReLogic", "ReLogic.dll"),
        Tuple.Create("Steamworks.NET.Linux.Steamworks.NET", "Steamworks.NET.dll")
    };

    static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: ExtractReLogic.exe <Terraria.exe> <dest dir>");
            return 1;
        }

        var terraria = args[0];
        var destDir  = args.Length == 1 ? Environment.CurrentDirectory : args[1];

        if (!File.Exists(terraria))
        {
            Console.Error.WriteLine("The input file doesn't exist.");
            return 1;
        }
        if (!Directory.Exists(destDir))
        {
            Console.Error.WriteLine("The output directory doesn't exist.");
            return 1;
        }

        var a = Assembly.ReflectionOnlyLoadFrom(terraria);

        foreach (var kvp in resources)
        {
            using (var fs = File.OpenWrite(destDir + Path.DirectorySeparatorChar + kvp.Item2))
            using (var ms = a.GetManifestResourceStream("Terraria.Libraries." + kvp.Item1 + ".dll"))
            {
                ms.CopyTo(fs);
            }
        }

        return 0;
    }
}

