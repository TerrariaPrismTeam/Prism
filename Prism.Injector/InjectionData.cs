using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

using ILInstruction = Mono.Cecil.Cil.Instruction;

namespace Prism.Injector
{
    public enum InjectionPosition
    {
        Pre,
        Post
    }

    public abstract class InjectionData
    {
        /// <summary>
        /// Method to inject the instructions in.
        /// </summary>
        public MethodDefinition Target;
        /// <summary>
        /// Before or after the target specification (method itself/method call/instruction).
        /// </summary>
        public InjectionPosition Position;
        /// <summary>
        /// The instructions to inject.
        /// </summary>
        public ILInstruction[] ToInject;

        [DebuggerStepThrough]
        protected internal InjectionData(MethodDefinition tar, InjectionPosition pos, ILInstruction[] toInject)
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

            [DebuggerStepThrough]
            internal Method(MethodDefinition tar, InjectionPosition pos, ILInstruction[] toInject, int retIndex)
                : base(tar, pos, toInject)
            {
                RetIndex = retIndex;
            }

            [DebuggerStepThrough]
            public static Method NewMethodPre (MethodDefinition tar, ILInstruction[] toInject)
            {
                return new Method(tar, InjectionPosition.Pre, toInject, -1);
            }
            [DebuggerStepThrough]
            public static Method NewMethodPost(MethodDefinition tar, ILInstruction[] toInject, int retIndex = 0)
            {
                return new Method(tar, InjectionPosition.Post, toInject, retIndex);
            }
        }
        /// <summary>
        /// Inject instructions before or after a certain method call.
        /// </summary>
        public sealed class Call : InjectionData
        {
            public MethodDefinition Callee;
            /// <summary>
            /// If the method is called multiple times, this specifies where exactly the instructions should be injected.
            /// </summary>
            public int CallPosition;

            [DebuggerStepThrough]
            public Call(MethodDefinition tar, InjectionPosition pos, ILInstruction[] toInject, MethodDefinition callee, int cpos = 0)
                : base(tar, pos, toInject)
            {
                Callee       = callee;
                CallPosition = cpos  ;
            }

            [DebuggerStepThrough]
            public static Call NewCall(MethodDefinition tar, InjectionPosition pos, ILInstruction[] toInject, MethodDefinition callee, int cpos = 0)
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

            [DebuggerStepThrough]
            internal Instruction(MethodDefinition tar, InjectionPosition pos, ILInstruction[] toInject, int indexOffset)
                : base(tar, pos, toInject)
            {
                IndexOffset = indexOffset;
            }

            [DebuggerStepThrough]
            public static Instruction NewInstructionIndex (MethodDefinition tar, InjectionPosition pos, ILInstruction[] toInject, int index )
            {
                return new Instruction(tar, pos, toInject, -index );
            }
            [DebuggerStepThrough]
            public static Instruction NewInstructionOffset(MethodDefinition tar, InjectionPosition pos, ILInstruction[] toInject, int offset)
            {
                return new Instruction(tar, pos, toInject,  offset);
            }
        }

        internal Instruction ToInstruction()
        {
            if (this is Instruction)
                return (Instruction)this;

            var b = Target.Body;
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
                        throw new Exception("Invalid ret OpCode index" + ri + " in method " + Target);

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

                    return ce == i.Operand;
                }).ToArray();

                if (cp < 0 || cp >= cs.Length)
                    throw new Exception("Invalid call index " + cp + " to method " + ce + " in method " + Target);

                return new Instruction(Target, Position, ToInject, cs[cp].Offset);
            }

            throw new Exception("Wat? InjectionData is of unexpected deriving type " + GetType());
        }
    }
}
