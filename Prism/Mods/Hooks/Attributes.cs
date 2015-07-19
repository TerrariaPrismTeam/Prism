using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Mods.Hooks
{
    /// <summary>
    /// Specifies that the method is used as a hook.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HookAttribute : Attribute { }
    /// <summary>
    /// Specifies the priority of the hook.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CallPriorityAttribute : Attribute
    {
        public float Priority
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies the priority of the hook.
        /// </summary>
        public CallPriorityAttribute(float priority)
        {
            if (Single.IsNaN(priority))
                throw new NotFiniteNumberException(priority);

            Priority = priority;
        }
    }
}
