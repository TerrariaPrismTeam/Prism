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
        /// <summary>
        /// Gets all hook methods of the current <see cref="HookContainer" /> instance.
        /// </summary>
        /// <returns>All hook methods of the current <see cref="HookContainer" /> instance.</returns>
        public MethodInfo[] GetHooks()
        {
            List<MethodInfo> ret = new List<MethodInfo>();

            Type t = GetType();
            MethodInfo[] all = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);

            object[] attrs;
            for (int i = 0; i < all.Length; i++)
                if ((attrs = all[i].GetCustomAttributes(typeof(HookAttribute), true)) != null && attrs.Length == 1)
                    ret.Add(all[i]);

            return ret.ToArray();
        }
    }
}
