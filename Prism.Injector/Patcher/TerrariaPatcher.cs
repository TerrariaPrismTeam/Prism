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

    public enum PatchPhase
    {
        FixBefore,

        Items,
        NPCs,
        Projs,
        Player,
        Mount,
        Main,
        Tile,
        World,
        Recipe,
        Buff,

        FixAfter,
        Finished
    }

    public static class TerrariaPatcher
    {
        internal static Platform Platform;

        internal static DNContext   context;
        internal static MemberResolver  memRes;

        static void FindPlatform()
        {
            if (memRes.GetType("Terraria.WindowsLaunch") != null)
                Platform = Platform.Windows;
            else if (memRes.GetType("Terraria.LinuxLaunch"  ) != null)
                Platform = Platform.Linux;
            else if (memRes.GetType("Terraria.MacLaunch"    ) != null)
                Platform = Platform.OSX;
            else
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.MacOSX:
                        Platform = Platform.OSX;
                        break;
                    case PlatformID.Unix:
                        Platform = Platform.Linux;
                        break;
                    default:
                        Platform = Platform.Windows;
                        break;
                }
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
        static void Publicify()
        {
            // make all types public

            foreach (TypeDef td in context.PrimaryAssembly.ManifestModule.Types)
                PublicifyRec(td);
        }

        static void AddInternalsVisibleToAttr()
        {
            //! raises attribute not imported error on write
            var ivt_t = memRes.ReferenceOf(typeof(InternalsVisibleToAttribute)).ResolveTypeDefThrow();
            var ivt_ctor = ivt_t.Methods.First(md => (md.Attributes & (MethodAttributes.SpecialName | MethodAttributes.RTSpecialName)) != 0);

            context.PrimaryAssembly.CustomAttributes.Add(new CustomAttribute(ivt_ctor, Encoding.UTF8.GetBytes("Prism")));
        }
        static void RemoveConsoleWriteLineInWndProcHook()
        {
            var kbi_t = memRes.GetType("Terraria.keyBoardInput");

            if (kbi_t.NestedTypes.Count == 0) // *nix version, only windows uses WndProc for text input (obviously)
                return;

            var inKey_t = kbi_t.NestedTypes[0];

            var preFilterMessage = inKey_t.GetMethod("PreFilterMessage");

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
        public static void OptimizeAll()
        {
            foreach (var td in context.PrimaryAssembly.ManifestModule.Types)
                foreach (var m in td.Methods)
                    if (m.Body != null)
                    {
                        m.Body.OptimizeBranches();
                        m.Body.OptimizeMacros();
                    }
        }

        public static void Patch(DNContext context, string outputPath)
        {
            TerrariaPatcher.context = context;
            memRes = TerrariaPatcher.context.Resolver;

            TerrariaPatcher.context.PrimaryAssembly.Name = "Prism.Terraria";
            TerrariaPatcher.context.PrimaryAssembly.ManifestModule.Name = TerrariaPatcher.context.PrimaryAssembly.Name + ".dll";

            FindPlatform();

            Publicify();
            //AddInternalsVisibleToAttr();
            RemoveConsoleWriteLineInWndProcHook();

            ItemPatcher      .Patch();
            NpcPatcher       .Patch();
            ProjectilePatcher.Patch();
            PlayerPatcher    .Patch();
            MountPatcher     .Patch();
            MainPatcher      .Patch();
            TilePatcher      .Patch();
            WorldFilePatcher .Patch();
            RecipePatcher    .Patch();
            BuffPatcher      .Patch();
            // do other stuff here

            OptimizeAll();

            // Newtonsoft.Json.dll, Steamworks.NET.dll and Ionic.Zip.CF.dll are required to write the assembly (and FNA and WindowsBase on mono, too)
            TerrariaPatcher.context.PrimaryAssembly.Write(outputPath);

            memRes = null;
            TerrariaPatcher.context = null;
        }
    }
}
