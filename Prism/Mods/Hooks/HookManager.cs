using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Prism.API;
using Prism.API.Behaviours;
using Prism.Debugging;
using Prism.Util;

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
        /// <param name="containers">The hook containers to add hooks from.</param>
        /// <param name="methodName">The name of the hook.</param>
        /// <typeparam name="THookContainer">The type of the hook container.</typeparam>
        /// <typeparam name="TDelegate">The delegate type of the hook.</typeparam>
        /// <returns>The sorted hook list.</returns>
        public static IEnumerable<TDelegate> CreateHooks<THookContainer, TDelegate>(IEnumerable<THookContainer> containers, string methodName)
            where THookContainer : HookContainer
            where TDelegate : class
        {
            if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("TDelegate must be a delegate type, but it's a " + typeof(TDelegate) + "!", "TDelegate");

            var unsorted = new List<Tuple<double, Delegate>>();

            var hookMtdDecls = typeof(THookContainer).GetMethods(HookContainer.HookFlags);
            if (!hookMtdDecls.Any(m => m.Name == methodName))
            {
                Logging.LogWarning("Hook container " + typeof(THookContainer) + " does not contain hook " + methodName + ".");

                return Empty<TDelegate>.Array;
            }

            foreach (THookContainer container in containers)
            {
                MethodInfo[] hooks = container.GetHooks<THookContainer>();

                for (int i = 0; i < hooks.Length; i++)
                {
                    if (hooks[i].Name != methodName)
                        continue;

                    double priority = 0d;

                    object[] attrs;
                    if ((attrs = hooks[i].GetCustomAttributes(typeof(CallPriorityAttribute), true)) != null && attrs.Length == 1)
                        priority = ((CallPriorityAttribute)attrs[0]).Priority;

                    Delegate del = Delegate.CreateDelegate(typeof(TDelegate), container, hooks[i], false);
                    if (del != null)
                        unsorted.Add(new Tuple<double, Delegate>(priority, del));
                }
            }

            return unsorted.OrderBy(t => t.Item1).Select(t => t.Item2 as TDelegate).Where(d => d != null);
        }

        /// <summary>
        /// Calls a hook list with the specified parameters and returns their return values.
        /// </summary>
        /// <param name="delegates">The hook to invoke (1 hook per mod).</param>
        /// <param name="args">The arguments to pass to the hook. Please use null if the method doesn't take any arguments to prevent unnecessary memory allocation.</param>
        /// <returns>The return values of all called hooks.</returns>
        public static object[] Call(IEnumerable<Delegate> delegates, params object[] args)
        {
            if (delegates == null)
                throw new ArgumentNullException("delegates");

            if (!CanCallHooks && !delegates.IsEmpty())
            {
                var stackTrace = new StackTrace(1);
                var mtd = stackTrace.GetFrame(0).GetMethod();

                Logging.LogWarning("Tried to call hook '" + mtd.DeclaringType + "." + mtd + "' when hooks are disabled!");
                Logging.LogWarning(String.Join(String.Empty, stackTrace.GetFrames().Take(3)));

                return Empty<object>.Array;
            }

            // using this instead of a map |> toArray will make it easier to debug (no lazy eval. etc)
            object[] ret = new object[delegates.Count()];

            int i = 0;
            foreach (var del in delegates)
                ret[i++] = del.DynamicInvoke(args);

            return ret;
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
