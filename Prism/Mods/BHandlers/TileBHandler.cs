using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prism.API.Behaviours;
using Prism.IO;
using Prism.Mods.Hooks;
using Terraria;
using Terraria.DataStructures;

namespace Prism.Mods.BHandlers
{
    //TODO: attach BHandler when a tile is placed
    public sealed class TileBHandler : EntityBHandler<TileBehaviour, Tile>
    {
        IEnumerable<Action> 
            onUpdate, onPlaced;

        public override void Create()
        {
            base.Create();

            onUpdate = HookManager.CreateHooks<TileBehaviour, Action>(Behaviours, "OnUpdate");
            onPlaced = HookManager.CreateHooks<TileBehaviour, Action>(Behaviours, "OnPlaced");
        }
        public override void Clear ()
        {
            base.Clear ();

            onUpdate = null;
            onPlaced = null;
        }

        public void OnUpdate()
        {
            HookManager.Call(onUpdate);
        }
        public void OnPlaced()
        {
            HookManager.Call(onPlaced);
        }
    }

    sealed class TileBHandlerEntity : TileEntity
    {
        internal TileBHandler bHandler;
        //internal TileEntity inner;

        public TileBHandlerEntity(TileBHandler bh)
        {
            bHandler = bh;

            type = 0; // TETraingingDummy
        }

        internal void Save(BinBuffer bb)
        {
            bHandler.Save(bb);
        }
        internal void Load(BinBuffer bb)
        {
            bHandler.Load(bb);
        }

        public override void ReadExtraData (BinaryReader r)
        {
            //if (inner != null) inner.ReadExtraData(r);

            base.ReadExtraData(r);
        }
        public override void WriteExtraData(BinaryWriter w)
        {
            //if (inner != null) inner.WriteExtraData(w);

            base.WriteExtraData(w);

            w.Write((short)(Main.maxNPCs - 1)); // fake NPC ID
        }

        public override void Update()
        {
            //if (inner != null)
            //    inner.Update();

            bHandler.OnUpdate();
        }
    }
}
