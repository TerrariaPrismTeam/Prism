using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class CecilHelper
    {
        public static TypeDefinition CreateDelegate(CecilContext c, string ns, string name, TypeReference returnType, out MethodDefinition invoke, params TypeReference[] parameters)
        {
            var r = c.Resolver;
            var ts = c.PrimaryAssembly.MainModule.TypeSystem;

            var ret = new TypeDefinition(ns, name, TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed, r.ReferenceOf(typeof(MulticastDelegate)));

            var ctor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, ts.Void);
            ctor.IsRuntime = true;
            ctor.Parameters.Add(new ParameterDefinition("object", 0, ts.Object));
            ctor.Parameters.Add(new ParameterDefinition("method", 0, ts.IntPtr));

            ret.Methods.Add(ctor);

            invoke = new MethodDefinition("Invoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, ts.Void);
            invoke.IsRuntime = true;
            for (int i = 0; i < parameters.Length; i++)
                invoke.Parameters.Add(new ParameterDefinition("arg" + i, 0, parameters[i]));

            ret.Methods.Add(invoke);

            var beginInvoke = new MethodDefinition("BeginInvoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, r.ReferenceOf(typeof(IAsyncResult)));
            beginInvoke.IsRuntime = true;
            for (int i = 0; i < parameters.Length; i++)
                beginInvoke.Parameters.Add(new ParameterDefinition("arg" + i, 0, parameters[i]));
            beginInvoke.Parameters.Add(new ParameterDefinition("callback", 0, r.ReferenceOf(typeof(AsyncCallback))));
            beginInvoke.Parameters.Add(new ParameterDefinition("object", 0, ts.Object));

            ret.Methods.Add(beginInvoke);

            var endInvoke = new MethodDefinition("EndInvoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, ts.Void);
            endInvoke.IsRuntime = true;
            endInvoke.Parameters.Add(new ParameterDefinition("result", 0, r.ReferenceOf(typeof(IAsyncResult))));

            ret.Methods.Add(endInvoke);

            c.PrimaryAssembly.MainModule.Types.Add(ret);

            return ret;
        }
        public static Instruction FindInstructionSeq(MethodBody body, OpCode[] instrs)
        {
            for (int i = 0; i < body.Instructions.Count - instrs.Length; i++)
            {
                for (int j = 0; j < instrs.Length; j++)
                {
                    if (body.Instructions[i + j].OpCode.Code != instrs[j].Code)
                        goto next_try;
                }

                return body.Instructions[i];
            next_try:
                ;
            }

            return null;
        }
        public static void RemoveInstructions(ILProcessor p, Instruction first, int count)
        {
            var cur = first;
            for (int i = 0; i < count; i++)
            {
                if (cur == null)
                    break;

                var n = cur.Next;
                p.Remove(cur);
                cur = n;
            }
        }
    }
}
