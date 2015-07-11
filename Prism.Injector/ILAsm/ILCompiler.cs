using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using SpecialFolder = System.Environment.SpecialFolder;

namespace Prism.Injector.ILAsm
{
    public class ILCompiler : ICodeCompiler
    {
        readonly static string
            D_NEWL = Environment.NewLine + Environment.NewLine,
            SPACE = " ",

            EXE = "--exe", DLL = "--dll",
            OUTPUT = "--output=",
            RESOURCE = "--resource=",
            DEBUG = "--debug",
            OPTIMIZE = "--optimize",
            DEF_ARGS = "--nologo --quiet ";

        readonly static bool IS_MONO = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;
        readonly static string ILASM = IS_MONO ? "ilasm"
            : Environment.GetFolderPath(SpecialFolder.Windows) + "\\Microsoft.NET\\Framework\\v4.0.30319\\ilasm.exe";

        readonly static char[] NEWL_CHARS = { '\r', '\n' };

        static string GetArgs(CompilerParameters options, string fileName, out string outp)
        {
            var args = new StringBuilder();

            args.Append(DEF_ARGS);

            outp =
                options.GenerateInMemory && String.IsNullOrWhiteSpace(options.OutputAssembly)
                ? Path.GetTempFileName()
                : options.OutputAssembly == Path.GetFileName(options.OutputAssembly) // just the name
                    ? Path.GetTempPath() + Path.DirectorySeparatorChar + options.OutputAssembly
                    : options.OutputAssembly;

            if (options.GenerateInMemory)
            {
                File.Delete(outp);
                options.TempFiles.AddFile(outp, true);
            }

            args.Append(options.GenerateExecutable ? EXE : DLL).Append(SPACE);
            args.Append(OUTPUT).Append(outp).Append(SPACE);
            args.Append(options.IncludeDebugInformation ? DEBUG : OPTIMIZE).Append(SPACE);

            if (!String.IsNullOrWhiteSpace(options.Win32Resource  ))
                args.Append(RESOURCE).Append(options.Win32Resource  ).Append(SPACE);
            if (!String.IsNullOrWhiteSpace(options.CompilerOptions))
                args                 .Append(options.CompilerOptions).Append(SPACE);

            args.Append(fileName);

            return args.ToString();
        }
        static Process LaunchProcess(CompilerParameters options, string fileName, out string outp)
        {
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo(ILASM, GetArgs(options, fileName, out outp))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,

                    RedirectStandardError = true,
                    RedirectStandardOutput = true,

                    WorkingDirectory = Environment.CurrentDirectory
                },

                PriorityClass = ProcessPriorityClass.AboveNormal,
                PriorityBoostEnabled = true
            };

            p.Start();
            p.WaitForExit();

            return p;
        }
        static CompilerResults GetResult(CompilerParameters options, Process p, string outp)
        {
            var r = new CompilerResults(options.TempFiles);

#pragma warning disable 618 // Type or member is obsolete
            r.Evidence = options.Evidence;
#pragma warning restore 618

            r.NativeCompilerReturnValue = p.ExitCode;

            r.PathToAssembly = outp;
            if (options.GenerateInMemory)
            {
                try
                {
                    r.CompiledAssembly = Assembly.LoadFrom(outp);
                    AppDomain.CurrentDomain.DomainUnload += (s, e) => File.Delete(outp);
                }
                catch // compilation error (LoadFrom failed)
                {
                    r.CompiledAssembly = null;
                }
            }

            r.Output.AddRange(p.StandardOutput.ReadToEnd().Split(NEWL_CHARS, StringSplitOptions.RemoveEmptyEntries));
            r.Errors.AddRange(
                (from e in p.StandardError.ReadToEnd().Split(NEWL_CHARS, StringSplitOptions.RemoveEmptyEntries)
                 select new CompilerError(String.Empty, 0, 0, String.Empty, e)).ToArray());

            return r;
        }

        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            if (!IS_MONO && !File.Exists(ILASM))
                throw new NotSupportedException("ILAsm.exe not found.");

            string outp;
            return GetResult(options, LaunchProcess(options, fileName, out outp), outp);
        }

        public CompilerResults CompileAssemblyFromSource     (CompilerParameters options, string   source   )
        {
            var f = Path.GetTempFileName();
            File.WriteAllText(f, source);

            options.TempFiles.AddFile(f, false);
            return CompileAssemblyFromFile(options, f);
        }
        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources  )
        {
            return CompileAssemblyFromSource(options, String.Join(D_NEWL, sources));
        }
        public CompilerResults CompileAssemblyFromFileBatch  (CompilerParameters options, string[] fileNames)
        {
            return CompileAssemblyFromSourceBatch(options, (from s in fileNames select File.ReadAllText(s)).ToArray());
        }

        public CompilerResults CompileAssemblyFromDom     (CompilerParameters options, CodeCompileUnit   compilationUnit)
        {
            throw new NotImplementedException();
        }
        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] compilationUnits)
        {
            throw new NotImplementedException();
        }
    }
}
