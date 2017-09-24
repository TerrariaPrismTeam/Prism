using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector.Patcher
{
    static class ILHacks
    {
        static DNContext context;
        static MemberResolver memRes;

        static ICorLibTypes typeSys;
        static TypeDef typeDef_ILHacks;

        static void AddCpblk()
        {
            var cpblk = new MethodDefUser("Cpblk", MethodSig.CreateStatic(typeSys.Void, typeSys.IntPtr, typeSys.IntPtr, typeSys.Int32),
                MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Static);
            cpblk.Parameters[0].Name = "dest";
            cpblk.Parameters[1].Name = "src" ;
            cpblk.Parameters[1].Name = "size";

            cpblk.Body = new CilBody();

            using (var ilp = cpblk.Body.GetILProcessor())
            {
                ilp.Emit(OpCodes.Ldarg_0);
                ilp.Emit(OpCodes.Ldarg_1);
                ilp.Emit(OpCodes.Ldarg_2);

                ilp.Emit(OpCodes.Cpblk);

                ilp.Emit(OpCodes.Ret);
            }

            typeDef_ILHacks.Methods.Add(cpblk);
        }

        internal static void Patch(Action<string> log)
        {
            context = TerrariaPatcher.context;
            memRes  = TerrariaPatcher.memRes ;

            typeSys      = context.PrimaryAssembly.ManifestModule.CorLibTypes;

            typeDef_ILHacks = new TypeDefUser("Prism.Util", "ILHacks");
            typeDef_ILHacks.Attributes = TypeAttributes.Public | TypeAttributes.AutoClass
                | TypeAttributes.Sealed | TypeAttributes.Abstract;

            AddCpblk();

            context.PrimaryAssembly.ManifestModule.Types.Add(typeDef_ILHacks);
        }
    }
}

