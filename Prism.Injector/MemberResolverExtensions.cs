using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Prism.Injector
{
    [Flags]
    public enum MethodFlags : byte
    {
        Public    =  1,
        NonPublic =  2,

        Instance  =  4,
        Static    =  8,

        All       = 15
    }

    public static class MemberResolverExtensions
    {
        readonly static SigComparer comp = new SigComparer(SigComparerOptions.PrivateScopeIsComparable);

        readonly static string CTOR = ".ctor", CCTOR = ".cctor";

        [DebuggerStepThrough]
        public static FieldDef GetField(this TypeDef type, string name)
        {
            return type.Fields.FirstOrDefault(fd => fd.Name == name);
        }
        [DebuggerStepThrough]
        public static PropertyDef GetProperty(this TypeDef type, string name)
        {
            return type.Properties.FirstOrDefault(pd => pd.Name == name);
        }

        [DebuggerStepThrough]
        public static MethodDef[] GetMethods(this TypeDef type, string name, MethodFlags flags, TypeSig[] arguments)
        {
            return type.GetMethods(name, flags, arguments.Select(ts => ts.ToTypeDefOrRef()).ToArray());
        }
        [DebuggerStepThrough]
        public static MethodDef[] GetMethods(this TypeDef type                   , string name, MethodFlags flags = MethodFlags.All, params ITypeDefOrRef[] arguments)
        {
            bool argsSpec = arguments != null && arguments.Length > 0;

            return type.Methods.Where(md =>
            {
                if (md.Name != name)
                    return false;

                if ( md.IsPublic && (flags & MethodFlags.   Public) == 0)
                    return false;
                if (!md.IsPublic && (flags & MethodFlags.NonPublic) == 0)
                    return false;

                if ( md.IsStatic && (flags & MethodFlags.Static  ) == 0)
                    return false;
                if (!md.IsStatic && (flags & MethodFlags.Instance) == 0)
                    return false;

                if (!argsSpec)
                    return true;

                var start = md.IsStatic ? 0 : 1;

                if (arguments.Length + start != md.Parameters.Count)
                    return false;

                for (int i = 0; i < arguments.Length; i++)
                    if (!comp.Equals(arguments[i], md.Parameters[i + start].Type))
                        return false;

                return true;
            }).ToArray();
        }
        [DebuggerStepThrough]
        public static MethodDef[] GetMethods(this TypeDef type, DNContext context, string name, MethodFlags flags = MethodFlags.All, params Type         [] arguments)
        {
            return GetMethods(type, name, flags, arguments.Select(t =>
            {
                if (context.RefComparer.AssemblyEquals(type.Module.Assembly, t.Assembly))
                    return context.Resolver.GetType(t);

                return context.PrimaryAssembly.ManifestModule.Import(t);
            }).ToArray());
        }
        
        [DebuggerStepThrough]
        public static MethodDef GetMethod(this TypeDef type, string name, MethodFlags flags, TypeSig[] arguments)
        {
            return type.GetMethod(name, flags, arguments.Select(ts => ts.ToTypeDefOrRef()).ToArray());
        }
        [DebuggerStepThrough]
        public static MethodDef GetMethod(this TypeDef type                   , string name, MethodFlags flags = MethodFlags.All, params ITypeDefOrRef[] arguments)
        {
            return GetMethods(type         , name, flags, arguments).FirstOrDefault();
        }
        [DebuggerStepThrough]
        public static MethodDef GetMethod(this TypeDef type, DNContext context, string name, MethodFlags flags = MethodFlags.All, params Type         [] arguments)
        {
            return GetMethods(type, context, name, flags, arguments).FirstOrDefault();
        }

        [DebuggerStepThrough]
        public static MethodDef GetConstructor(this TypeDef type, bool isNonPublic, TypeSig[] arguments)
        {
            return GetMethod(type, CTOR, (isNonPublic ? MethodFlags.NonPublic : MethodFlags.Public) | MethodFlags.Instance, arguments.Select(ts => ts.ToTypeDefOrRef()).ToArray());
        }
        [DebuggerStepThrough]
        public static MethodDef GetConstructor(this TypeDef type                   , bool isNonPublic = false, params ITypeDefOrRef[] arguments)
        {
            return GetMethod(type         , CTOR, (isNonPublic ? MethodFlags.NonPublic : MethodFlags.Public) | MethodFlags.Instance, arguments);
        }
        [DebuggerStepThrough]
        public static MethodDef GetConstructor(this TypeDef type, DNContext context, bool isNonPublic = false, params Type         [] arguments)
        {
            return GetMethod(type, context, CTOR, (isNonPublic ? MethodFlags.NonPublic : MethodFlags.Public) | MethodFlags.Instance, arguments);
        }

        [DebuggerStepThrough]
        public static MethodDef GetOrCreateStaticCtor(this TypeDef type, Action<CilBody> onCreate = null)
        {
            var cctor = type.Methods.FirstOrDefault(m => m.Name == CCTOR);

            if (cctor != null)
                return cctor;

            cctor = new MethodDefUser(CCTOR, MethodSig.CreateInstance(type.Module.CorLibTypes.Void), MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            var cb = cctor.Body = new CilBody();

            if (onCreate != null)
                onCreate(cb);

            cb.Instructions.Add(Instruction.Create(OpCodes.Ret));

            return cctor;
        }
    }
}
