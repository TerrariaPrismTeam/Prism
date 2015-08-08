using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;

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
        [DebuggerStepThrough]
        public static FieldDefinition GetField(this TypeDefinition type, string name)
        {
            return type.Fields.FirstOrDefault(fd => fd.Name == name);
        }
        [DebuggerStepThrough]
        public static PropertyDefinition GetProperty(this TypeDefinition type, string name)
        {
            return type.Properties.FirstOrDefault(pd => pd.Name == name);
        }

        [DebuggerStepThrough]
        public static MethodDefinition[] GetMethods(this TypeDefinition type                      , string name, MethodFlags flags = MethodFlags.All, params TypeReference[] arguments)
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

                if (arguments.Length != md.Parameters.Count)
                    return false;

                for (int i = 0; i < arguments.Length; i++)
                    if (arguments[i] != md.Parameters[i].ParameterType)
                        return false;

                return true;
            }).ToArray();
        }
        [DebuggerStepThrough]
        public static MethodDefinition[] GetMethods(this TypeDefinition type, CecilContext context, string name, MethodFlags flags = MethodFlags.All, params Type         [] arguments)
        {
            return GetMethods(type, name, flags, arguments.Select(t =>
            {
                if (context.Comparer.AssemblyEquals(type.Module.Assembly, t.Assembly))
                    return context.Resolver.GetType(t);

                return context.PrimaryAssembly.MainModule.Import(t);
            }).ToArray());
        }

        [DebuggerStepThrough]
        public static MethodDefinition GetMethod(this TypeDefinition type                      , string name, MethodFlags flags = MethodFlags.All, params TypeReference[] arguments)
        {
            return GetMethods(type         , name, flags, arguments).FirstOrDefault();
        }
        [DebuggerStepThrough]
        public static MethodDefinition GetMethod(this TypeDefinition type, CecilContext context, string name, MethodFlags flags = MethodFlags.All, params Type         [] arguments)
        {
            return GetMethods(type, context, name, flags, arguments).FirstOrDefault();
        }
    }
}
