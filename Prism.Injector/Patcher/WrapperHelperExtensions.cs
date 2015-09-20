using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class WrapperHelperExtensions
    {
        static TypeReference[]
            EmptyTRArr     = new TypeReference[0],
            SingletonTRArr = new TypeReference[1];

        /// <summary>
        /// Replaces all method references with the specified reference within the specified context.
        /// </summary>
        /// <param name="context">The current <see cref="CecilContext"/>.</param>
        /// <param name="targetRef">The <see cref="MethodReference"/> to replace.</param>
        /// <param name="newRef">The <see cref="MethodReference"/> to replace targetRef with.</param>
        /// <param name="exitRecursion">Excludes recursive method calls from the replacement operation (may have undesired consequences with recursive methods).</param>
        public static void ReplaceAllMethodRefs(this MethodReference targetRef, MethodReference newRef, CecilContext context, bool exitRecursion = true)
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

        static string GetSafeMethodName(string methodName)
        {
            // common compiler-generated names, '.' is from the compiled name of the ctor
            return methodName
                .Replace('.', '_')
                .Replace('<', '_')
                .Replace('>', '_')
                .Replace('$', '_')
                .Replace(' ', '_');
        }
        static string BuildArgTypesString(MethodDefinition method)
        {
            var types = method.Parameters.Select(p => p.ParameterType);

            if (!method.IsStatic)
                types = new[] { method.DeclaringType }.Concat(types);

            return String.Join("_", types.Select(r => r.Name));//r => r.IsPrimitive ? r.Name : r.FullName.Replace('.', '_').Replace("+", String.Empty)));
        }
        static string GetOverloadedName(MethodDefinition method)
        {
            var mtds = method.DeclaringType.GetMethods(method.Name);
            var safeName = GetSafeMethodName(method.Name);
            return mtds.Length == 1 ? safeName : safeName + "_" + BuildArgTypesString(method);
        }
        static string[] DefDelTypeName(MethodDefinition method)
        {
            return new[] { "Terraria.PrismInjections", method.DeclaringType.Name + "_" + GetOverloadedName(method) + "Delegate" };
        }
        /// <summary>
        /// Wraps a method using a fancy delegate. Replaces all references of the method with the wrapped one and creates an "On[MethodName]" hook which passes the method's parent type followed by the type parameters of the original method.
        /// </summary>
        /// <param name="context">The current Cecil context.</param>
        /// <param name="delegateNS">The namespace of the delegate type to create.</param>
        /// <param name="delegateTypeName">The name of the delegate type to create.</param>
        /// <param name="origMethod">The method to wrap.</param>
        public static void Wrap(this MethodDefinition origMethod, CecilContext context, string delegateNS, string delegateTypeName, string fieldName)
        {
            MethodDefinition invokeDelegate;

            SingletonTRArr[0] = origMethod.DeclaringType;

            //If anyone knows a better way to insert one element at the beginning of an array and scoot
            //all the other elements down one index then go ahead and do it lol. I dunno how2array.
            var delegateArgs = (origMethod.IsStatic ? EmptyTRArr : SingletonTRArr).Concat(origMethod.Parameters.Select(p => p.ParameterType)).ToArray();

            var newDelegateType = context.CreateDelegate(delegateNS, delegateTypeName, origMethod.ReturnType, out invokeDelegate, delegateArgs);

            var newMethod = origMethod.ReplaceAndHook(invokeDelegate, origMethod, fieldName);

            // you're not special anymore!
            if ((origMethod.Attributes & MethodAttributes.  SpecialName) != 0)
                origMethod.Attributes ^= MethodAttributes.  SpecialName;
            if ((origMethod.Attributes & MethodAttributes.RTSpecialName) != 0)
                origMethod.Attributes ^= MethodAttributes.RTSpecialName;

            origMethod.ReplaceAllMethodRefs(newMethod, context);
        }
        /// <summary>
        /// Wraps a method using a fancy delegate. Replaces all references of the method with the wrapped one and creates an "On[MethodName]" hook which passes the method's parent type followed by the type parameters of the original method.
        /// </summary>
        /// <param name="context">The current Cecil context.</param>
        /// <param name="origMethod">The method to wrap.</param>
        public static void Wrap(this MethodDefinition method, CecilContext context)
        {
            var delTypeName = DefDelTypeName(method);
            method.Wrap(context, delTypeName[0], delTypeName[1], "P_On" + GetOverloadedName(method));
        }

        public static MethodDefinition ReplaceAndHook(this MethodDefinition toHook, MethodReference invokeHook, MethodReference realMethod, string fieldName)
        {
            //! no delegate type checking is done, runtime errors might happen if it doesn't match exactly
            //! here be dragons

            string origName = toHook.Name;
            var containing = toHook.DeclaringType;

            // create and add the hook delegate field
            var hookField = new FieldDefinition(fieldName, FieldAttributes.Public | FieldAttributes.Static, invokeHook.DeclaringType);
            containing.Fields.Add(hookField);

            // change the hooked method name
            toHook.Name = "Real" + GetSafeMethodName(toHook.Name);

            // create a fake method with the original name that calls the hook delegate field
            var newMethod = new MethodDefinition(origName, toHook.Attributes, toHook.ReturnType);
            foreach (var p in toHook.Parameters)
                newMethod.Parameters.Add(p);

            var ilproc = newMethod.Body.GetILProcessor();

            //if (<hookField> != null) return <hookField>((this,)? <args>); else return (this.)?<realMethod>(<args>);

            /*
            ldsfld class <hookDelegateType> <hookField>
            brfalse.s VANILLA // if (<hookField> == null) goto VANILLA;

            ldsfld class <hookDelegateType> <hookField>
            ldarg.0 // this (if instance)
            ldarg.1
            ..
            ldarg.n
            callvirt instance <retval> <hookDelegateType>::Invoke(<args>)

            ret

        VANILLA:
            nop (for ease of adding the params afterwards)

            ldarg.0 // this (if instance)
            ldarg.1
            ..
            ldarg.n
            call instance? <retval> <realMethod>(<args>)

            ret
            */

            var VANILLA = ilproc.Create(OpCodes.Nop);

            ilproc.Emit(OpCodes.Ldsfld, hookField);
            ilproc.Emit(OpCodes.Brfalse_S, VANILLA);

            ilproc.Emit(OpCodes.Ldsfld, hookField);

            //ilproc.EmitWrapperCall(toHook);

            for (ushort i = 0; i < toHook.Parameters.Count + (toHook.IsStatic ? 0 : 1); i++)
                ilproc.Append(toHook.Parameters.GetLdargOf(i, !toHook.IsStatic));

            ilproc.Emit(OpCodes.Callvirt, invokeHook);

            ilproc.Emit(OpCodes.Ret);

            ilproc.Append(VANILLA);

            //ilproc.EmitWrapperCall(realMethod.Resolve());

            for (ushort i = 0; i < toHook.Parameters.Count + (toHook.IsStatic ? 0 : 1); i++)
                ilproc.Append(toHook.Parameters.GetLdargOf(i, !toHook.IsStatic));

            ilproc.Emit(OpCodes.Call, realMethod);

            ilproc.Emit(OpCodes.Ret);

            newMethod.Body.MaxStackSize = newMethod.Parameters.Count + 3;

            toHook.DeclaringType.Methods.Add(newMethod);

            return newMethod;
        }
    }
}
