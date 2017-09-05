using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    using StackInfo = Stack<StackItem>;

    public struct StackItem : IEquatable<StackItem>
    {
        public TypeSig Type;
        public Instruction Instr;
        public StackItem[] Origin;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is StackItem)
                return Equals((StackItem)obj);
            if (obj is IEquatable<StackItem>)
                return ((IEquatable<StackItem>)obj).Equals(this);

            return false;
        }
        public override int GetHashCode()
        {
            return Type.GetHashCode() + Instr.GetHashCode() + Origin.GetHashCode();
        }
        public override string ToString()
        {
            return "{" + Type + " -> " + Instr + "}";
        }

        public bool Equals(StackItem other)
        {
            return DNHelperExtensions.comp.Equals(Type, other.Type) && Instr == other.Instr;
        }

        public static bool operator ==(StackItem a, StackItem b)
        {
            return  a.Equals(b);
        }
        public static bool operator !=(StackItem a, StackItem b)
        {
            return !a.Equals(b);
        }
    }

    public static class DNHelperExtensions
    {
        internal readonly static SigComparer comp = new SigComparer(SigComparerOptions.PrivateScopeIsComparable);

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
                    return Instruction.Create(OpCodes.Ldarg, @params[index - offset]);
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

            return a == b;
        }

        public static TypeDef CreateDelegate(this DNContext context, string @namespace, string name, TypeSig returnType, out MethodDef invoke, params TypeSig[] parameters)
        {
            var cResolver = context.Resolver;
            var typeSys = context.PrimaryAssembly.ManifestModule.CorLibTypes;

            var delegateType = new TypeDefUser(@namespace, name, cResolver.ReferenceOf(typeof(MulticastDelegate)));
            delegateType.Attributes = TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Sealed;

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
                invoke.Parameters[i].ParamDef.Name = "arg" + (i - 1);
            }

            delegateType.Methods.Add(invoke);

            var beginInvoke = new MethodDefUser("BeginInvoke", MethodSig.CreateInstance(cResolver.ReferenceOf(typeof(IAsyncResult)).ToTypeSig(),
                    parameters.Concat(new[] { cResolver.ReferenceOf(typeof(AsyncCallback)).ToTypeSig(), typeSys.Object }).ToArray()),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
            beginInvoke.ImplAttributes |= MethodImplAttributes.Runtime;
            for (int i = 0; i < parameters.Length; i++)
            {
                beginInvoke.Parameters[i + 1].CreateParamDef();
                beginInvoke.Parameters[i + 1].ParamDef.Name = "arg" + i;
            }
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 2].CreateParamDef();
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 2].ParamDef.Name = "callback";
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 1].CreateParamDef();
            beginInvoke.Parameters[beginInvoke.Parameters.Count - 1].ParamDef.Name = "object"  ;

            delegateType.Methods.Add(beginInvoke);

            var endInvoke = new MethodDefUser("EndInvoke", MethodSig.CreateInstance(typeSys.Void, cResolver.ReferenceOf(typeof(IAsyncResult)).ToTypeSig()),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
            endInvoke.ImplAttributes |= MethodImplAttributes.Runtime;
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
                    if (instrs[j] != null /* allow for wildcards */
                            && !CodeEqIgnoreS(body.Instructions[i + j].OpCode.Code, instrs[j].Code))
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
        // NOTE: not sure if it would work correctly with branching for hand-written IL
        // NOTE: neither when popping an exn in an exn handler (i.e. a "catch { }")
        public static void EnumerateWithStackAnalysis(this MethodDef method, Func<int, Instruction, StackInfo, int> cb)
        {
            if (cb == null)
                throw new ArgumentNullException("cb");

            var body = method.Body;
            var ts = method.Module.CorLibTypes;

            var stack = new StackInfo(body.MaxStack);

            var ins = body.Instructions;
            for (int i = 0; i < ins.Count; i++)
            {
                var n = ins[i];
                var c = n.OpCode.Code;

                Action<TypeSig, StackItem[]> push = (tr, o) => stack.Push(new StackItem
                {
                    Type   = tr,
                    Instr  = n ,
                    Origin = o
                });

                i = cb(i, n, stack);

                StackItem pop0 = new StackItem(),
                          pop1 = new StackItem();

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
                        pop0 = stack.Pop();
                        pop1 = stack.Pop();
                        push(ICast(pop0.Type, pop1.Type, ts), new[] { pop0, pop1 });
                        break;
                    case Code.Neg:
                    case Code.Not:
                        pop0 = stack.Pop();
                        push(pop0.Type, new[] { pop0 });
                        break;
                    case Code.Newarr:
                        push(((ITypeDefOrRef)n.Operand).ToTypeSig(), new[] { stack.Peek() });
                        break;
                    case Code.Box:
                        // struct/value type ('bittable type') -> object ref
                        // eg. int x = 5; Foo((object /* here */)5);
                        push(ts.Object, new[] { stack.Pop() });
                        break;
                    case Code.Call:
                    case Code.Calli: // indirect call (native or managed), pops address and args
                    case Code.Callvirt: // virtual call (*always* an instance call)
                    case Code.Newobj: // said to return void, but actually pushes the class type
                        {
                            var m = (IMethod)n.Operand;

                            List<StackItem> pops = new List<StackItem>();

                            if (c == Code.Calli)
                                pops.Add(stack.Pop()); // address to call

                            // args
                            int start = n.OpCode.Code == Code.Newobj ? 1 : 0,
                                count = m.MethodSig.Params.Count;
                            for (int j = start; j < count; j++)
                                pops.Add(stack.Pop());

                            push(c == Code.Newobj ? m.DeclaringType.ToTypeSig() : m.MethodSig.RetType, pops.ToArray());
                        }
                        break;
                    case Code.Castclass: // cast class instance to another
                    case Code.Constrained: // ensure that something is of type foo (see eg. the disassembly of (5).ToString())
                        push(((ITypeDefOrRef)n.Operand).ToTypeSig(), new[] { stack.Pop() });
                        break;
                    case Code.Ceq:
                    case Code.Cgt:
                    case Code.Cgt_Un:
                    case Code.Clt:
                    case Code.Clt_Un:
                        push(ts.Boolean, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ckfinite: // throw if infinite or nan
                        break;
                    case Code.Isinst: // is obj ref of the given class?
                        push(ts.Boolean, new[] { stack.Pop() });
                        break;
                    case Code.Conv_I:
                    case Code.Conv_Ovf_I:
                    case Code.Conv_Ovf_I_Un:
                        push(ts.IntPtr, new[] { stack.Pop() });
                        break;
                    case Code.Conv_I1:
                    case Code.Conv_Ovf_I1:
                    case Code.Conv_Ovf_I1_Un:
                        push(ts.SByte, new[] { stack.Pop() });
                        break;
                    case Code.Conv_I2:
                    case Code.Conv_Ovf_I2:
                    case Code.Conv_Ovf_I2_Un:
                        push(ts.Int16, new[] { stack.Pop() });
                        break;
                    case Code.Conv_I4:
                    case Code.Conv_Ovf_I4:
                    case Code.Conv_Ovf_I4_Un:
                        push(ts.Int32, new[] { stack.Pop() });
                        break;
                    case Code.Conv_I8:
                    case Code.Conv_Ovf_I8:
                    case Code.Conv_Ovf_I8_Un:
                        push(ts.Int64, new[] { stack.Pop() });
                        break;
                    case Code.Conv_U:
                    case Code.Conv_Ovf_U:
                    case Code.Conv_Ovf_U_Un:
                        push(ts.UIntPtr, new[] { stack.Pop() });
                        break;
                    case Code.Conv_U1:
                    case Code.Conv_Ovf_U1:
                    case Code.Conv_Ovf_U1_Un:
                        push(ts.Byte, new[] { stack.Pop() });
                        break;
                    case Code.Conv_U2:
                    case Code.Conv_Ovf_U2:
                    case Code.Conv_Ovf_U2_Un:
                        push(ts.UInt16, new[] { stack.Pop() });
                        break;
                    case Code.Conv_U4:
                    case Code.Conv_Ovf_U4:
                    case Code.Conv_Ovf_U4_Un:
                        push(ts.UInt32, new[] { stack.Pop() });
                        break;
                    case Code.Conv_U8:
                    case Code.Conv_Ovf_U8:
                    case Code.Conv_Ovf_U8_Un:
                        push(ts.UInt64, new[] { stack.Pop() });
                        break;
                    case Code.Conv_R4:
                        push(ts.Single, new[] { stack.Pop() });
                        break;
                    case Code.Conv_R8:
                    case Code.Conv_R_Un:
                        push(ts.Double, new[] { stack.Pop() });
                        break;
                    case Code.Dup:
                        push(stack.Peek().Type, new[] { stack.Peek() });
                        break;
                    case Code.Ldarga: // push address of arg #n
                    case Code.Ldarga_S:
                        push(ts.IntPtr, null);
                        break;
                    case Code.Ldlen: // push length of arr (as native int)
                        push(ts.IntPtr, new[] { stack.Pop() });
                        break;
                    case Code.Ldelema: // load array element address (requires array & index)
                        push(ts.IntPtr, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldarg: // load array element (requires array & index)
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

                            /*if (!method.IsStatic)
                            {
                                if (pi == 0)
                                {
                                    push(method.DeclaringType, null);
                                    break;
                                }

                                pi--;
                            }*/

                            push(method.Parameters[pi].Type, null);
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
                        stack.Pop();
                        stack.Pop();
                        break;
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
                        push(ts.Int32, null);
                        break;
                    case Code.Ldc_I8:
                        push(ts.Int64, null);
                        break;
                    case Code.Ldc_R4:
                        push(ts.Single, null);
                        break;
                    case Code.Ldc_R8:
                        push(ts.Double, null);
                        break;
                    case Code.Ldelem_I: // load element of array
                        push(ts.IntPtr, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_I1:
                        push(ts.Byte, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_I2:
                        push(ts.Int16, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_I4:
                        push(ts.Int32, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_I8:
                        push(ts.Int64, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_U1:
                        push(ts.Byte, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_U2:
                        push(ts.UInt16, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_U4:
                        push(ts.UInt32, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_R4:
                        push(ts.Single, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_R8:
                        push(ts.Double, new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldelem_Ref: // load element of array (obj ref)
                        push(n.Operand == null ? ts.IntPtr : ((ITypeDefOrRef)n.Operand).ToTypeSig(),
                                new[] { stack.Pop(), stack.Pop() });
                        break;
                    case Code.Ldfld:
                        push(((IField)n.Operand).FieldSig.Type, new[] { stack.Pop() });
                        break;
                    case Code.Ldsfld:
                        push(((IField)n.Operand).FieldSig.Type, null);
                        break;
                    case Code.Ldflda: // load address of field
                        push(ts.IntPtr, new[] { stack.Pop() });
                        break;
                    case Code.Ldsflda:
                    case Code.Ldloca:
                    case Code.Ldloca_S:
                        push (ts.IntPtr, null);
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

                            push(body.Variables[li].Type, null);
                        }
                        break;
                    case Code.Ldind_I: // load indirectly (i.e. from address), eg *(foo + bar)
                        push(ts.IntPtr, new[] { stack.Pop() });
                        break;
                    case Code.Ldind_I1:
                        push(ts.SByte, new[] { stack.Pop() });
                        break;
                    case Code.Ldind_I2:
                        push(ts.Int16, new[] { stack.Pop() });
                        break;
                    case Code.Ldind_I4:
                        push(ts.Int32, new[] { stack.Pop() });
                        break;
                    case Code.Ldind_I8:
                        push(ts.Int64, new[] { stack.Pop() });
                        break;
                    case Code.Ldind_U1:
                        push(ts.Byte, new[] { stack.Pop() });
                        break;
                    case Code.Ldind_U2:
                        push(ts.UInt16, new[] { stack.Pop() });
                        break;
                    case Code.Ldind_U4:
                        push(ts.UInt32, new[] { stack.Pop() });
                        break;
                    // apparently doesn't exist (according to M$ docs)
                    /*case Code.Ldind_U8:
                        push(ts.UInt64, new[] { stack.Pop() });
                        break;*/
                    case Code.Ldnull: // push null
                        push(ts.Object, null);
                        break;
                    case Code.Ldobj: // load bittable type from pointer
                        push(((ITypeDefOrRef)n.Operand).ToTypeSig(), new[] { stack.Pop() });
                        break;
                    case Code.Ldstr:
                        push(ts.String, null);
                        break;
                    case Code.Ldftn: // metadata token -> function pointer
                    case Code.Ldvirtftn: // or ^ -> vtable entry
                        push(ts.IntPtr, null);
                        break;
                    case Code.Nop:
                        break;
                    case Code.Pop:
                        /*stack.Pop();
                        break;*/
                    case Code.Ret:
                        if (stack.Count > 0) // can return void
                            stack.Pop();
                        break;
                    case Code.Rethrow: // rethrows the exn, keeps stack trace (unlike throwing an already-existing exn, which makes it loose its stack trace)
                    case Code.Throw:
                        return;
                    case Code.Sizeof: // works with generic types and everything, much better than the C# keyword
                        push(ts.IntPtr, null);
                        break;
                    case Code.Stfld:
                    case Code.Stind_I:
                    case Code.Stind_I1:
                    case Code.Stind_I2:
                    case Code.Stind_I4:
                    case Code.Stind_I8:
                    case Code.Stind_R4:
                    case Code.Stind_R8:
                    case Code.Stind_Ref:
                    case Code.Stobj:
                        stack.Pop();
                        stack.Pop();
                        break;
                    case Code.Stelem_I: // store array element (requires array, index & value)
                    case Code.Stelem_I1:
                    case Code.Stelem_I2:
                    case Code.Stelem_I4:
                    case Code.Stelem_I8:
                    case Code.Stelem_R4:
                    case Code.Stelem_R8:
                    case Code.Stelem_Ref:
                        stack.Pop();
                        stack.Pop();
                        stack.Pop();
                        break;
                    case Code.Starg:
                    case Code.Starg_S:
                    case Code.Stloc:
                    case Code.Stloc_0:
                    case Code.Stloc_1:
                    case Code.Stloc_2:
                    case Code.Stloc_3:
                    case Code.Stloc_S:
                    case Code.Stsfld:
                        stack.Pop();
                        break;
                    case Code.Unbox: // unbox obj ref to blittable type
                    case Code.Unbox_Any:
                        push(((ITypeDefOrRef)n.Operand).ToTypeSig(), new[] { stack.Pop() });
                        break;
                    case Code.Ldtoken: // load metadata token
                        push(ts.Object /* a System.Type (does this work with /any/ type of token?) */, null);
                        break;
                    case Code.Switch: // jump to the offset on the stack
                        stack.Pop();
                        break;
                    case Code.Tailcall: // prefix: perform a tail call (unavailable in C#)
                        break;
                    case Code.Unaligned: // pointer access can be unaligned (GC etc. ensure good alignment for faster loading (probably))
                    case Code.Volatile: // next instruction is volatile
                        break;
                    case Code.Arglist: // load pointer to arglist (only available in *true* vararg methods
                                       // (as in, the C way, not the params T[] way)).
                                       // if one defines eg.
                                       //     [DllImport(...)]
                                       //     export void printf(string format, __arglist);
                                       // and then calls it with:
                                       //     printf("foo %i %s\n", __arglist(42), __arglist("bar"))
                                       // , 'true' vararg stuff is used (the resulting IL code will use some arglist hackery).
                                       // If one would implement such a function, one would do:
                                       //     void Foo(__arglist)
                                       //     {
                                       //         var aitor = new ArgIterator(__arglist);
                                       //         // do stuff with aitor
                                       //     }
                                       // the '__arglist' keyword here simply emits the 'arglist' instruction, which pushes
                                       // a pointer to the argument list which can be interpreted by the runtime and ArgIterator
                        push(ts.IntPtr, null); // actually a RuntimeArgumentHandle
                        break;
                    case Code.Break: // tells the debugger a breakpoint is reached
                        break;
                    case Code.Mkrefany: // address -> typed reference (strongly-typed pointerish type to a bittable type,
                                        // has some restrictions in usage compared to normal pointer types, but can be used to
                                        // implement generic pointers in pure C#)
                        push(ts.TypedReference, new[] { stack.Pop() });
                        break;
                    case Code.Refanytype: // typed reference -> type (of the pointer)
                        push(ts.Object /* a System.Type */, new[] { stack.Pop() });
                        break;
                    case Code.Refanyval: // typed reference -> address (of the pointee)
                        push(ts.IntPtr, new[] { stack.Pop() });
                        break;
                    case Code.Endfilter:
                        stack.Pop();
                        break;
                    case Code.Endfinally:
                        break;
                    case Code.Leave:
                    case Code.Leave_S:
                        break;
                    case Code.Localloc: // like malloc, but on the stack
                        push(ts.IntPtr, new [] { stack.Pop() });
                        break;
                }
                #endregion
            }
        }
    }
}
