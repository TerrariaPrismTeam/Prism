using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector.Patcher
{
    public static class CecilHelperExtensions
    {
        /// <summary>
        /// Gets the ldarg instruction of the specified index using the smallest value type it can (because we're targeting the Sega Genesis and need to save memory).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static Instruction GetLdargOf(this IList<ParameterDefinition> @params, ushort index, bool isInstance = true)
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

        public static TypeDefinition CreateDelegate(this CecilContext context, string @namespace, string name, TypeReference returnType, out MethodDefinition invoke, params TypeReference[] parameters)
        {
            var cResolver = context.Resolver;
            var typeSys = context.PrimaryAssembly.MainModule.TypeSystem;

            var delegateType = new TypeDefinition(@namespace, name, TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed, cResolver.ReferenceOf(typeof(MulticastDelegate)));

            var ctor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, typeSys.Void);
            ctor.IsRuntime = true;
            ctor.Parameters.Add(new ParameterDefinition("object", 0, typeSys.Object));
            ctor.Parameters.Add(new ParameterDefinition("method", 0, typeSys.IntPtr));

            delegateType.Methods.Add(ctor);

            invoke = new MethodDefinition("Invoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, returnType);
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
        public static Instruction[] FindInstrSeq(this MethodBody body, OpCode[] instrs)
        {
            return body.FindInstrSeq(instrs, instrs.Length);
        }
        public static Instruction[] FindInstrSeq(this MethodBody body, OpCode[] instrs, int amt)
        {
            Instruction[] result = new Instruction[amt];

            for (int i = 0; i < result.Length; i++)
                result[i] = i == 0 ? body.FindInstrSeqStart(instrs) : result[i - 1] != null ? result[i - 1].Next : null;

            return result;
        }
        public static Instruction FindInstrSeqStart(this MethodBody body, OpCode[] instrs)
        {
            for (int i = 0; i < body.Instructions.Count - instrs.Length; i++)
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
            if (!orig.Chronological(null, i => i.Next) || !repl.Chronological(null, i => i.Next))
                Console.Error.WriteLine("Error: Both sequences in CecilHelper.ReplaceInstructions(ILProcessor, IEnumerable<Instruction>, IEnumerable<Instruction>) must be chronological.");

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
                n = i.Next;
                p.Remove(i);
            }

            return n;
        }
        public static Instruction RemoveInstructions(this ILProcessor p, Instruction first, int count)
        {
            var cur = first;
            var prev = first.Previous;
            for (int i = 0; i < count; i++)
            {
                if (cur == null)
                    break;

                prev = cur;
                var n = cur.Next;
                p.Remove(cur);
                cur = n;
            }

            return cur ?? prev;
        }
        // callee must have the same args as the calling method
        // if the callee is an instance method, the object must be placed on the stack first
        public static void EmitWrapperCall(this ILProcessor proc, MethodDefinition toCall, Instruction before = null)
        {
            //var caller = proc.Body.Method;

            for (ushort i = 0; i < toCall.Parameters.Count /*+ (toCall.IsStatic ? 0 : 1)*/; i++)
                if (before == null)
                    proc.Append(toCall.Parameters.GetLdargOf(i, !toCall.IsStatic/*false*/));
                else
                    proc.InsertBefore(before, toCall.Parameters.GetLdargOf(i, !toCall.IsStatic/*false*/));

            var c = toCall.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;
            if (before == null)
                proc.Emit(c, toCall);
            else
                proc.InsertBefore(before, Instruction.Create(c, toCall));
        }
    }
}
