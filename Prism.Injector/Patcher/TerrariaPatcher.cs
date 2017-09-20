using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    public enum Platform
    {
        Windows,
        Linux,
        OSX
    }

    public static class TerrariaPatcher
    {
        internal static Platform Platform;

        internal static DNContext   context;
        internal static MemberResolver  memRes;

        static Platform OSPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return Platform.OSX;
                case PlatformID.Unix:
                    return Platform.Linux;
                default:
                    return Platform.Windows;
            }
        }
        static Platform FindPlatform()
        {
            if (memRes.GetType("Terraria.WindowsLaunch"   ) != null)
                return Platform.Windows;
            else if (memRes.GetType("Terraria.LinuxLaunch") != null)
                return Platform.Linux;
            else if (memRes.GetType("Terraria.MacLaunch"  ) != null)
                return Platform.OSX;
            else
                return OSPlatform(); // meh
        }

        static void PublicifyRec(TypeDef td)
        {
            if (!td.IsPublic)
                td.Attributes =
                    (td.Attributes & ~(td.Attributes & TypeAttributes.VisibilityMask)) | // remove all bits from the visibility mask (contains IsNested!)
                    (td.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public); // set it to either Public (1) or NestedPublic (2), if it was nested before.

            foreach (FieldDef  d in td.Fields )
                if (                !d.IsPublic)
                {
                    if ((d.Access & FieldAttributes.Private) != 0)
                        d.Access ^= FieldAttributes.Private;

                    d.Access |= FieldAttributes.Public;
                }
            foreach (MethodDef d in td.Methods)
                if (!d.IsVirtual && !d.IsPublic) // don't change access modifier of overridden protected members etc
                {
                    if ((d.Access & MethodAttributes.Private) != 0)
                        d.Access ^= MethodAttributes.Private;

                    d.Access |= MethodAttributes.Public;
                }

            if (td.HasNestedTypes)
                foreach (TypeDef d in td.NestedTypes)
                    PublicifyRec(d);
        }
        static void Publicify(Action<string> log)
        {
            // make all types public

            foreach (TypeDef td in context.PrimaryAssembly.ManifestModule.Types)
                PublicifyRec(td);
        }

        static void AddInternalsVisibleToAttr(Action<string> log)
        {
            //! raises attribute not imported error on write
            var ivt_t = memRes.ReferenceOf(typeof(InternalsVisibleToAttribute)).ResolveTypeDefThrow();
            var ivt_ctor = ivt_t.Methods.First(md => (md.Attributes & (MethodAttributes.SpecialName | MethodAttributes.RTSpecialName)) != 0);

            context.PrimaryAssembly.CustomAttributes.Add(new CustomAttribute(ivt_ctor, Encoding.UTF8.GetBytes("Prism")));
        }
        static void RemoveConsoleWriteLineInWndProcHook(Action<string> log)
        {
            var kbi_t = memRes.GetType("Terraria.keyBoardInput");

            if (kbi_t == null || kbi_t.NestedTypes.Count == 0) // *nix version, only windows uses WndProc for text input (obviously)
                return;

            var inKey_t = kbi_t.NestedTypes[0];

            var preFilterMessage = inKey_t.GetMethod("PreFilterMessage");

            if (preFilterMessage == null) // other *nix versions
                return;

            var instrs = preFilterMessage.Body.Instructions;

            var callWriteLine = instrs.First(i =>
            {
                if (i.OpCode.Code != Code.Call || !(i.Operand is MemberRef))
                    return false;

                var mr = (MemberRef)i.Operand;

                return mr.IsMethodRef && mr.Class.FullName == typeof(Console).FullName && mr.Name == "WriteLine";
            });

            instrs.RemoveAt(instrs.IndexOf(callWriteLine) - 1);
            instrs.Remove(callWriteLine);
        }
        public static void OptimizeAll(Action<string> log)
        {
            foreach (var td in context.PrimaryAssembly.ManifestModule.Types)
                foreach (var m in td.Methods)
                    if (m.Body != null)
                    {
                        m.Body.OptimizeBranches();
                        m.Body.OptimizeMacros();
                    }
        }

        static void FixParseArguementsTypo(Action<string> log)
        {
            memRes.GetType("Terraria.Utils").GetMethod("ParseArguements").Name = "ParseArguments";
        }

        public static void Patch(DNContext context, string outputPath, Action<string> log = null)
        {
            log = log ?? Console.Error.WriteLine;

            TerrariaPatcher.context = context;
            memRes = TerrariaPatcher.context.Resolver;

            TerrariaPatcher.context.PrimaryAssembly.Name = "Prism.Terraria";
            TerrariaPatcher.context.PrimaryAssembly.ManifestModule.Name = TerrariaPatcher.context.PrimaryAssembly.Name + ".dll";

            var ver = TerrariaPatcher.context.PrimaryAssembly.Version;
            var min = new Version(AssemblyInfo.MIN_TERRARIA_VERSION);
            var max = new Version(AssemblyInfo.MAX_TERRARIA_VERSION);

            if (ver < min)
                throw new NotSupportedException("The Terraria.exe version (" + ver + ") is too old!");
            if (ver > max)
                log("Warning: This Terraria.exe version (" + ver + ") is not supported, patching will probably fail.");
            else
                log("Version is " + ver);

            Platform = FindPlatform();
            log("Platform is " + Platform);
            if (Platform != OSPlatform())
                log("Warning: Platform is not the same as " + OSPlatform() + ", patching might fail.");

            log("Making members public...");
            Publicify(log);
            log("Fixing Terraria internals...");
            //AddInternalsVisibleToAttr(log);
            RemoveConsoleWriteLineInWndProcHook(log);
            FixParseArguementsTypo(log);

            log("Patching Terraria.Item...");
            ItemPatcher      .Patch(log);
            log("Patching Terraria.NPC...");
            NpcPatcher       .Patch(log);
            log("Patching Terraria.Projectile...");
            ProjectilePatcher.Patch(log);
            log("Patching Terraria.Player...");
            PlayerPatcher    .Patch(log);
            log("Patching Terraria.Mount...");
            MountPatcher     .Patch(log);
            log("Patching Terraria.Lang...");
            LangPatcher      .Patch(log);
            log("Patching Terraria.Main...");
            MainPatcher      .Patch(log);
            log("Patching Terraria.Tile...");
            TilePatcher      .Patch(log);
            log("Patching Terraria.IO.WorldFile...");
            WorldFilePatcher .Patch(log);
            log("Patching Terraria.Recipe...");
            RecipePatcher    .Patch(log);
            log("Patching Terraria.Buff...");
            BuffPatcher      .Patch(log);
            // do other stuff here

            log("Optimising MSIL...");
            OptimizeAll(log);

            log("Writing file...");

            // Newtonsoft.Json.dll, Steamworks.NET.dll and Ionic.Zip.CF.dll are required to write the assembly (and FNA and WindowsBase on mono, too)
            TerrariaPatcher.context.PrimaryAssembly.Write(outputPath);

            log("Done!");

            memRes = null;
            TerrariaPatcher.context = null;
        }
    }
}
