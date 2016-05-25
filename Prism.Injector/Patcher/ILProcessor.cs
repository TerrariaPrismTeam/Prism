using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    public class ILProcessor : ICollection<Instruction>, IDisposable
    {
        CilBody body;
        IList<Instruction> instrs;

        public int Count
        {
            get
            {
                return instrs.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ILProcessor(CilBody body)
        {
            instrs = (this.body = body).Instructions;
        }

        public Instruction Create(OpCode op)
        {
            return op.ToInstruction();
        }
        public Instruction Create(OpCode op, byte v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, Local v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, Parameter v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, MethodSig v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, ITokenOperand v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, IMethod v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, IField v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, MemberRef v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, CorLibTypeSig v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, IEnumerable<Instruction> v)
        {
            return op.ToInstruction(v.ToArray());
        }
        public Instruction Create(OpCode op, IList<Instruction> v)
        {
            return op.ToInstruction(v.ToArray());
        }
        public Instruction Create(OpCode op, Instruction v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, double v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, float v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, long v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, int v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, sbyte v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, ITypeDefOrRef v)
        {
            return op.ToInstruction(v);
        }
        public Instruction Create(OpCode op, string v)
        {
            return op.ToInstruction(v);
        }

        public ILProcessor Emit(OpCode op)
        {
            return Append(op.ToInstruction());
        }
        public ILProcessor Emit(OpCode op, byte v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, Local v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, Parameter v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, MethodSig v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, ITokenOperand v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, IMethod v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, IField v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, MemberRef v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, CorLibTypeSig v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, IEnumerable<Instruction> v)
        {
            return Append(op.ToInstruction(v.ToArray()));
        }
        public ILProcessor Emit(OpCode op, IList<Instruction> v)
        {
            return Append(op.ToInstruction(v.ToArray()));
        }
        public ILProcessor Emit(OpCode op, Instruction v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, double v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, float v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, long v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, int v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, sbyte v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, ITypeDefOrRef v)
        {
            return Append(op.ToInstruction(v));
        }
        public ILProcessor Emit(OpCode op, string v)
        {
            return Append(op.ToInstruction(v));
        }

        public Instruction Previous(Instruction ins)
        {
            var ind = instrs.IndexOf(ins);
            return ind < 1 ? null : instrs[ind - 1];
        }
        public Instruction Next    (Instruction ins)
        {
            var ind = instrs.IndexOf(ins);
            return ind < 0 || ind >= instrs.Count - 1 ? null : instrs[ind + 1];
        }

        public ILProcessor InsertBefore(int ind, Instruction toIns)
        {
            instrs.Insert(ind, toIns);

            return this;
        }
        public ILProcessor InsertAfter(int ind, Instruction toIns)
        {
            instrs.Insert(ind + 1, toIns);

            return this;
        }
        public ILProcessor InsertBefore(Instruction ins, Instruction toIns)
        {
            var ind = instrs.IndexOf(ins);
            instrs.Insert(ind, toIns);

            return this;
        }
        public ILProcessor InsertAfter(Instruction ins, Instruction toIns)
        {
            var ind = instrs.IndexOf(ins);
            instrs.Insert(ind + 1, toIns);
            
            return this;
        }

        public ILProcessor Append(Instruction ins)
        {
            instrs.Add(ins);
            
            return this;
        }
        public ILProcessor Prepend(Instruction ins)
        {
            instrs.Insert(0, ins);
            
            return this;
        }

        public bool Remove(Instruction ins)
        {
            return instrs.Remove(ins);
        }

        public ILProcessor Replace(Instruction toRem, Instruction toIns)
        {
            var ind = instrs.IndexOf(toRem);

            if (ind == -1)
                return this;

            instrs[ind] = toIns;

            return this;
        }

        public ILProcessor InsertBeforeMany(Instruction target, IEnumerable<Instruction> toIns)
        {
            var tari = instrs.IndexOf(target);

            if (tari == -1)
                return this;

            foreach (var i in toIns)
                InsertBefore(tari++, i);

            return this;
        }
        public ILProcessor InsertAftereMany(Instruction target, IEnumerable<Instruction> toIns)
        {
            var tari = instrs.IndexOf(target) + 1;

            if (tari == 0)
                return this;

            foreach (var i in toIns)
                InsertBefore(tari++, i);

            return this;
        }

        public ILProcessor AppendMany(IEnumerable<Instruction> toIns)
        {
            foreach (var i in toIns)
                instrs.Add(i);

            return this;
        }
        public ILProcessor PrependMany(IEnumerable<Instruction> toIns)
        {
            if (instrs.Count == 0)
                return AppendMany(toIns); // doesn't make a diff in this case

            var tari = 0;

            foreach (var i in toIns)
                InsertBefore(tari++, i);

            return this;
        }
        public ILProcessor RemoveMany(IEnumerable<Instruction> toRem)
        {
            foreach (var i in toRem)
                instrs.Remove(i);

            return this;
        }
        public ILProcessor RemoveMany(Instruction first, int count)
        {
            var tari = instrs.IndexOf(first);

            for (int i = 0; tari != -1 && i < count; i++)
                instrs.RemoveAt(tari);

            return this;
        }
        
        public IEnumerator<Instruction> GetEnumerator()
        {
            return instrs.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return instrs.GetEnumerator();
        }

        void ICollection<Instruction>.Add(Instruction item)
        {
            Append(item);
        }

        public void Clear()
        {
            instrs.Clear();
        }

        public bool Contains(Instruction item)
        {
            return instrs.Contains(item);
        }

        public void CopyTo(Instruction[] array, int arrayIndex)
        {
            instrs.CopyTo(array, arrayIndex);
        }

        public void Dispose()
        {
            body.UpdateInstructionOffsets();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;

            if (obj is ILProcessor)
                return instrs.Equals(((ILProcessor)obj).instrs);
            if (obj is LinkedList<Instruction>)
                return instrs.Equals((LinkedList<Instruction>)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return instrs.GetHashCode();
        }
        public override string ToString()
        {
            return instrs.ToString();
        }
    }

    public static class ILProcExts
    {
        public static ILProcessor GetILProcessor(this CilBody body)
        {
            return new ILProcessor(body);
        }

        public static Instruction Previous(this Instruction instr, ILProcessor proc)
        {
            return proc.Previous(instr);
        }
        public static Instruction Next    (this Instruction instr, ILProcessor proc)
        {
            return proc.Next(instr);
        }
    }
}
