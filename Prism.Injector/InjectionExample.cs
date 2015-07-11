using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector
{
    static class InjectionExample
    {
        static void Example()
        {
            // I'll write helper methods for this (reflection stuff -> cecil stuff) later
            AssemblyDefinition
                toInjectIn = null, // meh
                mscorlib = AssemblyDefinition.ReadAssembly(typeof(int).Assembly.Location);

            TypeDefinition console = mscorlib.MainModule.GetType("System.Console");
            TypeDefinition string_t = mscorlib.MainModule.GetType("System.String");
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
        }
    }
}
