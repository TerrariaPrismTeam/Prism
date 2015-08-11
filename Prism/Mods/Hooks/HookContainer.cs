using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Prism.Mods.Hooks
{
    /// <summary>
    /// Base class for all types that contain hook methods.
    /// </summary>
    public abstract class HookContainer
    {
        internal const BindingFlags HookFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Gets all hook methods of the current <see cref="HookContainer" /> instance.
        /// </summary>
        /// <returns>All hook methods of the current <see cref="HookContainer" /> instance.</returns>
        public MethodInfo[] GetHooks<THookContainer>()
            where THookContainer : HookContainer
        {
            List<MethodInfo> ret = new List<MethodInfo>();

            Type t = GetType();
            MethodInfo[] all = t.GetMethods(HookFlags);

            object[] attrs;
            for (int i = 0; i < all.Length; i++)
                if ((attrs = all[i].GetCustomAttributes(typeof(HookAttribute), true)) != null && attrs.Length == 1 && all[i].DeclaringType != typeof(THookContainer) && !typeof(THookContainer).IsSubclassOf(all[i].DeclaringType))
                    ret.Add(all[i]);

            return ret.ToArray();
        }
    }
}
