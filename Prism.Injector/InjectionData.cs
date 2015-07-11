using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using ReflectionMethodInfo = System.Reflection.MethodInfo;

namespace Prism.Injector
{
    public enum InjectionPosition
    {
        Pre,
        Post
    }

    public struct MethodInfo
    {
        readonly static string
            DOT = ".",
            H_T = "#";

        public string Type;
        public string Name;
        /// <summary>
        /// Overload index, if multiple exist.
        /// </summary>
        public int Overload;

        public MethodInfo(string type, string name, int ovl = -1)
        {
            Type = type;
            Name = name;
            Overload = ovl;
        }

        public override string ToString()
        {
            return Type + DOT + Name + H_T + Overload;
        }
    }
    public static class MethodInfoExt
    {
        public static MethodInfo ToInjectorInfo(ReflectionMethodInfo mi)
        {
            return
                new MethodInfo(mi.DeclaringType.FullName, mi.Name,
                    Array.IndexOf(mi.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Where(rmi => rmi.Name == mi.Name).ToArray(), mi));
        }
    }

    public abstract class InjectionData
    {
        /// <summary>
        /// Method to inject the instructions in.
        /// </summary>
        public MethodInfo Target;
        /// <summary>
        /// Before or after the target specification (method itself/method call/instruction).
        /// </summary>
        public InjectionPosition Position;
        /// <summary>
        /// The instructions to inject.
        /// </summary>
        public ILInstruction[] ToInject;

        protected internal InjectionData(MethodInfo tar, InjectionPosition pos, ILInstruction[] toInject)
        {
            Target   = tar     ;
            Position = pos     ;
            ToInject = toInject;
        }

        /// <summary>
        /// Inject instructions at the beginning or before a 'ret' instruction in a method.
        /// </summary>
        public sealed class Method : InjectionData
        {
            public int RetIndex;

            internal Method(MethodInfo tar, InjectionPosition pos, ILInstruction[] toInject, int retIndex)
                : base(tar, pos, toInject)
            {
                RetIndex = retIndex;
            }

            public static Method NewMethodPre (MethodInfo tar, ILInstruction[] toInject)
            {
                return new Method(tar, InjectionPosition.Pre, toInject, -1);
            }
            public static Method NewMethodPost(MethodInfo tar, ILInstruction[] toInject, int retIndex = 0)
            {
                return new Method(tar, InjectionPosition.Post, toInject, retIndex);
            }
        }
        /// <summary>
        /// Inject instructions before or after a certain method call.
        /// </summary>
        public sealed class Call : InjectionData
        {
            public MethodInfo Callee;
            /// <summary>
            /// If the method is called multiple times, this specifies where exactly the instructions should be injected.
            /// </summary>
            public int CallPosition;

            public Call(MethodInfo tar, InjectionPosition pos, ILInstruction[] toInject, MethodInfo callee, int cpos = 0)
                : base(tar, pos, toInject)
            {
                Callee       = callee;
                CallPosition = cpos  ;
            }

            public static Call NewCall(MethodInfo tar, InjectionPosition pos, ILInstruction[] toInject, MethodInfo callee, int cpos = 0)
            {
                return new Call(tar, pos, toInject, callee, cpos);
            }
        }
        public sealed class Instruction : InjectionData
        {
            public int IndexOffset;

            public bool IsIndex
            {
                get
                {
                    return IndexOffset <= 0;
                }
            }
            public bool IsOffset
            {
                get
                {
                    return IndexOffset >= 0;
                }
            }

            public int Index
            {
                get
                {
                    if (IndexOffset > 0)
                        throw new Exception("IndexOffset represents an offset.");

                    return -IndexOffset;
                }
            }
            public int Offset
            {
                get
                {
                    if (IndexOffset < 0)
                        throw new Exception("IndexOffset represents an index.");

                    return IndexOffset;
                }
            }

            internal Instruction(MethodInfo tar, InjectionPosition pos, ILInstruction[] toInject, int indexOffset)
                : base(tar, pos, toInject)
            {
                IndexOffset = indexOffset;
            }

            public static Instruction NewInstructionIndex (MethodInfo tar, InjectionPosition pos, ILInstruction[] toInject, int index )
            {
                return new Instruction(tar, pos, toInject, -index );
            }
            public static Instruction NewInstructionOffset(MethodInfo tar, InjectionPosition pos, ILInstruction[] toInject, int offset)
            {
                return new Instruction(tar, pos, toInject,  offset);
            }
        }

        internal Instruction ToInstruction(Func<MethodInfo, MethodDefinition> methodResolver)
        {
            if (methodResolver == null)
                throw new ArgumentNullException("methodResolver");

            if (this is Instruction)
                return (Instruction)this;

            var t = methodResolver(Target);

            if (t == null)
                throw new Exception("Resolved target method is null. Method to resolve: " + Target);

            var b = t.Body;
            var ins = b.Instructions;

            if (this is Method)
            {
                int off;
                var ri = ((Method)this).RetIndex;

                if (ri < 0)
                    off = 0;
                else
                {
                    var f = ins.Where(i => i.OpCode.Code == Code.Ret).ToArray();

                    if (ri < 0 || ri >= f.Length)
                        throw new Exception("Invalid ret OpCode index" + ri
                            + " in method " + Target.Name + ":" + Target.Overload);

                    off = f[ri].Offset;
                }

                return new Instruction(Target, InjectionPosition.Pre, ToInject, off);
            }
            if (this is Call)
            {
                var ce = ((Call)this).Callee      ;
                var cp = ((Call)this).CallPosition;

                var cs = ins.Where(i =>
                {
                    if (i.OpCode.Code != Code.Call && i.OpCode.Code != Code.Callvirt)
                        return false;

                    var o = methodResolver(ce);

                    if (o == null)
                        throw new Exception("Resolved method is null. Method to resolve: " + Target);

                    return o == i.Operand;
                }).ToArray();

                if (cp < 0 || cp >= cs.Length)
                    throw new Exception("Invalid call index " + cp
                        + " to method " + ce    .Name + ":" + ce    .Overload
                        + " in method " + Target.Name + ":" + Target.Overload);

                return new Instruction(Target, Position, ToInject, cs[cp].Offset);
            }

            throw new Exception("Wat? InjectionData is of unexpected deriving type " + GetType());
        }
    }
}
