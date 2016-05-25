using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;

namespace Prism.Injector
{
    public struct DNReflectionComparer
    {
        WeakReference c_wr;

        public DNContext Context
        {
            get
            {
                if (c_wr.IsAlive)
                    return (DNContext)c_wr.Target;

                throw new ObjectDisposedException("Context");
            }
        }

        public DNReflectionComparer(DNContext context)
        {
            c_wr = new WeakReference(context);
        }

        [DebuggerStepThrough]
        public bool AssemblyEquals(AssemblyDef ad, Assembly a)
        {
            return ad.FullName == a.GetName().FullName;
        }
        public bool TypeEquals(ITypeDefOrRef td, Type t)
        {
            if (!AssemblyEquals(td.Module.Assembly, t.Assembly) || td.FullName != t.FullName)
                return false;

            return TypeEquals(td.ResolveTypeDefThrow(), t);
        }
        public bool TypeEquals(TypeDef td, Type t)
        {
            if (!AssemblyEquals(td.Module.Assembly, t.Assembly) || td.FullName != t.FullName)
                return false;

            return td.GenericParameters.Count == (t.IsGenericType ? t.GetGenericArguments().Length : 0);
        }
        public bool TypeEquals(TypeSig td, Type t)
        {
            if (!AssemblyEquals(td.Module.Assembly, t.Assembly) || td.FullName != t.FullName)
                return false;

            return TypeEquals(td.TryGetTypeRef(), t);
        }

        [DebuggerStepThrough]
        public bool MemberEquals(IMemberDef md, MemberInfo mi)
        {
            return TypeEquals(md.DeclaringType, mi.DeclaringType) && md.Name == mi.Name;
        }

        public bool FieldEquals(FieldDef fd, FieldInfo fi)
        {
            return MemberEquals(fd, fi) && TypeEquals(fd.FieldType, fi.FieldType) && (int)fd.Attributes == (int)fi.Attributes; // visibility, static?, etc
        }
        public bool MethodEquals(MethodDef md, MethodInfo mi)
        {
            if (!MemberEquals(md, mi) || !TypeEquals(md.ReturnType, mi.ReturnType) || (int)md.Attributes != (int)mi.Attributes)
                return false;

            var pis = mi.GetParameters();
            if (md.Parameters.Count != pis.Length)
                return false;

            for (int i = 0; i < pis.Length; i++)
                if (!TypeEquals(md.Parameters[i].Type, pis[i].ParameterType))
                    return false;

            if (!mi.IsGenericMethod)
                return md.GenericParameters.Count == 0;

            if (mi.GetGenericArguments().Length != md.GenericParameters.Count)
                return false;

            return mi.GetGenericArguments().Length == md.GenericParameters.Count;
        }
        public bool PropertyEquals(PropertyDef pd, PropertyInfo pi)
        {
            if (!MemberEquals(pd, pi) || TypeEquals(pd.PropertySig.RetType, pi.PropertyType) || (int)pd.Attributes != (int)pi.Attributes)
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
