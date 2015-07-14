using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector
{
    public static class InjectionExample
    {
        public static void Example()
        {
            CecilContext c = new CecilContext("Terraria.exe");

            AssemblyDefinition
                toInjectIn = c.PrimaryAssembly, // meh
                mscorlib   = c.DefinitionOf(typeof(int).Assembly);

            var console  = c.DefinitionOf(typeof(Console));
            var string_t = c.DefinitionOf(typeof(String ));

            MethodDefinition writeStrLine = console.Methods.Where(md => md.Name == "WriteLine" && md.Parameters.Count == 1 && md.Parameters[0].ParameterType == string_t).First();

            MethodInfo mainUpdate = new MethodInfo("Terraria.Main", "Update");
            Instruction[] hw = new[] // Console.WriteLine("Hello, world") in IL
            {
                // these instructions are hardcoded, but ILSnippetCompiler can be used as well (but snippets should be embedded resources or something, not directly embedded in the source)
                // references to prism and (unpatched) terraria must be passed to the compilesnippet(s) methods

                // ldstr "Hello, world"
                Instruction.Create(OpCodes.Ldstr, "Hello, world"),
                // call void [mscorlib]System.Console::WriteLine(string)
                Instruction.Create(OpCodes.Call , writeStrLine  )
            };

            ILInjector.Inject(toInjectIn, new InjectionData[]
            {
                // also see all XmlDoc in InjectionData.cs
                InjectionData.Method.NewMethodPre(mainUpdate, hw), // inject it in the very beginning of Terraria.Main.Update
                InjectionData.Instruction.NewInstructionIndex(mainUpdate, InjectionPosition.Post, hw, 5), // inject it after the 5th instruction
                InjectionData.Call.NewCall(mainUpdate, InjectionPosition.Post, hw, new MethodInfo("System.String", "Concat")) // inject it after the first occurence of a "String.Concat" call
            });

            toInjectIn.Write("foo.dll");
        }
    }
}
