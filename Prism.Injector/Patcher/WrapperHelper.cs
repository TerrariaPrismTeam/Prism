using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class WrapperHelper
    {
        static TypeReference[]
            EmptyTRArr     = new TypeReference[0],
            SingletonTRArr = new TypeReference[1];

        /// <summary>
        /// Gets the ldarg instruction of the specified index using the smallest value type it can (because we're targeting the Sega Genesis and need to save memory).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        static Instruction GetLdargOf(ushort index, IList<ParameterDefinition> @params, bool isInstance = true)
        {
            int offset = isInstance ? 1 : 0;

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
                        return Instruction.Create(OpCodes.Ldarg_S, @params[index - offset]);
                    //Y U NO HAVE USHORT
                    return Instruction.Create(OpCodes.Ldarg, @params[index]);
            }
        }

        /// <summary>
        /// Replaces all method references with the specified reference within the specified context.
        /// </summary>
        /// <param name="context">The current <see cref="CecilContext"/>.</param>
        /// <param name="targetRef">The <see cref="MethodReference"/> to replace.</param>
        /// <param name="newRef">The <see cref="MethodReference"/> to replace targetRef with.</param>
        /// <param name="exitRecursion">Excludes recursive method calls from the replacement operation (may have undesired consequences with recursive methods).</param>
        public static void ReplaceAllMethodRefs(CecilContext context, MethodReference targetRef, MethodReference newRef, bool exitRecursion = true)
        {
            foreach (TypeDefinition tDef in context.PrimaryAssembly.MainModule.Types)
                foreach (MethodDefinition mDef in tDef.Methods)
                {
                    if (!mDef.HasBody) // abstract, runtime & external, etc
                        continue;

                    if (exitRecursion && mDef == newRef) // may have undesired consequences with recursive methods
                        continue;

                    foreach (Instruction i in mDef.Body.Instructions)
                        if (i.Operand == targetRef)
                            i.Operand = newRef;
                }
        }

        static string[] DefDelTypeName(TypeDefinition typeDef, string methodName)
        {
            return new[] { "Terraria.PrismInjections", typeDef.Name + "_" + methodName + "Delegate" };
        }
        /// <summary>
        /// Wraps a method using a fancy delegate. Replaces all references of the method with the wrapped one and creates an "On[MethodName]" hook which passes the method's parent type followed by the type parameters of the original method.
        /// </summary>
        /// <param name="context">The current Cecil context.</param>
        /// <param name="delegateNS">The namespace of the delegate type to create.</param>
        /// <param name="delegateTypeName">The name of the delegate type to create.</param>
        /// <param name="origMethod">The method to wrap.</param>
        public static void WrapMethod(CecilContext context, string delegateNS, string delegateTypeName, MethodDefinition origMethod)
        {
            MethodDefinition invokeDelegate;

            SingletonTRArr[0] = origMethod.DeclaringType;

            //If anyone knows a better way to insert one element at the beginning of an array and scoot
            //all the other elements down one index then go ahead and do it lol. I dunno how2array.
            var delegateArgs = (origMethod.IsStatic ? EmptyTRArr : SingletonTRArr).Concat(origMethod.Parameters.Select(p => p.ParameterType)).ToArray();

            var newDelegateType = context.CreateDelegate(delegateNS, delegateTypeName, origMethod.ReturnType, out invokeDelegate, delegateArgs);

            var newMethod = ReplaceAndHook(origMethod, invokeDelegate);

            ReplaceAllMethodRefs(context, origMethod, newMethod);
        }
        /// <summary>
        /// Wraps a method using a fancy delegate. Replaces all references of the method with the wrapped one and creates an "On[MethodName]" hook which passes the method's parent type followed by the type parameters of the original method.
        /// </summary>
        /// <param name="context">The current Cecil context.</param>
        /// <param name="origMethod">The method to wrap.</param>
        public static void WrapMethod(CecilContext context, MethodDefinition method)
        {
            var delTypeName = DefDelTypeName(method.DeclaringType, method.Name);
            WrapMethod(context, delTypeName[0], delTypeName[1], method);
        }

        public static MethodDefinition ReplaceAndHook(MethodDefinition toHook, MethodReference invokeHook)
        {
            //! no delegate type checking is done, runtime errors might happen if it doesn't match exactly

            string origName = toHook.Name;
            var containing = toHook.DeclaringType;

            // create and add the hook delegate field
            var hookField = new FieldDefinition("P_On" + origName, FieldAttributes.Public | FieldAttributes.Static, invokeHook.DeclaringType);
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

            for (ushort i = 0; i < toHook.Parameters.Count + (toHook.IsStatic ? 0 : 1); i++)
                ilproc.Append(GetLdargOf(i, toHook.Parameters, !toHook.IsStatic));

            ilproc.Emit(OpCodes.Callvirt, invokeHook);

            ilproc.Emit(OpCodes.Ret);

            newMethod.Body.MaxStackSize = newMethod.Parameters.Count + 2;

            toHook.DeclaringType.Methods.Add(newMethod);

            return newMethod;
        }
    }
}
