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

        public static string OriginChain(StackItem si)
        {
            return "{" + String.Join(";", (si.Origin ?? Empty<StackItem>.Array).Select(ii => ii.Instr.ToString() + " -> " + OriginChain(ii))) + "}";
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
                case 0: return Instruction.Create(OpCodes.Ldarg_0);
                case 1: return Instruction.Create(OpCodes.Ldarg_1);
                case 2: return Instruction.Create(OpCodes.Ldarg_2);
                case 3: return Instruction.Create(OpCodes.Ldarg_3);
                default:
                    if (index <= Byte.MaxValue)
                        return Instruction.Create(OpCodes.Ldarg_S, @params[index - offset]);
                    //Y U NO HAVE USHORT
                    return Instruction.Create(OpCodes.Ldarg, @params[index - offset]);
            }
        }

        internal static Code Simplify(this Code a)
        {
            switch (a)
            {
                case Code.Beq_S: return Code.Beq;
                case Code.Bge_S: case Code.Bge_Un: case Code.Bge_Un_S: return Code.Bge;
                case Code.Ble_S: case Code.Ble_Un: case Code.Ble_Un_S: return Code.Ble;
                case Code.Bgt_S: case Code.Bgt_Un: case Code.Bgt_Un_S: return Code.Bgt;
                case Code.Blt_S: case Code.Blt_Un: case Code.Blt_Un_S: return Code.Blt;
                case Code.Bne_Un_S: return Code.Bne_Un;
                case Code.Brfalse_S: return Code.Brfalse;
                case Code.Brtrue_S: return Code.Brtrue;
                case Code.Br_S: return Code.Br;
                case Code.Ldarga_S: return Code.Ldarga;
                case Code.Ldc_I4_0: case Code.Ldc_I4_1: case Code.Ldc_I4_2: case Code.Ldc_I4_3: case Code.Ldc_I4_4: case Code.Ldc_I4_5: case Code.Ldc_I4_6: case Code.Ldc_I4_7: case Code.Ldc_I4_8: case Code.Ldc_I4_M1: case Code.Ldc_I4_S:
                    return Code.Ldc_I4;
                case Code.Ldloc_0: case Code.Ldloc_1: case Code.Ldloc_2: case Code.Ldloc_3: case Code.Ldloc_S: return Code.Ldloc;
                case Code.Ldloca_S: return Code.Ldloca;
                case Code.Leave_S: return Code.Leave;
                case Code.Starg_S: return Code.Starg;
                case Code.Stloc_0: case Code.Stloc_1: case Code.Stloc_2: case Code.Stloc_3: case Code.Stloc_S: return Code.Stloc;
                case Code.Ldind_I1: case Code.Ldind_I2: case Code.Ldind_I4: case Code.Ldind_I8:
                case Code.Ldind_U1: case Code.Ldind_U2: case Code.Ldind_U4: return Code.Ldind_I;
                case Code.Ldind_R4: return Code.Ldind_R8;
                case Code.Stind_I1: case Code.Stind_I2: case Code.Stind_I4: case Code.Stind_I8: return Code.Stind_I;
                case Code.Stind_R4: return Code.Stind_R8;
                case Code.Unbox_Any: return Code.Unbox;
                case Code.Add_Ovf: case Code.Add_Ovf_Un: return Code.Add;
                case Code.Div_Un: return Code.Div;
                case Code.Mul_Ovf: case Code.Mul_Ovf_Un: return Code.Mul;
                case Code.Shr_Un: return Code.Shr;
                case Code.Rem_Un: return Code.Rem;
                case Code.Sub_Ovf: case Code.Sub_Ovf_Un: return Code.Sub;
                case Code.Callvirt: return Code.Call;
                case Code.Cgt_Un: return Code.Cgt;
                case Code.Clt_Un: return Code.Clt;
                case Code.Conv_I1: case Code.Conv_I2: case Code.Conv_I4: case Code.Conv_I8:
                case Code.Conv_Ovf_I1: case Code.Conv_Ovf_I2: case Code.Conv_Ovf_I4: case Code.Conv_Ovf_I8:
                case Code.Conv_Ovf_I1_Un: case Code.Conv_Ovf_I2_Un: case Code.Conv_Ovf_I4_Un: case Code.Conv_Ovf_I8_Un:
                    return Code.Conv_I;
                case Code.Conv_U1: case Code.Conv_U2: case Code.Conv_U4: case Code.Conv_U8:
                case Code.Conv_Ovf_U1: case Code.Conv_Ovf_U2: case Code.Conv_Ovf_U4: case Code.Conv_Ovf_U8:
                case Code.Conv_Ovf_U1_Un: case Code.Conv_Ovf_U2_Un: case Code.Conv_Ovf_U4_Un: case Code.Conv_Ovf_U8_Un:
                    return Code.Conv_U;
                case Code.Conv_R4: case Code.Conv_R_Un: return Code.Conv_R8;
                default: return a;
            }
        }

        static bool CodeEqIgnoreS(Code a, Code b)
        {
            if (a == b) return true;

            switch (a)
            {
                case Code.Beq: case Code.Beq_S: return b == Code.Beq || b == Code.Beq_S;
                case Code.Bge: case Code.Bge_S: return b == Code.Bge || b == Code.Bge_S;
                case Code.Ble: case Code.Ble_S: return b == Code.Ble || b == Code.Ble_S;
                case Code.Bgt: case Code.Bgt_S: return b == Code.Bgt || b == Code.Bgt_S;
                case Code.Blt: case Code.Blt_S: return b == Code.Blt || b == Code.Blt_S;
                case Code.Bge_Un: case Code.Bge_Un_S: return b == Code.Bge_Un || b == Code.Bge_Un_S;
                case Code.Ble_Un: case Code.Ble_Un_S: return b == Code.Ble_Un || b == Code.Ble_Un_S;
                case Code.Bgt_Un: case Code.Bgt_Un_S: return b == Code.Bgt_Un || b == Code.Bgt_Un_S;
                case Code.Blt_Un: case Code.Blt_Un_S: return b == Code.Blt_Un || b == Code.Blt_Un_S;
                case Code.Bne_Un: case Code.Bne_Un_S: return b == Code.Bne_Un || b == Code.Bne_Un_S;
                case Code.Brfalse: case Code.Brfalse_S: return b == Code.Brfalse || b == Code.Brfalse_S;
                case Code.Brtrue: case Code.Brtrue_S: return b == Code.Brtrue || b == Code.Brtrue_S;
                case Code.Br: case Code.Br_S: return b == Code.Br || b == Code.Br_S;
                case Code.Ldarg: case Code.Ldarg_S: return b == Code.Ldarg || b == Code.Ldarg_S;
                case Code.Ldarga: case Code.Ldarga_S: return b == Code.Ldarga || b == Code.Ldarga_S;
                case Code.Ldc_I4: case Code.Ldc_I4_S: return b == Code.Ldc_I4 || b == Code.Ldc_I4_S;
                case Code.Ldloc: case Code.Ldloc_S: return b == Code.Ldloc || b == Code.Ldloc_S;
                case Code.Ldloca: case Code.Ldloca_S: return b == Code.Ldloca || b == Code.Ldloca_S;
                case Code.Leave: case Code.Leave_S: return b == Code.Leave || b == Code.Leave_S;
                case Code.Starg: case Code.Starg_S: return b == Code.Starg || b == Code.Starg_S;
                case Code.Stloc: case Code.Stloc_S: return b == Code.Stloc || b == Code.Stloc_S;
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

        public static void RewireBranches(this CilBody body, Instruction old, Instruction nw)
        {
            // instructions
            if (body.HasInstructions) foreach (var i in body.Instructions)
                // don't even bother checking the opcode
                if (i.Operand == old) i.Operand = nw;

            // handlers
            if (body.HasExceptionHandlers) foreach (var h in body.ExceptionHandlers)
            {
                if (h.TryStart == old) h.TryStart = nw;
                if (h.TryEnd   == old) h.TryEnd   = nw;
                if (h. FilterStart == old) h. FilterStart = nw;
                if (h.HandlerStart == old) h.HandlerStart = nw;
                if (h.HandlerEnd   == old) h.HandlerEnd   = nw;
            }
        }

        static TypeSig ICast/* needs a better name */(TypeSig a, TypeSig b, ICorLibTypes ts)
        {
            if (comp.Equals(a, b)) return a;
            if (comp.Equals(a, ts.String)) return a;
            if (comp.Equals(b, ts.String)) return b;
            if (comp.Equals(a, ts.IntPtr) || comp.Equals(a, ts.UIntPtr)) return a;
            if (comp.Equals(b, ts.IntPtr) || comp.Equals(b, ts.UIntPtr)) return b;
            if (comp.Equals(a, ts.Double)) return a;
            if (comp.Equals(b, ts.Double)) return b;
            if (comp.Equals(a, ts.Single)) return a;
            if (comp.Equals(b, ts.Single)) return b;
            if (comp.Equals(a, ts.Int64) || comp.Equals(a, ts.UInt64)) return a;
            if (comp.Equals(b, ts.Int64) || comp.Equals(b, ts.UInt64)) return b;

            if (!a.IsByRef || !b.IsByRef) return ts.Object;

            var ad = a.ToTypeDefOrRef().ResolveTypeDefThrow();
            var bd = b.ToTypeDefOrRef().ResolveTypeDefThrow();

            if ((!ad.IsSequentialLayout || !ad.IsExplicitLayout) && (!bd.IsSequentialLayout || !bd.IsExplicitLayout))
                return ts.IntPtr;

            // close enough
            return ad.PackingSize > bd.PackingSize ? a : b;
        }

        static TypeSig TypeOfExpr(MethodDef md, Instruction ins, StackItem[] args)
        {
            var ts = md.Module.CorLibTypes;
            var c = ins.OpCode.Code;

            switch (c)
            {
                case Code.Add: case Code.Add_Ovf:
                case Code.And: case Code.Or: case Code.Xor:
                case Code.Div: case Code.Div_Un:
                case Code.Mul: case Code.Mul_Ovf: case Code.Mul_Ovf_Un:
                case Code.Rem: case Code.Rem_Un:
                case Code.Shl: case Code.Shr: case Code.Shr_Un:
                case Code.Sub: case Code.Sub_Ovf: case Code.Sub_Ovf_Un:
                    return ICast(args[0].Type, args[1].Type, ts);
                case Code.Not: case Code.Neg: case Code.Dup: return args[0].Type;
                case Code.Castclass: case Code.Constrained: case Code.Newarr:
                case Code.Ldobj: case Code.Unbox: case Code.Ldelem_Ref:
                    return ins.Operand == null ? ts.IntPtr : ((ITypeDefOrRef)ins.Operand).ToTypeSig();
                case Code.Box: case Code.Ldnull: case Code.Ldtoken: case Code.Refanytype:
                    return ts.Object;
                case Code.Call: case Code.Calli: case Code.Callvirt:
                    {   var m = (IMethod)ins.Operand;
                        return c == Code.Newobj ? m.DeclaringType.ToTypeSig() : m.MethodSig.RetType; }
                case Code.Ceq: case Code.Cgt: case Code.Cgt_Un: case Code.Clt: case Code.Clt_Un:
                case Code.Ckfinite: case Code.Isinst: return ts.Boolean;
                case Code.Conv_I: case Code.Conv_Ovf_I: case Code.Conv_Ovf_I_Un:
                case Code.Ldarga: case Code.Ldarga_S: case Code.Ldlen: case Code.Ldelema:
                case Code.Ldelem_I: case Code.Ldflda: case Code.Ldsflda: case Code.Ldloca:
                case Code.Ldloca_S: case Code.Ldind_I: case Code.Ldftn: case Code.Ldvirtftn:
                case Code.Sizeof: case Code.Arglist: case Code.Refanyval: case Code.Localloc:
                    return ts.IntPtr;
                case Code.Conv_I1: case Code.Conv_Ovf_I1: case Code.Conv_Ovf_I1_Un:
                case Code.Ldelem_I1: case Code.Ldind_I1: return ts.SByte;
                case Code.Conv_I2: case Code.Conv_Ovf_I2: case Code.Conv_Ovf_I2_Un:
                case Code.Ldelem_I2: case Code.Ldind_I2: return ts.Int16;
                case Code.Ldc_I4: case Code.Ldc_I4_0: case Code.Ldc_I4_1: case Code.Ldc_I4_2:
                case Code.Ldc_I4_3: case Code.Ldc_I4_4: case Code.Ldc_I4_5: case Code.Ldc_I4_6:
                case Code.Ldc_I4_7: case Code.Ldc_I4_8: case Code.Ldc_I4_M1: case Code.Ldc_I4_S:
                case Code.Conv_I4: case Code.Conv_Ovf_I4: case Code.Conv_Ovf_I4_Un:
                case Code.Ldelem_I4: case Code.Ldind_I4: return ts.Int32;
                case Code.Conv_I8: case Code.Conv_Ovf_I8: case Code.Conv_Ovf_I8_Un:
                case Code.Ldc_I8: case Code.Ldelem_I8: case Code.Ldind_I8: return ts.Int64;
                case Code.Conv_U1: case Code.Conv_Ovf_U1: case Code.Conv_Ovf_U1_Un:
                case Code.Ldelem_U1: case Code.Ldind_U1: return ts.Byte;
                case Code.Conv_U: case Code.Conv_Ovf_U: case Code.Conv_Ovf_U_Un: return ts.UIntPtr;
                case Code.Conv_U2: case Code.Conv_Ovf_U2: case Code.Conv_Ovf_U2_Un:
                case Code.Ldelem_U2: case Code.Ldind_U2: return ts.UInt16;
                case Code.Conv_U4: case Code.Conv_Ovf_U4: case Code.Conv_Ovf_U4_Un:
                case Code.Ldelem_U4: case Code.Ldind_U4: return ts.UInt32;
                case Code.Conv_U8: case Code.Conv_Ovf_U8: case Code.Conv_Ovf_U8_Un: return ts.UInt64;
                case Code.Ldc_R4: case Code.Conv_R4: case Code.Ldelem_R4: return ts.Single;
                case Code.Conv_R8: case Code.Conv_R_Un: case Code.Ldc_R8: case Code.Ldelem_R8:
                    return ts.Double;
                case Code.Ldarg_0: case Code.Ldarg_1: case Code.Ldarg_2: case Code.Ldarg_3:
                case Code.Ldarg: case Code.Ldarg_S: return ins.GetParameter(md.Parameters).Type;
                case Code.Ldfld: case Code.Ldsfld: return ((IField)ins.Operand).FieldSig.Type;
                case Code.Ldloc_0: case Code.Ldloc_1: case Code.Ldloc_2: case Code.Ldloc_3:
                case Code.Ldloc: case Code.Ldloc_S: return ins.GetLocal(md.Body.Variables).Type;
                case Code.Ldstr: return ts.String;
                case Code.Ret: return md.ReturnType;
                case Code.Mkrefany: return ts.TypedReference;
            }

            return ts.Void;
        }
        static void RecreateStack(this MethodDef def, List<StackItem> stack, ref int ind)
        {
            var body = def.Body;
            var ins = body.Instructions[ind];

            int pushes, pops;

            var isv = def.ReturnType.RemovePinnedAndModifiers().ElementType == ElementType.Void;
            ins.CalculateStackUsage(!isv, out pushes, out pops);

            if (pops   < 0) // clear the stack
                throw new NotImplementedException("stack clearing isn't implemented (" + ins + ").");

            if (pushes == 0)
            {
                // nothing, just fall through...
                if (pops == 0)
                {
                    ind--;
                    RecreateStack(def, stack, ref ind);
                    return;
                }
                // well shit
                else throw new NotImplementedException("Consuming-only instructions (" + ins + ") aren't implemented.");
            }

            int oldTop = stack.Count;
            for (int i = 0; i < pops; i++)
            {
                ind--;
                RecreateStack(def, stack, ref ind);
            }

            if (oldTop > stack.Count)
                throw new InvalidOperationException("Bogus stack");

            if (pushes > 1) // only opcode that pushes more than 1 thing is dup
            {
                if (ins.OpCode.Code != Code.Dup)
                    throw new NotImplementedException("Unexpected opcode " + ins);
                // input.Length == 1

                var toDup = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);

                var dup = new StackItem { Type = toDup.Type, Instr = ins, Origin = new[] { toDup } };
                stack.Add(dup);
                stack.Add(dup);

                return;
            }

            StackItem[] popv = new StackItem[pops];
            for (int i = 0; i < pops; i++)
            {
                popv[pops - i - 1 /* reverse */] = stack[oldTop];
                stack.RemoveAt(oldTop);
            }

            var ty = TypeOfExpr(def, ins, popv);
            if (ty.RemovePinnedAndModifiers().ElementType == ElementType.Void)
                throw new InvalidOperationException("Using a void method in an expression tree " + ins);

            stack.Add(new StackItem { Type = ty, Instr = ins, Origin = popv });
        }
        public static StackItem RecreateStack(MethodDef md, Instruction ins)
        {
            var body = md.Body;

            var stack = new List<StackItem>(body.MaxStack);

            int ind = body.Instructions.IndexOf(ins) - 1;
            RecreateStack(md, stack, ref ind);

            return stack[0]; // it should boil down to a single expr
        }

        // NOTE: stack may contain an int/uint when the actual C# type is a byte/..., because most structural primitives are all (u)ints in IL
        // NOTE: not sure if it would work correctly with branching for hand-written IL
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
                    Origin = o ?? Empty<StackItem>.Array
                });

                TypeSig exnType = null;
                foreach (var eh in body.ExceptionHandlers)
                    if (eh.HandlerType == ExceptionHandlerType.Filter)
                    {
                        if (eh.FilterStart == n)
                        {
                            exnType = eh.CatchType.ToTypeSig() ?? ts.Object;
                            goto PUSH;
                        }
                    }
                    else if (eh.HandlerType == ExceptionHandlerType.Catch && eh.HandlerStart == n)
                    {
                        exnType = eh.CatchType.ToTypeSig();
                        goto PUSH;
                    }

                goto NO_PUSH;
            PUSH:
                push(exnType, Empty<StackItem>.Array);
            NO_PUSH:

                i = cb(i, n, stack);
                if (i < 0)
                    return;

                if (n.OpCode.Code == Code.Dup)
                {
                    var tod = stack.Peek();
                    push(tod.Type, new[] { tod });

                    continue;
                }

                int pushes, pops;
                var isv = method.ReturnType.RemovePinnedAndModifiers().ElementType == ElementType.Void;
                n.CalculateStackUsage(!isv, out pushes, out pops);

                // pushes should be <= 1

                int len = pops < 0 ? stack.Count : pops;
                StackItem[] popv = new StackItem[len];
                for (int ii = 0; ii < len; ++ii)
                    popv[ii] = stack.Pop();

                if (pushes > 0)
                    push(TypeOfExpr(method, n, popv), popv);
            }
        }
    }
}

