using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Prism.Injector
{
    public struct CecilReflectionComparer
    {
        WeakReference c_wr;

        public CecilContext Context
        {
            get
            {
                if (c_wr.IsAlive)
                    return (CecilContext)c_wr.Target;

                throw new ObjectDisposedException("Context");
            }
        }

        public CecilReflectionComparer(CecilContext context)
        {
            c_wr = new WeakReference(context);
        }

        [DebuggerStepThrough]
        public bool AssemblyEquals(AssemblyDefinition ad, Assembly a)
        {
            return ad.Name.FullName == a.GetName().FullName;
        }
        public bool TypeEquals(TypeReference td, Type t)
        {
            if (!AssemblyEquals(td.Module.Assembly, t.Assembly) || td.FullName != t.FullName)
                return false;

            return td.GenericParameters.Count == (t.IsGenericType ? t.GetGenericArguments().Length : 0);
        }

        [DebuggerStepThrough]
        public bool MemberEquals(IMemberDefinition md, MemberInfo mi)
        {
            return TypeEquals(md.DeclaringType, mi.DeclaringType) && md.Name == mi.Name;
        }

        public bool FieldEquals(FieldDefinition fd, FieldInfo fi)
        {
            return MemberEquals(fd, fi) && TypeEquals(fd.FieldType, fi.FieldType) && (int)fd.Attributes == (int)fi.Attributes; // visibility, static?, etc
        }
        public bool MethodEquals(MethodDefinition md, MethodInfo mi)
        {
            if (!MemberEquals(md, mi) || !TypeEquals(md.ReturnType, mi.ReturnType) || (int)md.Attributes != (int)mi.Attributes)
                return false;

            var pis = mi.GetParameters();
            if (md.Parameters.Count != pis.Length)
                return false;

            for (int i = 0; i < pis.Length; i++)
                if (!TypeEquals(md.Parameters[i].ParameterType, pis[i].ParameterType))
                    return false;

            if (!mi.IsGenericMethod)
                return md.GenericParameters.Count == 0;

            if (mi.GetGenericArguments().Length != md.GenericParameters.Count)
                return false;

            return mi.GetGenericArguments().Length == md.GenericParameters.Count;
        }
        public bool PropertyEquals(PropertyDefinition pd, PropertyInfo pi)
        {
            if (!MemberEquals(pd, pi) || TypeEquals(pd.PropertyType, pi.PropertyType) || (int)pd.Attributes != (int)pi.Attributes)
                return false;

            var gm = pi.GetGetMethod();
            var sm = pi.GetSetMethod();

            if (gm == null ^ pd.GetMethod == null) return false;
            if (sm == null ^ pd.SetMethod == null) return false;

            return (gm == null || MethodEquals(pd.GetMethod, gm))
                && (sm == null || MethodEquals(pd.SetMethod, sm));
        }
    }
}
