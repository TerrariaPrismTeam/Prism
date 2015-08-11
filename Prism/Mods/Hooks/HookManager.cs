using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API;
using Prism.API.Behaviours;

namespace Prism.Mods.Hooks
{
    /// <summary>
    /// Manages hooks.
    /// </summary>
    public interface IHookManager
    {
        /// <summary>
        /// Creates the hooks.
        /// </summary>
        void Create();
        /// <summary>
        /// Clears the hooks.
        /// </summary>
        void Clear ();
    }

    public static class HookManager
    {
        internal static Dictionary<Type, IHookManager> managers = new Dictionary<Type, IHookManager>();

        static bool canCallHooks = false; // see ModLoader
        internal static bool CanCallHooks
        {
            get
            {
                return canCallHooks && !ModLoader.Reloading;
            }
            set
            {
                canCallHooks = value;
            }
        }

        internal static void Create()
        {
            RegisterManager(typeof(ModDef       ), ModDef        = new ModDefHooks());
            RegisterManager(typeof(GameBehaviour), GameBehaviour = new GameHooks  ());

            foreach (var v in managers.Values)
                v.Create();
        }
        internal static void Clear ()
        {
            foreach (var v in managers.Values)
                v.Clear();

            managers.Clear();

            ModDef        = null;
            GameBehaviour = null;
        }

        /// <summary>
        /// Create a hook delegate list from a collection of HookContainers.
        /// </summary>
        /// <param name="types">The hook containers to add hooks from.</param>
        /// <param name="methodName">The name of the hook.</param>
        /// <typeparam name="THookContainer">The type of the hook container.</typeparam>
        /// <typeparam name="TDelegate">The delegate type of the hook.</typeparam>
        /// <returns>The sorted hook list.</returns>
        public static IEnumerable<TDelegate> CreateHooks<THookContainer, TDelegate>(IEnumerable<THookContainer> types, string methodName)
            where THookContainer : HookContainer
            where TDelegate : class
        {
            if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("TDelegate must be a delegate type, but it's a " + typeof(TDelegate) + "!", "TDelegate");

            List<Tuple<double, Delegate>> nonSorted = new List<Tuple<double, Delegate>>();

            foreach (THookContainer container in types)
            {
                MethodInfo[] methods = container.GetHooks();

                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name != methodName)
                        continue;

                    double priority = 0d;

                    object[] attrs;
                    if ((attrs = methods[i].GetCustomAttributes(typeof(CallPriorityAttribute), true)) != null && attrs.Length == 1)
                        priority = ((CallPriorityAttribute)attrs[0]).Priority;

                    Delegate del = Delegate.CreateDelegate(typeof(TDelegate), container, methods[i], false);
                    if (del != null)
                        nonSorted.Add(new Tuple<double, Delegate>(priority, del));
                }
            }

            return nonSorted.OrderBy(t => t.Item1).Select(t => t.Item2 as TDelegate).Where(d => d != null);
        }

        /// <summary>
        /// Calls a hook list with the specified parameters and returns their return values.
        /// </summary>
        /// <param name="delegates">The hook to invoke (1 hook per mod).</param>
        /// <param name="args">The arguments to pass to the hook. Please use null if the method doesn't take any arguments to prevent unnecessary memory allocation.</param>
        /// <returns>The return values of all called hooks.</returns>
        public static object[] Call(IEnumerable<Delegate> delegates, params object[] args)
        {
            if (!CanCallHooks)
                return new object[0];

            return delegates.Select(del => del.DynamicInvoke(args)).ToArray();
        }

        /// <summary>
        /// Registers an IHookManager.
        /// </summary>
        /// <param name="containerType">The type of the <see cref="HookContainer" /> the <see cref="IHookManager" /> manages.</param>
        /// <param name="hookMgr">The IHookManager to register.</param>
        public static void RegisterManager(Type containerType, IHookManager hookMgr)
        {
            managers.Add(containerType, hookMgr);
        }

        internal static ModDefHooks ModDef;
        internal static GameHooks GameBehaviour;
    }
}
