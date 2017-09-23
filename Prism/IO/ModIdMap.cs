using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.Util;

using ModName = System.String;
using ModID = System.UInt16;

using ObjName = System.String;
using ObjID = System.UInt32;

using PackedValue = System.UInt32;

namespace Prism.IO
{
    // let's hope nobody will ever run >2047 mods at once, or with >1 048 575 objects in total
    public class ModIdMap
    {
        const  int ObjIdShl   =  0;
        const  int ModIdShl   = 20;

        const uint ModIdMask0 = 0x7FF;
        const uint ModIdMask  = ModIdMask0 << ModIdShl;
                             // 0x7FF00000;
        const uint IsModFlag  = 0x80000000;
        const uint ObjIdMask  = 0x000FFFFF;

        // vanilla gets the whole space
        const uint VanIdMask = ModIdMask | ObjIdMask;

        // using typedef'ed names for clarity
        Func<ObjectRef, ObjID> GetVanillaID;
        Func<ObjID, ObjectRef> GetVanillaRef;
        ObjID maxVanillaId;

        BiDictionary<ModName, ModID> Mods = new BiDictionary<ModName, ModID>();
        Dictionary<ModID, BiDictionary<ObjName, ObjID>> ModObjects = new Dictionary<ModID, BiDictionary<ModName, ObjID>>();

        ModName[] mnames = null;
        BiDictionary<ObjName, ObjID>[] mobjs = null;

        public ModIdMap(int maxVanilla, Func<ObjectRef, int> vanillaId, Func<int, ObjectRef> vanillaRef)
        {
            if (vanillaId  == null)
                throw new ArgumentNullException("vanillaId" );
            if (vanillaRef == null)
                throw new ArgumentNullException("vanillaRef");

            maxVanillaId = unchecked((ObjID)maxVanilla);

            GetVanillaID  = r  => unchecked((ObjID)vanillaId(r));
            GetVanillaRef = id => vanillaRef(unchecked((int)id));
        }

        public PackedValue Register(int       vid)
        {
            return unchecked((PackedValue)vid & VanIdMask); // should work just fine
        }
        public PackedValue Register(ObjectRef obj)
        {
            if (obj.IsNull)
                return 0;
            if (String.IsNullOrEmpty(obj.ModName) || obj.Mod == PrismApi.VanillaInfo)
                return GetVanillaID(obj) & VanIdMask;

            ModName mn = obj.ModName;
            ModID mid;

            if (Mods.ContainsKey(mn))
                mid = Mods.Forward[mn];
            else
            {
                mid = (ModID)Mods.Count;
                Mods.Add(mn, mid);

                ModObjects.Add(mid, new BiDictionary<ObjName, ObjID>());
            }

            ObjName on = obj.Name;
            ObjID oid;

            var mod = ModObjects[mid];

            if (!mod.TryGetValue(on, out oid))
            {
                oid = (ObjID)mod.Count;
                mod.Add(on, oid);
            }

#pragma warning disable 675
            return unchecked((PackedValue)( oid & ObjIdMask               ) |
                            ((PackedValue)((mid & ModIdMask0) << ModIdShl)) |
                                            IsModFlag);
#pragma warning restore 675
        }
        public void GetRef(PackedValue id, Action<int> cVanilla, Action<ObjectRef> cMod)
        {
            if (id == 0)
            {
                cVanilla(0);
                return;
            }
            if ((id & IsModFlag) == 0)
            {
                // negative!
                if ((id & ((PackedValue)1 << 30)) != 0)
                    // readd the normal sign bit
                    id |= (PackedValue)1 << 31;

                cVanilla(unchecked((int)id));
                return;
            }

            ObjID oid;
            ModID mid;

            unchecked
            {
                oid = (ObjID) (id & ObjIdMask);
                mid = (ModID)((id & ModIdMask) >> ModIdShl);
            }

            var mn = mnames[mid];
            var on = mobjs [mid].Reverse[oid];

            cMod(new ObjectRef(on, mn));
        }

        public void WriteDictionary(BinBuffer bb)
        {
            unchecked
            {
                bb.Write((ModID)Mods.Count);
            }

            foreach (var kvp in Mods)
            {
                bb.Write(kvp.Key  );
                bb.Write(kvp.Value);
            }

            foreach (var kvp in ModObjects)
            {
                bb.Write(kvp.Key);

                var mod = kvp.Value;

                bb.Write(unchecked((ObjID)mod.Count));

                foreach (var kvp_ in mod)
                {
                    bb.Write(kvp_.Key  );
                    bb.Write(kvp_.Value);
                }
            }
        }
        public void ReadDictionary (BinBuffer bb)
        {
            int modAmt = bb.ReadUInt16();

            mnames = new ModName[modAmt];
            for (int i = 0; i < modAmt; i++)
            {
                string mn  = bb.ReadString();
                ushort ind = bb.ReadUInt16();

                Mods.Add(mnames[ind] = mn, ind);
            }

            mobjs = new BiDictionary<ObjName, ObjID>[mnames.Length];
            for (int i = 0; i < modAmt; i++)
            {
                ModID mid = bb.ReadUInt16();

                var modObjs = new BiDictionary<ObjName, ObjID>();

                uint objAmt = bb.ReadUInt32();

                for (int j = 0; j < objAmt; j++)
                    modObjs.Add(bb.ReadString(), bb.ReadUInt32());

                ModObjects.Add(mid, mobjs[mid] = modObjs);
            }
        }
    }
}
