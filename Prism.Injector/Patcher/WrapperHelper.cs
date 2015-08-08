using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class WrapperHelper
    {
        /// <summary>
        /// Gets the ldarg instruction of the specified index using the smallest value type it can (because we're targeting the Sega Genesis and need to save memory).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
                    //Y U NO HAVE USHORT
                    return Instruction.Create(OpCodes.Ldarg, /* int */ index);
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

        public static string[] DefDelTypeName(TypeDefinition typeDef, string methodName)
        {
            return new[] { "Terraria.PrismInjections", typeDef.Name + "_" + methodName + "Delegate" };
        }        

        /// <summary>
        /// Wraps a method using a fancy delegate. Replaces all references of the method with the wrapped one and creates an "On[MethodName]" hook which passes the method's parent type followed by the type parameters of the original method 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="typeDef"></param>
        /// <param name="methodName"></param>
        /// <param name="methodFlags"></param>
        /// <param name="delegateNS"></param>
        /// <param name="delegateTypeName"></param>
        /// <param name="returnType"></param>
        /// <param name="args"></param>
        public static void WrapInstanceMethod(CecilContext context, string delegateNS, string delegateTypeName, TypeDefinition typeDef, TypeReference returnType, string methodName, MethodFlags methodFlags = MethodFlags.All, params TypeReference[] args)
        {
            MethodDefinition invokeDelegate;

            //If anyone knows a better way to insert one element at the beginning of an array and scoot 
            //all the other elements down one index then go ahead and do it lol. I dunno how2array.
            var delegateArgs = (new TypeReference[] { typeDef }).Concat(args).ToArray();

            var newDelegateType = CecilHelper.CreateDelegate(context, delegateNS, delegateTypeName, returnType, out invokeDelegate, delegateArgs);

            var origMethod = typeDef.GetMethod(methodName, methodFlags, args);

            var newMethod = WrapperHelper.ReplaceAndHook(origMethod, invokeDelegate);

            WrapperHelper.ReplaceAllMethodRefs(context, origMethod, newMethod);
        }

        //TODO: Finish the XmlDoc for the original method then copy it to this overload (underload?)...
        public static void WrapInstanceMethod(CecilContext context, TypeDefinition typeDef, TypeReference returnType, string methodName, MethodFlags methodFlags = MethodFlags.All, params TypeReference[] args)
        {
            var delTypeName = DefDelTypeName(typeDef, methodName);
            WrapInstanceMethod(context, delTypeName[0], delTypeName[1], typeDef, returnType, methodName, methodFlags, args);
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
