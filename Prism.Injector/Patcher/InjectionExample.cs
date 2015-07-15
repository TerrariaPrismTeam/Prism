using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class InjectionExample
    {
        static CecilContext   c;
        static MemberResolver r;

        public static void Load()
        {
            c = new CecilContext("Terraria.exe");
            r = c.Resolver; // this contains a lot of useful methods
        }
        public static void Emit()
        {
            // Newtonsoft.Json.dll and Steamworks.NET.dll are required to write the assembly
            c.PrimaryAssembly.Write("Prism.Terraria.dll");
        }

        public static void Example  ()
        {
            var toInjectIn = c.PrimaryAssembly;

            var main = r.GetType("Terraria.Main");

            var writeStrLine = r.MethodOfA<string>(Console.WriteLine);
            var newText = main.GetMethod("NewText");

            // get the update method
            var mainUpdate = main.GetMethod("Update");


            // Console.WriteLine("Hello, world") in IL
            var hw_stdout = new[]
            {
                // these instructions are hardcoded, but ILSnippetCompiler can be used as well (but snippets should be embedded resources or something, not directly embedded in the source)
                // references to prism and (unpatched) terraria must be passed to the compilesnippet(s) methods
                // but this can be easier in some places (and definitely faster)

                // ldstr "Hello, world"
                Instruction.Create(OpCodes.Ldstr, "Hello, world"),
                // call void [mscorlib]System.Console::WriteLine(string)
                Instruction.Create(OpCodes.Call , writeStrLine  )
            };
            // Main.NewText("Hello, world", 255, 255, 255, false) in IL
            // optional arguments must be explicit (and aren't optional) in IL
            var hw_ingame = new[]
            {
                // ldstr "Hello, world"
                Instruction.Create(OpCodes.Ldstr, "Hello, world"),
                // ldc.i4 255
                Instruction.Create(OpCodes.Ldc_I4, 255),
                // ldc.i4 255
                Instruction.Create(OpCodes.Ldc_I4, 255),
                // ldc.i4 255
                Instruction.Create(OpCodes.Ldc_I4, 255),
                // ldc.i4.0
                Instruction.Create(OpCodes.Ldc_I4_0),

                // call void [Terraria]Terraria.Main::NewText(string, byte, byte, byte, bool)
                Instruction.Create(OpCodes.Call , newText  )
            };

            ILInjector.Inject(toInjectIn, new InjectionData[]
            {
                // this injects 'hello world' in 3 places
                // (also see all XmlDoc in InjectionData.cs)

                // inject it in the very beginning of Main.Update
                InjectionData.Method.NewMethodPre(mainUpdate, hw_stdout),
                // inject it after the 5th instruction
                InjectionData.Instruction.NewInstructionIndex(mainUpdate, InjectionPosition.Post, hw_ingame, 5),

                // inject it after the first occurence of a "String.Concat(object[])" call
                // this throws an exception, because there is no String.Concat call in Update, but I hope you get how it works
                //InjectionData.Call.NewCall(mainUpdate, InjectionPosition.Post, hw, r.MethodOfF<object[], string>(String.Concat))
            });
        }
    }
}
