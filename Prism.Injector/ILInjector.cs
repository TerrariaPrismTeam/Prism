using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Prism.Injector
{
    public static class ILInjector
    {
        //static Instruction Clone(Instruction i)
        //{
        //    var oc = i.OpCode ;
        //    var op = i.Operand;

        //    if (op == null)
        //        return Instruction.Create(oc);

        //    if (op is byte  )
        //        return Instruction.Create(oc, (byte  )op);
        //    if (op is sbyte )
        //        return Instruction.Create(oc, (sbyte )op);
        //    if (op is int   )
        //        return Instruction.Create(oc, (int   )op);
        //    if (op is long  )
        //        return Instruction.Create(oc, (long  )op);
        //    if (op is float )
        //        return Instruction.Create(oc, (float )op);
        //    if (op is double)
        //        return Instruction.Create(oc, (double)op);
        //    if (op is string)
        //        return Instruction.Create(oc, (string)op);

        //    if (op is CallSite)
        //    {
        //        var cs = (CallSite)op;

        //        return Instruction.Create(oc, new CallSite(cs.ReturnType));
        //    }
        //    if (op is FieldReference)
        //    {
        //        var fr = (FieldReference)op;

        //        return Instruction.Create(oc, new FieldReference(fr.Name, fr.FieldType, fr.DeclaringType));
        //    }
        //    if (op is Instruction)
        //        return Instruction.Create(oc, Clone((Instruction)op));
        //    if (op is VariableDefinition)
        //    {
        //        var vd = (VariableDefinition)op;

        //        return Instruction.Create(oc, new VariableDefinition(vd.Name, vd.VariableType));
        //    }
        //    if (op is ParameterDefinition)
        //    {
        //        var pd = (ParameterDefinition)op;

        //        return Instruction.Create(oc, new ParameterDefinition(pd.Name, pd.Attributes, pd.ParameterType));
        //    }
        //    if (op is Instruction[])
        //    {
        //        var ia = (Instruction[])op;

        //        return Instruction.Create(oc, Array.ConvertAll(ia, Clone));
        //    }
        //    if (op is MethodReference)
        //    {
        //        var mr = (MethodReference)op;

        //        return Instruction.Create(oc, new MethodReference(mr.Name, mr.ReturnType, mr.DeclaringType));
        //    }
        //    if (op is TypeReference)
        //    {
        //        var tr = (TypeReference)op;

        //        return Instruction.Create(oc, new TypeReference(tr.Namespace, tr.Name, tr.Module, tr.Scope, tr.IsValueType));
        //    }

        //    return Instruction.Create(oc);
        //}
        //[DebuggerStepThrough]
        //static MethodDefinition ResolveMethod(AssemblyDefinition ad, InjectionData id, MethodRef mi)
        //{
        //    var t = ad.MainModule.Types.FirstOrDefault(td => td.FullName == mi.Type);

        //    if (t == null)
        //        return null;

        //    var methods = t.Methods.Where(md => md.Name == id.Target.Name).ToArray();

        //    if (methods.Length < 2)
        //        return methods.FirstOrDefault();

        //    if (id.Target.Overload > -1 && id.Target.Overload < methods.Length)
        //        return methods[id.Target.Overload];

        //    return methods.First();
        //}

        public static void Inject(AssemblyDefinition ad, IEnumerable<InjectionData> toInject)
        {
            foreach (var id in toInject)
            {
                //var t = ResolveMethod(ad, id, id.Target);
                var t = id.Target;

                var instr = id.ToInstruction();// mi => ResolveMethod(ad, id, mi));

                var b = t.Body;
                var p = b.GetILProcessor();

                var tar = instr.IsIndex
                    ? (instr.Index > t.Body.Instructions.Count ? null : t.Body.Instructions[instr.Index])
                    :  t.Body.Instructions.FirstOrDefault(i => i.Offset == instr.Offset);

                if (tar == null)
                    throw new Exception("Invalid target instruction index/offset: " + instr.IndexOffset + " in method " + t.FullName + ".");

                //var ninsts = Array.ConvertAll(instr.ToInject, Clone);
                var ninsts = instr.ToInject;

                var emit = instr.Position == InjectionPosition.Post
                    ? (Action<Instruction>)(i => p.InsertAfter (tar, i))
                    :                       i => p.InsertBefore(tar, i) ;

                for (int i = ninsts.Length - 1; i >= 0; i--) // reverse order, otherwise it will blow up
                    emit(ninsts[i]);
            }
        }
    }
}
