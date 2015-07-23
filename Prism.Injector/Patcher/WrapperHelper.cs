using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class WrapperHelper
    {
        // gets the 'ldarg' instruction that requries the lowest possible memory
        static Instruction GetLdargOf(ushort index)
        {
            switch (index)
            {
                case 0:
                    return Instruction.Create(OpCodes.Ldarg_0);
                case 1:
                    return Instruction.Create(OpCodes.Ldarg_1);
                case 2:
                    return Instruction.Create(OpCodes.Ldarg_2);
                case 3:
                    return Instruction.Create(OpCodes.Ldarg_3);
                default:
                    if (index <= Byte.MaxValue)
                        return Instruction.Create(OpCodes.Ldarg_S, (byte)index);

                    return Instruction.Create(OpCodes.Ldarg, /* int */ index);
            }
        }

        public static void ReplaceAllMethodRefs(CecilContext c, MethodReference tar, MethodReference @new)
        {
            foreach (TypeDefinition t in c.PrimaryAssembly.MainModule.Types)
                foreach (MethodDefinition m in t.Methods)
                {
                    if (!m.HasBody) // abstract, runtime & external, etc
                        continue;

                    foreach (Instruction i in m.Body.Instructions)
                        if (i.Operand == tar)
                            i.Operand = @new;
                }
        }

        public static MethodDefinition ReplaceAndHook(MethodDefinition toHook, MethodReference invokeHook)
        {
            //! no delegate type checking is done, runtime errors might happen if it doesn't match exactly

            string origName = toHook.Name;
            var containing = toHook.DeclaringType;

            // create and add the hook delegate field
            var hookField = new FieldDefinition("On" + origName, FieldAttributes.Public | FieldAttributes.Static, invokeHook.DeclaringType);
            containing.Fields.Add(hookField);

            // change the hooked method name
            toHook.Name = "Real" + toHook.Name;

            // create a fake method with the original name that calls the hook delegate field
            var newMethod = new MethodDefinition(origName, toHook.Attributes, toHook.ReturnType);
            foreach (var p in toHook.Parameters)
                newMethod.Parameters.Add(p);

            var ilproc = newMethod.Body.GetILProcessor();

            //<hookField>(this, <args>);
            /*
            ldsfld class <hookDelegateType> <hookField>
            ldarg.0 // this
            ldarg.1
            ..
            ldarg.n
            callvirt instance void <hookDelegateType>::Invoke(<args>)

            ret
            */

            ilproc.Emit(OpCodes.Ldsfld, hookField);

            for (ushort i = 0; i <= toHook.Parameters.Count; i++)
                ilproc.Append(GetLdargOf(i));

            ilproc.Emit(OpCodes.Callvirt, invokeHook);

            ilproc.Emit(OpCodes.Ret);

            newMethod.Body.MaxStackSize = newMethod.Parameters.Count + 2;

            toHook.DeclaringType.Methods.Add(newMethod);

            return newMethod;
        }
    }
}
