using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class TerrariaPatcher
    {
        internal static CecilContext   context;
        internal static MemberResolver  memRes;

        static void PublicifyRec(TypeDefinition td)
        {
            if (!td.IsPublic)
                td.Attributes =
                    (td.Attributes & ~(td.Attributes & TypeAttributes.VisibilityMask)) | // remove all bits from the visibility mask (contains IsNested!)
                    (td.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public); // set it to either Public (1) or NestedPublic (2), if it was nested before.

            foreach (FieldDefinition  d in td.Fields ) if (                !d.IsPublic) d.IsPublic = true;
            foreach (MethodDefinition d in td.Methods) if (!d.IsVirtual && !d.IsPublic) d.IsPublic = true; // don't change access modifier of overridden protected members etc

            if (td.HasNestedTypes)
                foreach (TypeDefinition d in td.NestedTypes)
                    PublicifyRec(d);
        }
        static void Publicify()
        {
            // make all types and members in the "Terraria" and "Terraria.Map" namespace public

            foreach (TypeDefinition td in context.PrimaryAssembly.MainModule.Types)
            {
                if (td.Namespace != "Terraria" && td.Namespace != "Terraria.Map")
                    continue;

                PublicifyRec(td);
            }
        }

        static void AddInternalsVisibleToAttr()
        {
            // raises attribute not imported error on write
            var ivt_t = memRes.ReferenceOf(typeof(InternalsVisibleToAttribute)).Resolve();
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
            var pfmilp = preFilterMessage.Body.GetILProcessor();

            var callWriteLine = instrs.First(i => i.OpCode.Code == Code.Call && i.Operand is MethodReference && ((MethodReference)i.Operand).Name == "WriteLine");

            pfmilp.Remove(callWriteLine.Previous);
            pfmilp.Remove(callWriteLine);
        }

        public static void Patch(CecilContext context, string outputPath)
        {
            TerrariaPatcher.context = context;
            memRes = TerrariaPatcher.context.Resolver;

            TerrariaPatcher.context.PrimaryAssembly.Name.Name = "Prism.Terraria";
            TerrariaPatcher.context.PrimaryAssembly.MainModule.Name = TerrariaPatcher.context.PrimaryAssembly.Name.Name + ".dll";

            Publicify();
            //AddInternalsVisibleToAttr();
            RemoveConsoleWriteLineInWndProcHook();

            ItemPatcher      .Patch();
            NpcPatcher       .Patch();
            ProjectilePatcher.Patch();
            PlayerPatcher    .Patch();
            MainPatcher      .Patch();
            // do other stuff here

            // Newtonsoft.Json.dll, Steamworks.NET.dll and Ionic.Zip.CF.dll are required to write the assembly (and FNA and WindowsBase on mono, too)
            TerrariaPatcher.context.PrimaryAssembly.Write(outputPath);

            memRes = null;
            TerrariaPatcher.context = null;
        }
    }
}
