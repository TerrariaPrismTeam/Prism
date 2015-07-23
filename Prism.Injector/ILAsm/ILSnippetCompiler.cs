using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.ILAsm
{
    public static class ILSnippetCompiler
    {
        readonly static string
            TAB = "    ",
            OPEN_BRACE = "{",
            CLOSE_BRACE = "}",
            EMPTY_BRACES = " {}",
            CLASS_NAME = "Container",
            METHOD_NAME = "Snippet",

            ASM_EXT = ".assembly extern ",
            CLASS_DECL = ".class private abstract sealed auto ansi " + CLASS_NAME,
            INHERIT_OBJ = "extends [mscorlib]System.Object",
            METHOD_DECL = ".method private hidebysig static void " + METHOD_NAME,
            CIL_MANAGED = "() cil managed",
            DEF_MAXSTACK = ".maxstack 8",
            RET_OPCODE = "ret";

        static string manif, refs;

        static ILCompiler compiler = new ILCompiler();

        static string[] OneStringArr = new string[0];

        static void InitResourceStrings()
        {
            if (String.IsNullOrEmpty(refs ))
                using (var r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Prism.Injector.ILAsm.asm-refs.il" )))
                {
                    refs  = r.ReadToEnd();
                }

            if (String.IsNullOrEmpty(manif))
                using (var r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Prism.Injector.ILAsm.asm-manif.il")))
                {
                    manif = r.ReadToEnd();
                }
        }

        static string CreateSource(string[] snippets, Assembly[] references)
        {
            InitResourceStrings();

            var src = new StringBuilder();

            src.Append(refs);

            for (int i = 0; i < references.Length; i++)
                src.Append(ASM_EXT).Append(references[i].GetName().Name).AppendLine(EMPTY_BRACES);

            src.AppendLine(CLASS_DECL)
                .Append(TAB).Append(TAB).AppendLine(INHERIT_OBJ)
                .AppendLine(OPEN_BRACE);

            for (int i = 0; i < snippets.Length; i++)
            {
                src.Append(TAB).Append(METHOD_DECL).Append(i).AppendLine(CIL_MANAGED)
                    .Append(TAB).AppendLine(OPEN_BRACE)
                    .Append(TAB).AppendLine(DEF_MAXSTACK) // this doesn't matter, because it will never be executed, just extracted using cecil
                    .AppendLine(snippets[i])
                    .Append(TAB).Append(TAB).AppendLine(RET_OPCODE)
                    .Append(TAB).AppendLine(CLOSE_BRACE);
            }

            src.AppendLine("}");

            return src.ToString();
        }

        public static Instruction[] CompileSnippet(string snippet, Assembly[] references)
        {
            OneStringArr[0] = snippet;
            return CompileSnippets(OneStringArr, references)[0];
        }
        public static Instruction[][] CompileSnippets(string[] snippets, Assembly[] references)
        {
            var cp = new CompilerParameters();

            cp.GenerateExecutable      = false;
            cp.IncludeDebugInformation = false;
            cp.GenerateInMemory        = false;

            var cr = compiler.CompileAssemblyFromSource(cp, CreateSource(snippets, references));

            if (cr.Errors.HasErrors)
                throw new AggregateException(from CompilerError e in cr.Errors select new Exception(e.ErrorText));

            var ad = AssemblyDefinition.ReadAssembly(cr.PathToAssembly);

            List<Instruction[]> mtds = new List<Instruction[]>();

            foreach (var m in ad.MainModule.GetType(CLASS_NAME).Methods.Where(m => m.Name.StartsWith(METHOD_NAME, StringComparison.Ordinal) /* should be in the correct order */))
                mtds.Add(m.Body.Instructions.Take(m.Body.Instructions.Count - 1 /* don't include the final 'ret' */).ToArray());

            return mtds.ToArray();
        }
    }
}
