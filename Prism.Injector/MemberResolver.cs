using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;

namespace Prism.Injector
{
    public class MemberResolver
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

        public MemberResolver(CecilContext context)
        {
            c_wr = new WeakReference(context);
        }

        [DebuggerStepThrough]
        public TypeDefinition GetType(string fullName)
        {
            return Context.primaryAssembly.types.FirstOrDefault(td => td.FullName == fullName);
        }
        [DebuggerStepThrough]
        public TypeDefinition GetType(Type t)
        {
            return Context.primaryAssembly.types.FirstOrDefault(td => Comparer.TypeEquals(td, t));
        }

        [DebuggerStepThrough]
        public bool HasTypeDefinition(Type t)
        {
            return Context.primaryAssembly.types.Any(td => Comparer.TypeEquals(td, t));
        }

        [DebuggerStepThrough]
        public TypeReference ReferenceOf(Type t)
        {
            return Context.PrimaryAssembly.MainModule.Import(t);
        }

        [DebuggerStepThrough]
        public FieldReference FieldOf   (FieldInfo fi)
        {
            return Context.PrimaryAssembly.MainModule.Import(fi);
        }
        [DebuggerStepThrough]
        public FieldReference FieldOf<T>(Expression<Func<T>> expr)
        {
            return FieldOf((FieldInfo)((MemberExpression)expr.Body).Member);
        }

        [DebuggerStepThrough]
        public MethodReference MethodOf(MethodInfo mi)
        {
            return Context.PrimaryAssembly.MainModule.Import(mi);
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
