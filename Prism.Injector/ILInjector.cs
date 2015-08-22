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
        public static void Inject(IEnumerable<InjectionData> toInject)
        {
            foreach (var id in toInject)
            {
                var t = id.Target;

                var instr = id.ToInstruction();

                var b = t.Body;
                var p = b.GetILProcessor();

                var tar = instr.IsIndex
                    ? (instr.Index > t.Body.Instructions.Count ? null : t.Body.Instructions[instr.Index])
                    :  t.Body.Instructions.FirstOrDefault(i => i.Offset == instr.Offset);

                if (tar == null)
                    throw new Exception("Invalid target instruction index/offset: " + instr.IndexOffset + " in method " + t.FullName + ".");

                var ninsts = instr.ToInject;

                if (instr.Position == InjectionPosition.Pre)
                    for (int i = 0; i < ninsts.Length; i++)
                        p.InsertBefore(tar, ninsts[i]);
                else
                    for (int i = ninsts.Length - 1; i >= 0; i--) // reverse order, otherwise it will blow up
                        p.InsertAfter (tar, ninsts[i]);
            }
        }
    }
}
