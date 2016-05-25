using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    public static class WrapperHelperExtensions
    {
        readonly static SigComparer comp = new SigComparer(SigComparerOptions.PrivateScopeIsComparable);

        static TypeSig[]
            EmptyTRArr     = new TypeSig[0],
            SingletonTRArr = new TypeSig[1];

        /// <summary>
        /// Replaces all method references with the specified reference within the specified context.
        /// </summary>
        /// <param name="context">The current <see cref="DNContext"/>.</param>
        /// <param name="targetRef">The <see cref="MethodReference"/> to replace.</param>
        /// <param name="newRef">The <see cref="MethodReference"/> to replace targetRef with.</param>
        /// <param name="exitRecursion">Excludes recursive method calls from the replacement operation (may have undesired consequences with recursive methods).</param>
        public static void ReplaceAllMethodRefs(this MethodDef targetRef, MethodDef newRef, DNContext context, bool exitRecursion = true)
        {
            foreach (var tDef in context.PrimaryAssembly.ManifestModule.Types)
                foreach (var mDef in tDef.Methods)
                {
                    if (!mDef.HasBody) // abstract, runtime & external, etc
                        continue;

                    if (exitRecursion && mDef == newRef) // may have undesired consequences with recursive methods
                        continue;

                    foreach (var i in mDef.Body.Instructions)
                        if (i.Operand is MemberRef && ((MemberRef)i.Operand).IsMethodRef && comp.Equals(targetRef, (MemberRef)i.Operand))
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
        static string BuildArgTypesString(MethodDef method)
        {
            var types = method.Parameters.Select(p => p.Type);

            if (!method.IsStatic)
                types = new[] { method.DeclaringType.ToTypeSig() }.Concat(types);

            return String.Join("_", types.Select(r => r.TypeName));//r => r.IsPrimitive ? r.Name : r.FullName.Replace('.', '_').Replace("+", String.Empty)));
        }
        static string GetOverloadedName(MethodDef method)
        {
            var mtds = method.DeclaringType.GetMethods(method.Name);
            var safeName = GetSafeMethodName(method.Name);
            return mtds.Length == 1 ? safeName : safeName + "_" + BuildArgTypesString(method);
        }
        static string[] DefDelTypeName(MethodDef method)
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
        public static void Wrap(this MethodDef origMethod, DNContext context, string delegateNS, string delegateTypeName, string fieldName)
        {
            MethodDef invokeDelegate;

            //SingletonTRArr[0] = origMethod.DeclaringType.ToTypeSig();

            ////If anyone knows a better way to insert one element at the beginning of an array and scoot
            ////all the other elements down one index then go ahead and do it lol. I dunno how2array.
            var delegateArgs = origMethod.Parameters.Select(p => p.Type).ToArray();

            var newDelegateType = context.CreateDelegate(delegateNS, delegateTypeName, origMethod.ReturnType, out invokeDelegate, delegateArgs);

            var newMethod = origMethod.ReplaceAndHook(invokeDelegate, origMethod, fieldName);

            // you're not special anymore!
            if ((origMethod.Attributes & MethodAttributes.SpecialName) != 0)
                origMethod.Attributes ^= MethodAttributes.SpecialName;
            if ((origMethod.Attributes & MethodAttributes.RTSpecialName) != 0)
                origMethod.Attributes ^= MethodAttributes.RTSpecialName;

            origMethod.ReplaceAllMethodRefs(newMethod, context);
        }
        /// <summary>
        /// Wraps a method using a fancy delegate. Replaces all references of the method with the wrapped one and creates an "On[MethodName]" hook which passes the method's parent type followed by the type parameters of the original method.
        /// </summary>
        /// <param name="context">The current Cecil context.</param>
        /// <param name="origMethod">The method to wrap.</param>
        public static void Wrap(this MethodDef method, DNContext context)
        {
            var delTypeName = DefDelTypeName(method);
            method.Wrap(context, delTypeName[0], delTypeName[1], "P_On" + GetOverloadedName(method));
        }

        public static MethodDef ReplaceAndHook(this MethodDef toHook, MethodDef invokeHook, MethodDef realMethod, string fieldName)
        {
            //! no delegate type checking is done, runtime errors might happen if it doesn't match exactly
            //! here be dragons

            string origName = toHook.Name;
            var containing = toHook.DeclaringType;

            // create and add the hook delegate field
            var hookField = new FieldDefUser(fieldName, new FieldSig(invokeHook.DeclaringType.ToTypeSig()), FieldAttributes.Public | FieldAttributes.Static);
            containing.Fields.Add(hookField);

            // change the hooked method name
            toHook.Name = "Real" + GetSafeMethodName(toHook.Name);

            // create a fake method with the original name that calls the hook delegate field
            var isstatic = (toHook.Attributes & MethodAttributes.Static) != 0;
            var statAdd = isstatic ? 0 : 1;
            var newMethod = new MethodDefUser(origName,
                isstatic
                    ? MethodSig.CreateStatic  (toHook.ReturnType, toHook.Parameters.Select(p => p.Type).ToArray())
                    : MethodSig.CreateInstance(toHook.ReturnType, toHook.Parameters.Skip(1).Select(p => p.Type).ToArray()),
                toHook.Attributes);
            for (int i = statAdd; i < toHook.Parameters.Count; i++)
            {
                newMethod.Parameters[i].CreateParamDef();
                newMethod.Parameters[i].ParamDef.Name = toHook.Parameters[i].Name;
            }

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

            newMethod.Body = new CilBody();
            var ins = newMethod.Body.Instructions;
            using (var p = newMethod.Body.GetILProcessor())
            {
                Instruction VANILLA = OpCodes.Nop.ToInstruction();

                p.Emit(OpCodes.Ldsfld, hookField);
                p.Emit(OpCodes.Brfalse_S, VANILLA);

                p.Emit(OpCodes.Ldsfld, hookField);

                //ilproc.EmitWrapperCall(toHook);

                for (ushort i = 0; i < toHook.Parameters.Count /*- statAdd*/; i++)
                    p.Append(toHook.Parameters.GetLdargOf(i, false));

                p.Emit(OpCodes.Callvirt, invokeHook);

                p.Emit(OpCodes.Ret);

                p.Append(VANILLA);

                //ilproc.EmitWrapperCall(realMethod.Resolve());

                for (ushort i = 0; i < toHook.Parameters.Count /*- statAdd*/; i++)
                    p.Append(toHook.Parameters.GetLdargOf(i, false));

                p.Emit(OpCodes.Call, realMethod);

                p.Emit(OpCodes.Ret);
            }

            newMethod.Body.MaxStack = (ushort)(newMethod.Parameters.Count + 3 & 0xFFFF);

            toHook.DeclaringType.Methods.Add(newMethod);

            return newMethod;
        }
    }
}
