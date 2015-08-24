using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.IO;
using Prism.Mods.Hooks;

namespace Prism.Mods.BHandlers
{
    public abstract class IOBHandler<TBehaviour> : IHookManager
        where TBehaviour : IOBehaviour
    {
        internal List<TBehaviour> behaviours = new List<TBehaviour>();

        IEnumerable<Action<BinBuffer>> save, load;

        internal Dictionary<string, BinBuffer> data = new Dictionary<string, BinBuffer>();

        public IEnumerable<TBehaviour> Behaviours
        {
            get
            {
                return behaviours;
            }
        }

        public virtual void Create()
        {
            save = HookManager.CreateHooks<TBehaviour, Action<BinBuffer>>(behaviours, "Save");
            load = HookManager.CreateHooks<TBehaviour, Action<BinBuffer>>(behaviours, "Load");
        }
        public virtual void Clear ()
        {
            save = load = null;
        }

        public void Save(BinBuffer bb)
        {
            // populate data dictionary
            foreach (var act in save)
            {
                BinBuffer bb_ = new BinBuffer();

                act(bb_);

                if (bb_.Position > 0) // i.e. has written anything
                {
                    bb_.Position = 0; // reset for writing
                    data[act.Method.DeclaringType.FullName] = bb_;
                }
                else
                    bb_.Dispose();
            }

            // write the dictionary to the buffer
            bb.Write(data.Count);

            foreach (var kvp in data)
            {
                bb.Write(kvp.Key       );
                bb.Write(kvp.Value.Size);
                bb.Write(kvp.Value     );
            }
        }
        public void Load(BinBuffer bb)
        {
            int count = bb.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var k = bb.ReadString();
                var l = bb.ReadInt32();
                var v = new BinBuffer(bb.ReadBytes(l), false);

                // keeping the data in the dictionary will make sure data from IOBehaviours will not get lost when the mod is unloaded
                data[k] = v;

                var act = load.FirstOrDefault(a => a.Method.DeclaringType.FullName == k);

                if (act != null)
                    act(v);
            }
        }
    }
}
