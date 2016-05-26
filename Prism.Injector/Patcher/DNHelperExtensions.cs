using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    public static class DNHelperExtensions
    {
        readonly static SigComparer comp = new SigComparer(SigComparerOptions.PrivateScopeIsComparable);

        /// <summary>
        /// Gets the ldarg instruction of the specified index using the smallest value type it can (because we're targeting the Sega Genesis and need to save memory).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static Instruction GetLdargOf(this IList<Parameter> @params, ushort index, bool isInstance = true)
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

        static bool CodeEqIgnoreS(Code a, Code b)
        {
            if (a == b)
                return true;

            switch (a)
            {
                case Code.Beq:
                case Code.Beq_S:
                    return b == Code.Beq || b == Code.Beq_S;
                case Code.Bge:
                case Code.Bge_S:
                    return b == Code.Bge || b == Code.Bge_S;
                case Code.Ble:
                case Code.Ble_S:
                    return b == Code.Ble || b == Code.Ble_S;
                case Code.Bgt:
                case Code.Bgt_S:
                    return b == Code.Bgt || b == Code.Bgt_S;
                case Code.Blt:
                case Code.Blt_S:
                    return b == Code.Blt || b == Code.Blt_S;
                case Code.Bge_Un:
                case Code.Bge_Un_S:
                    return b == Code.Bge_Un || b == Code.Bge_Un_S;
                case Code.Ble_Un:
                case Code.Ble_Un_S:
                    return b == Code.Ble_Un || b == Code.Ble_Un_S;
                case Code.Bgt_Un:
                case Code.Bgt_Un_S:
                    return b == Code.Bgt_Un || b == Code.Bgt_Un_S;
                case Code.Blt_Un:
                case Code.Blt_Un_S:
                    return b == Code.Blt_Un || b == Code.Blt_Un_S;
                case Code.Bne_Un:
                case Code.Bne_Un_S:
                    return b == Code.Bne_Un || b == Code.Bne_Un_S;
                case Code.Brfalse:
                case Code.Brfalse_S:
                    return b == Code.Brfalse || b == Code.Brfalse_S;
                case Code.Brtrue:
                case Code.Brtrue_S:
                    return b == Code.Brtrue || b == Code.Brtrue_S;
                case Code.Br:
                case Code.Br_S:
                    return b == Code.Br || b == Code.Br_S;
                case Code.Ldarg:
                case Code.Ldarg_S:
                    return b == Code.Ldarg || b == Code.Ldarg_S;
                case Code.Ldarga:
                case Code.Ldarga_S:
                    return b == Code.Ldarga || b == Code.Ldarga_S;
                case Code.Ldc_I4:
                case Code.Ldc_I4_S:
                    return b == Code.Ldc_I4 || b == Code.Ldc_I4_S;
                case Code.Ldloc:
                case Code.Ldloc_S:
                    return b == Code.Ldloc || b == Code.Ldloc_S;
                case Code.Ldloca:
                case Code.Ldloca_S:
                    return b == Code.Ldloca || b == Code.Ldloca_S;
                case Code.Leave:
                case Code.Leave_S:
                    return b == Code.Leave || b == Code.Leave_S;
                case Code.Starg:
                case Code.Starg_S:
                    return b == Code.Starg || b == Code.Starg_S;
                case Code.Stloc:
                case Code.Stloc_S:
                    return b == Code.Stloc || b == Code.Stloc_S;
            }

            return false;
        }

        public static TypeDef CreateDelegate(this DNContext context, string @namespace, string name, TypeSig returnType, out MethodDef invoke, params TypeSig[] parameters)
        {
            var cResolver = context.Resolver;
            var typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;

            var delegateType = new TypeDefUser(@namespace, name, cResolver.ReferenceOf(typeof(MulticastDelegate)));
            delegateType.Attributes = TypeAttributes.Public /*| TypeAttributes.AutoClass*/ | TypeAttributes.Sealed;

            var ctor = new MethodDefUser(".ctor", MethodSig.CreateInstance(typeSys.Void, typeSys.Object, typeSys.IntPtr),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            ctor.ImplAttributes |= MethodImplAttributes.Runtime;
            // param 0 is 'this'
            ctor.Parameters[1].CreateParamDef();
            ctor.Parameters[1].ParamDef.Name = "object";
            ctor.Parameters[2].CreateParamDef();
            ctor.Parameters[2].ParamDef.Name = "method";

            delegateType.Methods.Add(ctor);

            invoke = new MethodDefUser("Invoke", MethodSig.CreateInstance(returnType, parameters), MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
            invoke.ImplAttributes |= MethodImplAttributes.Runtime;
            for (int i = 1; i <= parameters.Length; i++)
            {
                invoke.Parameters[i].CreateParamDef();
                invoke.Parameters[i].ParamDef.Name = "arg" + i;
            }

            delegateType.Methods.Add(invoke);

            var beginInvoke = new MethodDefUser("BeginInvoke", MethodSig.CreateInstance(cResolver.ReferenceOf(typeof(IAsyncResult)).ToTypeSig(),
                    parameters.Concat(new[] { cResolver.ReferenceOf(typeof(AsyncCallback)).ToTypeSig(), typeSys.Object }).ToArray()),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
            beginInvoke.ImplAttributes |= MethodImplAttributes.Runtime;
            for (int i = 1; i < parameters.Length - 1; i++)
            {
                beginInvoke.Parameters[i].CreateParamDef();
                beginInvoke.Parameters[i].ParamDef.Name = "arg" + i;
            }
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 2].CreateParamDef();
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 2].ParamDef.Name = "callback";
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 1].CreateParamDef();
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 1].ParamDef.Name = "object"  ;

            delegateType.Methods.Add(beginInvoke);

            var endInvoke = new MethodDefUser("EndInvoke", MethodSig.CreateInstance(typeSys.Void, cResolver.ReferenceOf(typeof(IAsyncResult)).ToTypeSig()),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
            beginInvoke.ImplAttributes |= MethodImplAttributes.Runtime;
            endInvoke.Parameters[1].CreateParamDef();
            endInvoke.Parameters[1].ParamDef.Name = "result";

            delegateType.Methods.Add(endInvoke);

            context.PrimaryAssembly.ManifestModule.Types.Add(delegateType);

            return delegateType;
        }
        public static Instruction[] FindInstrSeq(this CilBody body, ILProcessor p, OpCode[] instrs)
        {
            return body.FindInstrSeq(p, instrs, instrs.Length);
        }
        public static Instruction[] FindInstrSeq(this CilBody body, ILProcessor p, OpCode[] instrs, int amt)
        {
            Instruction[] result = new Instruction[amt];

            for (int i = 0; i < result.Length; i++)
                result[i] = i == 0 ? body.FindInstrSeqStart(instrs) : result[i - 1] != null ? result[i - 1].Next(p) : null;

            return result;
        }
        public static Instruction FindInstrSeqStart(this CilBody body, OpCode[] instrs, int startIndex = 0)
        {
            for (int i = startIndex; i <= body.Instructions.Count - instrs.Length; i++)
            {
                for (int j = 0; j < instrs.Length; j++)
                {
                    if (!CodeEqIgnoreS(body.Instructions[i + j].OpCode.Code, instrs[j].Code))
                        goto next_try;
                }

                return body.Instructions[i];
            next_try:
                ;
            }

            return null;
        }
        public static void ReplaceInstructions(this ILProcessor p, IEnumerable<Instruction> orig, IEnumerable<Instruction> repl)
        {
            if (!orig.Chronological(null, i => i.Next(p)) || !repl.Chronological(null, i => i.Next(p)))
                Console.Error.WriteLine("Error: Both sequences in DNHelperExtensions.ReplaceInstructions(ILProcessor, IEnumerable<Instruction>, IEnumerable<Instruction>) must be chronological.");

            Instruction firstOrig = orig.First();

            foreach (var i in repl)
                p.InsertBefore(firstOrig, i);

            p.RemoveInstructions(orig);
        }
        public static Instruction RemoveInstructions(this ILProcessor p, IEnumerable<Instruction> instrs)
        {
            Instruction n = null;
            foreach (var i in instrs)
            {
                n = i.Next(p);
                p.Remove(i);
            }

            return n;
        }
        public static Instruction RemoveInstructions(this ILProcessor p, Instruction first, int count)
        {
            var cur = first;
            for (int i = 0; i < count; i++)
            {
                if (cur == null)
                    break;

                var n = cur.Next(p);
                p.Remove(cur);
                cur = n;
            }

            return cur;
        }
        // callee must have the same args as the calling method
        // if the callee is an instance method, the object must be placed on the stack first
        public static void EmitWrapperCall(this ILProcessor proc, MethodDef toCall, Instruction before = null)
        {
            //var caller = proc.Body.Method;

            for (ushort i = 0; i < toCall.Parameters.Count - (toCall.IsStatic ? 0 : 1); i++)
                if (before == null)
                    proc.Append(toCall.Parameters.GetLdargOf(i, /*!toCall.IsStatic*/false));
                else
                    proc.InsertBefore(before, toCall.Parameters.GetLdargOf(i, /*!toCall.IsStatic*/false));

            var c = toCall.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;
            if (before == null)
                proc.Emit(c, toCall);
            else
                proc.InsertBefore(before, Instruction.Create(c, toCall));
        }

        static TypeSig ICast/* needs a better name */(TypeSig a, TypeSig b, ICorLibTypes ts)
        {
            if (comp.Equals(a, b))
                return a;
            if (comp.Equals(a, ts.String))
                return a;
            if (comp.Equals(b, ts.String))
                return b;
            if (comp.Equals(a, ts.IntPtr) || comp.Equals(a, ts.UIntPtr))
                return a;
            if (comp.Equals(b, ts.IntPtr) || comp.Equals(b, ts.UIntPtr))
                return b;
            if (comp.Equals(a, ts.Double))
                return a;
            if (comp.Equals(b, ts.Double))
                return b;
            if (comp.Equals(a, ts.Single))
                return a;
            if (comp.Equals(b, ts.Single))
                return b;
            if (comp.Equals(a, ts.Int64) || comp.Equals(a, ts.UInt64))
                return a;
            if (comp.Equals(b, ts.Int64) || comp.Equals(b, ts.UInt64))
                return b;

            if (!a.IsByRef || !b.IsByRef)
                return ts.Object;

            var ad = a.ToTypeDefOrRef().ResolveTypeDefThrow();
            var bd = b.ToTypeDefOrRef().ResolveTypeDefThrow();

            if ((!ad.IsSequentialLayout || !ad.IsExplicitLayout) && (!bd.IsSequentialLayout || !bd.IsExplicitLayout))
                return ts.IntPtr;

            // close enough
            return ad.PackingSize > bd.PackingSize ? a : b;
        }
        // NOTE: stack may contain an int/uint when the actual C# type is a byte/..., because most structural primitives are all (u)ints in IL
        // NOTE: not sure if it would work correctly with branching
        public static void EnumerateWithStackAnalysis(this MethodDef method, Action<Instruction, Stack<Tuple<TypeSig, Instruction>>> cb)
        {
            if (cb == null)
                throw new ArgumentNullException("cb");

            var body = method.Body;
            var ts = method.Module.CorLibTypes;

            var stack = new Stack<Tuple<TypeSig, Instruction>>(body.MaxStack);
            Func<TypeSig> pop = () => stack.Pop().Item1;

            var ins = body.Instructions;
            for (int i = 0; i < ins.Count; i++)
            {
                var n = ins[i];
                var c = n.OpCode.Code;

                Action<TypeSig> push = tr => stack.Push(Tuple.Create(tr, n));

                cb(n, stack);

                #region huge switch
                switch (c)
                {
                    case Code.Add:
                    case Code.Add_Ovf:
                    case Code.And:
                    case Code.Div:
                    case Code.Div_Un:
                    case Code.Mul:
                    case Code.Mul_Ovf:
                    case Code.Mul_Ovf_Un:
                    case Code.Or:
                    case Code.Rem:
                    case Code.Rem_Un:
                    case Code.Shl:
                    case Code.Shr:
                    case Code.Shr_Un:
                    case Code.Sub:
                    case Code.Sub_Ovf:
                    case Code.Sub_Ovf_Un:
                    case Code.Xor:
                        push(ICast(pop(), pop(), ts));
                        break;
                    case Code.Neg:
                    case Code.Not:
                        push(pop());
                        break;
                    case Code.Newarr:
                        push(((ITypeDefOrRef)n.Operand).ToTypeSig());
                        break;
                    case Code.Arglist:
                        throw new NotSupportedException();
                    case Code.Box:
                        stack.Pop();
                        push(ts.Object);
                        break;
                    case Code.Call:
                    case Code.Calli:
                    case Code.Callvirt:
                    case Code.Newobj:
                        {
                            if (n.Operand is MemberRef)
                            {
                                var mr = (MemberRef)n.Operand;

                                for (int j = n.OpCode.Code == Code.Newobj ? 1 : 0; j < mr.MethodSig.Params.Count; j++)
                                    stack.Pop();

                                push(mr.MethodSig.RetType);
                            }
                            else if (n.Operand is MethodSpec)
                            {
                                var ms = (MethodSpec)n.Operand;

                                for (int j = n.OpCode.Code == Code.Newobj ? 1 : 0; j < ms.Method.MethodSig.Params.Count; j++)
                                    stack.Pop();

                                push(ms.Method.MethodSig.RetType);
                            }
                            else if (n.Operand is MethodDef)
                            {
                                var md = (MethodDef)n.Operand;

                                for (int j = n.OpCode.Code == Code.Newobj ? 1 : 0; j < md.Parameters.Count; j++)
                                    stack.Pop();

                                push(md.ReturnType);
                            }
                            else
                            {
                                //! PLACE BREAKPOINT HERE
                                int iii = 0;
                                iii = ++iii - 1;
                            }
                        }
                        break;
                    case Code.Castclass:
                    case Code.Constrained:
                        if (c == Code.Castclass)
                            stack.Pop();

                        push(((ITypeDefOrRef)n.Operand).ToTypeSig());
                        break;
                    case Code.Ceq:
                    case Code.Cgt:
                    case Code.Cgt_Un:
                    case Code.Ckfinite:
                    case Code.Clt:
                    case Code.Clt_Un:
                    case Code.Isinst:
                        if (c != Code.Ckfinite && c != Code.Isinst)
                            stack.Pop();
                        stack.Pop();

                        push(ts.Boolean);
                        break;
                    case Code.Conv_I:
                    case Code.Conv_Ovf_I:
                    case Code.Conv_Ovf_I_Un:
                        stack.Pop();
                        push(ts.IntPtr);
                        break;
                    case Code.Conv_I1:
                    case Code.Conv_Ovf_I1:
                    case Code.Conv_Ovf_I1_Un:
                        stack.Pop();
                        push(ts.SByte);
                        break;
                    case Code.Conv_I2:
                    case Code.Conv_Ovf_I2:
                    case Code.Conv_Ovf_I2_Un:
                        stack.Pop();
                        push(ts.Int16);
                        break;
                    case Code.Conv_I4:
                    case Code.Conv_Ovf_I4:
                    case Code.Conv_Ovf_I4_Un:
                        stack.Pop();
                        push(ts.Int32);
                        break;
                    case Code.Conv_I8:
                    case Code.Conv_Ovf_I8:
                    case Code.Conv_Ovf_I8_Un:
                        stack.Pop();
                        push(ts.Int64);
                        break;
                    case Code.Conv_U:
                    case Code.Conv_Ovf_U:
                    case Code.Conv_Ovf_U_Un:
                        stack.Pop();
                        push(ts.UIntPtr);
                        break;
                    case Code.Conv_U1:
                    case Code.Conv_Ovf_U1:
                    case Code.Conv_Ovf_U1_Un:
                        stack.Pop();
                        push(ts.Byte);
                        break;
                    case Code.Conv_U2:
                    case Code.Conv_Ovf_U2:
                    case Code.Conv_Ovf_U2_Un:
                        stack.Pop();
                        push(ts.UInt16);
                        break;
                    case Code.Conv_U4:
                    case Code.Conv_Ovf_U4:
                    case Code.Conv_Ovf_U4_Un:
                        stack.Pop();
                        push(ts.UInt32);
                        break;
                    case Code.Conv_U8:
                    case Code.Conv_Ovf_U8:
                    case Code.Conv_Ovf_U8_Un:
                        stack.Pop();
                        push(ts.UInt64);
                        break;
                    case Code.Conv_R4:
                        stack.Pop();
                        push(ts.Single);
                        break;
                    case Code.Conv_R8:
                    case Code.Conv_R_Un:
                        stack.Pop();
                        push(ts.Double);
                        break;
                    case Code.Dup:
                        push(stack.Peek().Item1);
                        break;
                    case Code.Ldarga:
                    case Code.Ldarga_S:
                    case Code.Ldelema:
                    case Code.Ldlen:
                        push(ts.IntPtr);
                        break;
                    case Code.Ldarg:
                    case Code.Ldarg_0:
                    case Code.Ldarg_1:
                    case Code.Ldarg_2:
                    case Code.Ldarg_3:
                    case Code.Ldarg_S:
                        {
                            var pi = 0;

                            if (c == Code.Ldarg || c == Code.Ldarg_S)
                                pi = n.Operand is int ? (int)n.Operand : ((Parameter)n.Operand).Index;
                            else
                                switch (c)
                                {
                                    // ldarg.0: default val is 0
                                    case Code.Ldarg_1:
                                        pi = 1;
                                        break;
                                    case Code.Ldarg_2:
                                        pi = 2;
                                        break;
                                    case Code.Ldarg_3:
                                        pi = 3;
                                        break;
                                }

                            push(method.Parameters[pi].Type);
                        }
                        break;
                    case Code.Beq:
                    case Code.Beq_S:
                    case Code.Bge:
                    case Code.Bge_S:
                    case Code.Bge_Un:
                    case Code.Bge_Un_S:
                    case Code.Bgt:
                    case Code.Bgt_S:
                    case Code.Bgt_Un:
                    case Code.Bgt_Un_S:
                    case Code.Ble:
                    case Code.Ble_S:
                    case Code.Ble_Un:
                    case Code.Ble_Un_S:
                    case Code.Blt:
                    case Code.Blt_S:
                    case Code.Blt_Un:
                    case Code.Blt_Un_S:
                    case Code.Bne_Un:
                    case Code.Bne_Un_S:
                    case Code.Brfalse:
                    case Code.Brfalse_S:
                    case Code.Brtrue:
                    case Code.Brtrue_S:
                        stack.Pop();
                        break;
                    case Code.Ldc_I4:
                    case Code.Ldc_I4_0:
                    case Code.Ldc_I4_1:
                    case Code.Ldc_I4_2:
                    case Code.Ldc_I4_3:
                    case Code.Ldc_I4_4:
                    case Code.Ldc_I4_5:
                    case Code.Ldc_I4_6:
                    case Code.Ldc_I4_7:
                    case Code.Ldc_I4_8:
                    case Code.Ldc_I4_M1:
                    case Code.Ldc_I4_S:
                        push(ts.Int32);
                        break;
                    case Code.Ldc_I8:
                        push(ts.Int64);
                        break;
                    case Code.Ldc_R4:
                        push(ts.Single);
                        break;
                    case Code.Ldc_R8:
                        push(ts.Double);
                        break;
                    case Code.Ldelem_I:
                        stack.Pop();
                        push(ts.IntPtr);
                        break;
                    case Code.Ldelem_I1:
                        stack.Pop();
                        push(ts.Byte);
                        break;
                    case Code.Ldelem_I2:
                        stack.Pop();
                        push(ts.Int16);
                        break;
                    case Code.Ldelem_I4:
                        stack.Pop();
                        push(ts.Int32);
                        break;
                    case Code.Ldelem_I8:
                        stack.Pop();
                        push(ts.Int64);
                        break;
                    case Code.Ldelem_U1:
                        stack.Pop();
                        push(ts.Byte);
                        break;
                    case Code.Ldelem_U2:
                        stack.Pop();
                        push(ts.UInt16);
                        break;
                    case Code.Ldelem_U4:
                        stack.Pop();
                        push(ts.UInt32);
                        break;
                    case Code.Ldelem_R4:
                        stack.Pop();
                        push(ts.Single);
                        break;
                    case Code.Ldelem_R8:
                        stack.Pop();
                        push(ts.Double);
                        break;
                    case Code.Ldelem_Ref:
                        stack.Pop();
                        push(n.Operand == null ? ts.IntPtr : ((ITypeDefOrRef)n.Operand).ToTypeSig());
                        break;
                    case Code.Ldfld:
                    case Code.Ldsfld:
                        if (n.Operand is FieldDef)
                            push(((FieldDef)n.Operand).FieldType);
                        else if (n.Operand is MemberRef)
                           push(((MemberRef)n.Operand).FieldSig.Type);
                        else
                        {
                            //! PLACE BREAKPOINT HERE
                            int iii = 0;
                            iii = ++iii - 1;
                        }
                        break;
                    case Code.Ldflda:
                    case Code.Ldsflda:
                    case Code.Ldloca:
                    case Code.Ldloca_S:
                        push(ts.IntPtr);
                        break;
                    case Code.Ldloc:
                    case Code.Ldloc_S:
                    case Code.Ldloc_0:
                    case Code.Ldloc_1:
                    case Code.Ldloc_2:
                    case Code.Ldloc_3:
                        {
                            var li = 0;

                            if (c == Code.Ldloc || c == Code.Ldloc_S)
                                li = n.Operand is int ? (int)n.Operand : ((Local)n.Operand).Index;
                            else
                                switch (c)
                                {
                                    // ldloc.0: default val is 0
                                    case Code.Ldloc_1:
                                        li = 1;
                                        break;
                                    case Code.Ldloc_2:
                                        li = 2;
                                        break;
                                    case Code.Ldloc_3:
                                        li = 3;
                                        break;
                                }

                            push(body.Variables[li].Type);
                        }
                        break;
                    case Code.Ldnull:
                    case Code.Ldobj:
                        push(ts.Object);
                        break;
                    case Code.Ldstr:
                        push(ts.String);
                        break;
                    case Code.Ldftn:
                    case Code.Ldvirtftn:
                        push(ts.IntPtr);
                        break;
                    case Code.Nop:
                        break;
                    case Code.Pop:
                    case Code.Ret:
                        if (stack.Count > 0) // can return void
                            stack.Pop();
                        break;
                    case Code.Rethrow:
                    case Code.Throw:
                        return;
                    case Code.Sizeof:
                        push(ts.Int32);
                        break;
                    case Code.Starg:
                    case Code.Starg_S:
                    case Code.Stelem_I:
                    case Code.Stelem_I1:
                    case Code.Stelem_I2:
                    case Code.Stelem_I4:
                    case Code.Stelem_I8:
                    case Code.Stelem_R4:
                    case Code.Stelem_R8:
                    case Code.Stelem_Ref:
                    case Code.Stfld:
                    case Code.Stind_I:
                    case Code.Stind_I1:
                    case Code.Stind_I2:
                    case Code.Stind_I4:
                    case Code.Stind_I8:
                    case Code.Stind_R4:
                    case Code.Stind_R8:
                    case Code.Stind_Ref:
                    case Code.Stloc:
                    case Code.Stloc_0:
                    case Code.Stloc_1:
                    case Code.Stloc_2:
                    case Code.Stloc_3:
                    case Code.Stloc_S:
                    case Code.Stobj:
                    case Code.Stsfld:
                        stack.Pop();
                        break;
                    case Code.Unbox:
                        push(((ITypeDefOrRef)n.Operand).ToTypeSig());
                        break;
                    case Code.Unbox_Any:
                        stack.Pop();
                        push(ts.IntPtr);
                        break;
                }
                #endregion
            }
        }
    }
}
