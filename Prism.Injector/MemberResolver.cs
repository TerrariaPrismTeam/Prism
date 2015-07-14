using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;

namespace Prism.Injector
{
    public class MetadataResolver
    {
        const BindingFlags ALL_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

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
        public CecilReflectionComparer Comparer
        {
            get
            {
                return Context.Comparer;
            }
        }

        public MetadataResolver(CecilContext context)
        {
            c_wr = new WeakReference(context);
        }

        [DebuggerStepThrough]
        AsmInfo InfoOf(AssemblyDefinition def)
        {
            if (def == Context.PrimaryAssembly)
                return Context.primaryAssembly;

            return Context.stdLibAsms.FirstOrDefault(ai => ai.assembly == def);
        }

        [DebuggerStepThrough]
        public AssemblyDefinition GetAssembly(string displayName)
        {
            if (displayName == Context.PrimaryAssembly.Name.Name)
                return Context.PrimaryAssembly;

            return Context.stdLibAsms.FirstOrDefault(ai => ai.assembly.Name.Name == displayName).assembly;
        }

        [DebuggerStepThrough]
        public TypeDefinition GetType(string fullName, string asmDisplayName = null)
        {
            return GetType(fullName, asmDisplayName == null ? null : GetAssembly(asmDisplayName));
        }
        [DebuggerStepThrough]
        public TypeDefinition GetType(string fullName, AssemblyDefinition asm = null)
        {
            IEnumerable<AsmInfo> i = asm == null
                ? Context.AllDefinedAssemblies.Select(ad => InfoOf(ad)).Where(ai => ai != null)
                : new[] { InfoOf(asm) };

            foreach (var ai in i)
            {
                var fod = ai.types.FirstOrDefault(td => td.FullName == fullName);
                if (fod != null)
                    return fod;
            }

            return null;
        }

        [DebuggerStepThrough]
        public bool HasAssemblyDef(Assembly a)
        {
            var an = a.GetName().FullName;

            if (Comparer.AssemblyEquals(Context.PrimaryAssembly, a))
                return true;

            return Context.StdLibReferences.Any(ad => Comparer.AssemblyEquals(ad, a));
        }
        [DebuggerStepThrough]
        public bool HasTypeDefinition(Type t)
        {
            if (!HasAssemblyDef(t.Assembly))
                return false;

            return Context.primaryAssembly.types.Any(td => Comparer.TypeEquals(td, t))
                || Context.stdLibAsms.Any(ai => ai.types.Any(td => Comparer.TypeEquals(td, t)));
        }

        [DebuggerStepThrough]
        public AssemblyDefinition DefinitionOf(Assembly a)
        {
            if (!HasAssemblyDef(a))
                return null;

            if (Comparer.AssemblyEquals(Context.PrimaryAssembly, a))
                return Context.PrimaryAssembly;

            return Context.StdLibReferences.FirstOrDefault(ad => Comparer.AssemblyEquals(ad, a));
        }
        [DebuggerStepThrough]
        public TypeDefinition DefinitionOf(Type t)
        {
            if (!HasTypeDefinition(t))
                return null;

            return Context.primaryAssembly.types.FirstOrDefault(td => Comparer.TypeEquals(td, t))
                ?? Context.stdLibAsms.Select(ai => ai.types.FirstOrDefault(td => Comparer.TypeEquals(td, t))).FirstOrDefault();
        }
        [DebuggerStepThrough]
        public TypeReference ReferenceOf(Type t)
        {
            return Context.PrimaryAssembly.MainModule.Import(t);
            //var fod = Context.loadedRefTypes.FirstOrDefault(tr =>
            //    Comparer.AssemblyEquals(tr.Module.Assembly, t.Assembly) && tr.FullName == t.FullName);
            //if (fod != null)
            //    return fod;

            //if (HasTypeDefinition(t))
            //    return DefinitionOf(t);

            //if (!HasAssemblyDef(t.Assembly))
            //    return null;

            //// meh
            //return null;
        }

        [DebuggerStepThrough]
        public FieldReference FieldOf   (FieldInfo fi)
        {
            return Context.PrimaryAssembly.MainModule.Import(fi);
            //var td = DefinitionOf(fi.DeclaringType);
            //if (td != null)
            //{
            //    var r = td.Fields.FirstOrDefault(fd => Comparer.FieldEquals(fd, fi));
            //    if (r == null) return null;
            //    return Context.PrimaryAssembly.MainModule.Import(r);
            //}
            //return null;
        }
        [DebuggerStepThrough]
        public FieldReference FieldOf<T>(Expression<Func<T>> expr)
        {
            return FieldOf((FieldInfo)((MemberExpression)expr.Body).Member);
        }

        [DebuggerStepThrough]
        public PropertyReference PropertyOf   (PropertyInfo pi)
        {
            var td = DefinitionOf(pi.DeclaringType);
            if (td != null)
                return td.Properties.FirstOrDefault(pd => Comparer.PropertyEquals(pd, pi));

            return null;
        }
        [DebuggerStepThrough]
        public PropertyReference PropertyOf<T>(Expression<Func<T>> expr)
        {
            return PropertyOf((PropertyInfo)((MemberExpression)expr.Body).Member);
        }

        [DebuggerStepThrough]
        public MethodReference MethodOf(MethodInfo mi)
        {
            return Context.PrimaryAssembly.MainModule.Import(mi);
            //var td = DefinitionOf(mi.DeclaringType);
            //if (td != null)
            //{
            //    var r = td.Methods.FirstOrDefault(md => Comparer.MethodEquals(md, mi));
            //    if (r == null) return null;
            //    return Context.PrimaryAssembly.MainModule.Import(r);
            //}
            //return null;
        }
        [DebuggerStepThrough]
        public MethodReference MethodOf<TDelegate>(TDelegate @delegate)
            where TDelegate : class
        {
            if (!(@delegate is Delegate))
                throw new ArgumentException("Argument type should be a delegate.", "delegate");

            return MethodOf(((Delegate)(object)@delegate).Method);
        }

        #region MethodOf overloads of common delegate types (func, action, converter, predicate, eventhandler)
        [DebuggerStepThrough]
        public MethodReference MethodOfA                               (Action                                act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T                            >(Action<T                            > act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T, T2                        >(Action<T, T2                        > act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T, T2, T3                    >(Action<T, T2, T3                    > act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T, T2, T3, T4                >(Action<T, T2, T3, T4                > act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T, T2, T3, T4, T5            >(Action<T, T2, T3, T4, T5            > act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T, T2, T3, T4, T5, T6        >(Action<T, T2, T3, T4, T5, T6        > act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T, T2, T3, T4, T5, T6, T7    >(Action<T, T2, T3, T4, T5, T6, T7    > act)
        {
            return MethodOf(act.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfA<T, T2, T3, T4, T5, T6, T7, T8>(Action<T, T2, T3, T4, T5, T6, T7, T8> act)
        {
            return MethodOf(act.Method);
        }

        [DebuggerStepThrough]
        public MethodReference MethodOfF<                               TResult>(Func<                               TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T,                             TResult>(Func<T,                             TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T, T2,                         TResult>(Func<T, T2,                         TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T, T2, T3,                     TResult>(Func<T, T2, T3,                     TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T, T2, T3, T4,                 TResult>(Func<T, T2, T3, T4,                 TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T, T2, T3, T4, T5,             TResult>(Func<T, T2, T3, T4, T5,             TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T, T2, T3, T4, T5, T6,         TResult>(Func<T, T2, T3, T4, T5, T6,         TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T, T2, T3, T4, T5, T6, T7,     TResult>(Func<T, T2, T3, T4, T5, T6, T7,     TResult> func)
        {
            return MethodOf(func.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfF<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T, T2, T3, T4, T5, T6, T7, T8, TResult> func)
        {
            return MethodOf(func.Method);
        }

        [DebuggerStepThrough]
        public MethodReference MethodOfE        (EventHandler         handler)
        {
            return MethodOf(handler.Method);
        }
        [DebuggerStepThrough]
        public MethodReference MethodOfE<TEvent>(EventHandler<TEvent> handler)
            where TEvent : EventArgs
        {
            return MethodOf(handler.Method);
        }

        [DebuggerStepThrough]
        public MethodReference MethodOf<TIn, TOut>(Converter<TIn, TOut> conv)
        {
            return MethodOf(conv.Method);
        }

        [DebuggerStepThrough]
        public MethodReference MethodOf<T>(Predicate<T> pred)
        {
            return MethodOf(pred.Method);
        }
        #endregion
    }
}
