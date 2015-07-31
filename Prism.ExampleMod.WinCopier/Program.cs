using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Prism.ExampleMod.WinCopier
{
    public class Program
    {
        public static bool GetYesNoInput()
        {
            var eInput = ErasableInput.Get();
            var choice = eInput.Input.ToUpper().Trim();

            if (choice.StartsWith("Y", StringComparison.Ordinal))
            {
                return true;
            }
            else if (choice.StartsWith("N", StringComparison.Ordinal))
            {
                return false;
            }
            eInput.Erase();
            return GetYesNoInput();
        }

        public static bool AskBool(string question)
        {
#if TRACE
            Console.Write(question + " (y/n): ");
            return GetYesNoInput();
#else
            return true;
#endif
        }

        public static void Write(string text)
        {
#if TRACE
            Console.Write(text);
#endif
        }

        public static void WriteLine(string text = "")
        {
#if TRACE
            Console.WriteLine(text);
#endif
        }

        public static void Main(string[] args)
        {
            var slnDir = Path.Combine(Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar).Except(new string[] { "Prism.ExampleMod.WinCopier", "bin", "Debug" }).ToArray()).Replace(":", ":\\");
            var exModBuildDir = Path.Combine(slnDir, "Prism.ExampleMod", "bin", "Debug");
            var prismModDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "Prism", "Mods");
            var exModTargetDir = Path.Combine(prismModDir, "Prism.ExampleMod");

            WriteLine(string.Format("          Prism Solution Directory: '{0}'", slnDir));
            WriteLine(string.Format(" Prism Example Mod Build Directory: '{0}'", exModBuildDir));
            WriteLine(string.Format("               Prism Mod Directory: '{0}'", prismModDir));
            WriteLine(string.Format("Prism Example Mod Target Directory: '{0}'", exModTargetDir));

            WriteLine();

            bool proceed = AskBool(string.Format("Check the above directories. Copy all files from '{0}' to '{1}'?", exModBuildDir, exModTargetDir));

            if (proceed)
            {
                var destDirFiles = Directory.GetFiles(exModTargetDir, "*", SearchOption.AllDirectories).Select(x => x.Replace(exModTargetDir + Path.DirectorySeparatorChar, string.Empty));


                WriteLine(string.Format("Deleting {0} existing file(s) from destination directory:", destDirFiles.Count()));

                foreach(var f in destDirFiles)
                {
                    var destF = Path.Combine(exModTargetDir, f);
                    Write(string.Format("    '{0}'...", f));

                    if (!File.Exists(destF))
                    {
                        WriteLine("No longer exists.");
                        continue;
                    }

                    File.Delete(destF);

                    WriteLine("Done.");
                }

                var destDirFolders = Directory.GetDirectories(exModTargetDir, "*", SearchOption.AllDirectories).Select(x => x.Replace(exModBuildDir + Path.DirectorySeparatorChar, string.Empty));

                WriteLine(string.Format("Done.\n\nDeleting {0} folder(s) from destination directory:", destDirFolders.Count()));

                foreach(var f in destDirFolders)
                {
                    var destF = Path.Combine(exModTargetDir, f);
                    Write(string.Format("    '\\{0}'...", f));

                    if (!Directory.Exists(destF))
                    {
                        WriteLine("No longer exists.");
                        continue;
                    }

                    Directory.Delete(destF, true);

                    WriteLine("Done.");
                }

                var srcDirFiles = Directory.GetFiles(exModBuildDir, "*.*", SearchOption.AllDirectories).Select(x => x.Replace(exModBuildDir + Path.DirectorySeparatorChar, string.Empty));

                WriteLine(string.Format("Done.\n\nCopying {0} file(s) from '{1}' to '{2}':", srcDirFiles.Count(), exModBuildDir, exModTargetDir));

                var failedCopies = new List<string>();

                foreach(var f in srcDirFiles)
                {
                    var srcF = Path.Combine(exModBuildDir, f);
                    var destF = Path.Combine(exModTargetDir, f);
                    var destFinf = new FileInfo(destF);
                    Write(string.Format("    '{0}'...", f));

                    try
                    {
                        if (!Directory.Exists(destFinf.DirectoryName))
                        {
                            Directory.CreateDirectory(destFinf.DirectoryName);
                        }
                        if (!File.Exists(destF))
                        {
                            FileStream newFile = File.Create(destF);
                            newFile.Close();
                        }
                        File.Copy(srcF, destF, true);
                    }
                    catch (Exception e)
                    {
                        failedCopies.Add(string.Format("    '{0}' --> \"{1}\"", f, e.Message));
                        WriteLine("Failed.");
                        continue;
                    }

                    WriteLine("Done.");
                }

                WriteLine("Done.");

                if (failedCopies.Count > 0)
                {
                    Console.WriteLine("Encountered the following error(s) while copying:"); //These appear even without TRACE

                    foreach (var fc in failedCopies)
                    {
                        Console.WriteLine(fc);
                    }

#if !TRACE
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadLine();
#endif
                }

                WriteLine("\nPress any key to continue...");

#if TRACE
                Console.ReadLine();
#endif
            }
        }
    }

    public struct ErasableInput
    {
        public string Input
        {
            get;
            private set;
        }
        private Point StartingPoint;

        public void MoveToStartingPoint()
        {
            Console.CursorLeft = StartingPoint.X;
            Console.CursorLeft = StartingPoint.X;
        }

        private void WriteBlankText()
        {
            Console.Write(new string(' ', Input.Length));
        }

        public void Erase()
        {
            MoveToStartingPoint();
            WriteBlankText();
            MoveToStartingPoint();
        }

        public static ErasableInput Get()
        {
            ErasableInput eri = new ErasableInput();
            eri.StartingPoint = new Point(Console.CursorLeft, Console.CursorTop);
            eri.Input = Console.ReadLine();
            return eri;
        }
    }
}
