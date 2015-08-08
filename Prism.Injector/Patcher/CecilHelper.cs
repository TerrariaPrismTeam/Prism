using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class CecilHelper
    {
        public static TypeDefinition CreateDelegate(CecilContext context, string @namespace, string name, TypeReference returnType, out MethodDefinition invoke, params TypeReference[] parameters)
        {
            var cResolver = context.Resolver;
            var typeSys = context.PrimaryAssembly.MainModule.TypeSystem;

            var delegateType = new TypeDefinition(@namespace, name, TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed, cResolver.ReferenceOf(typeof(MulticastDelegate)));

            var ctor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, typeSys.Void);
            ctor.IsRuntime = true;
            ctor.Parameters.Add(new ParameterDefinition("object", 0, typeSys.Object));
            ctor.Parameters.Add(new ParameterDefinition("method", 0, typeSys.IntPtr));

            delegateType.Methods.Add(ctor);

            invoke = new MethodDefinition("Invoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, typeSys.Void);
            invoke.IsRuntime = true;
            for (int i = 0; i < parameters.Length; i++)
                invoke.Parameters.Add(new ParameterDefinition("arg" + i, 0, parameters[i]));

            delegateType.Methods.Add(invoke);

            var beginInvoke = new MethodDefinition("BeginInvoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, cResolver.ReferenceOf(typeof(IAsyncResult)));
            beginInvoke.IsRuntime = true;
            for (int i = 0; i < parameters.Length; i++)
                beginInvoke.Parameters.Add(new ParameterDefinition("arg" + i, 0, parameters[i]));
            beginInvoke.Parameters.Add(new ParameterDefinition("callback", 0, cResolver.ReferenceOf(typeof(AsyncCallback))));
            beginInvoke.Parameters.Add(new ParameterDefinition("object", 0, typeSys.Object));

            delegateType.Methods.Add(beginInvoke);

            var endInvoke = new MethodDefinition("EndInvoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, typeSys.Void);
            endInvoke.IsRuntime = true;
            endInvoke.Parameters.Add(new ParameterDefinition("result", 0, cResolver.ReferenceOf(typeof(IAsyncResult))));

            delegateType.Methods.Add(endInvoke);

            context.PrimaryAssembly.MainModule.Types.Add(delegateType);

            return delegateType;
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
