using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.Util;

using ModName = System.String;
using ModID = System.Byte;

using ObjName = System.String;
using ObjID = System.Int16;

using PackedValue = System.UInt32;
using Prism.API.Defs;

namespace Prism.IO
{
    // let's hope nobody will ever run >255 mods at once, or with >32 767 objects in total
    public class ModIdMap
    {
        const uint IsModFlag = 0x01000000;

        // using typedef'ed names for clarity
        Func<ObjectRef, ObjID> GetVanillaID;
        Func<ObjID, ObjectRef> GetVanillaRef;
        ObjID maxVanillaId;

        BiDictionary<ModName, ModID> Mods = new BiDictionary<ModName, ModID>();
        Dictionary<ModID, BiDictionary<ObjName, ObjID>> ModObjects = new Dictionary<ModID, BiDictionary<ModName, ObjID>>();

        public ModIdMap(int maxVanilla, Func<ObjectRef, int> vanillaId, Func<int, ObjectRef> vanillaRef)
        {
            if (vanillaId  == null)
                throw new ArgumentNullException("vanillaId" );
            if (vanillaRef == null)
                throw new ArgumentNullException("vanillaRef");

            maxVanillaId = unchecked((ObjID)maxVanilla);

            GetVanillaID  = r  => unchecked((ObjID)vanillaId (r ));
            GetVanillaRef = id =>                  vanillaRef(id) ;
        }

        public PackedValue Register(ObjectRef obj)
        {
            if (obj.IsNull)
                return 0;
            if (String.IsNullOrEmpty(obj.ModName) || obj.Mod == PrismApi.VanillaInfo)
                return unchecked((PackedValue)GetVanillaID(obj)); // vanilla - no need to register

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

            if (mod.ContainsKey(on))
                oid = mod[on];
            else
            {
                oid = (ObjID)mod.Count;
                mod.Add(on, oid);
            }

#pragma warning disable 675
            return unchecked((PackedValue)oid | ((PackedValue)mid << 8 * sizeof(ObjID)) | IsModFlag);
#pragma warning restore 675
        }
        public ObjectRef GetRef(PackedValue id)
        {
            if (id == 0)
                return new ObjectRef();
            if ((id & IsModFlag) == 0)
                return GetVanillaRef(unchecked((ObjID)id));

            ObjID oid;
            ModID mid;

            unchecked
            {
                oid = (ObjID)(id & UInt16.MaxValue);
                mid = (ModID)(id & (Byte.MaxValue << 8 * sizeof(ObjID)) >> 8 * sizeof(ObjID));
            }

            var mn = Mods.Reverse[mid];
            var on = ModObjects[mid].Reverse[oid];

            return new ObjectRef(on, mn);
        }

        public void WriteDictionary(BinBuffer bb)
        {
            unchecked
            {
                bb.WriteByte((ModID)Mods.Count);
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
            int modAmt = bb.ReadByte();

            for (int i = 0; i < modAmt; i++)
                Mods.Add(bb.ReadString(), bb.ReadByte());

            for (int i = 0; i < modAmt; i++)
            {
                ModID mid = bb.ReadByte();

                var modObjs = new BiDictionary<ObjName, ObjID>();

                short objAmt = bb.ReadInt16();

                for (int j = 0; j < objAmt; j++)
                    modObjs.Add(bb.ReadString(), bb.ReadInt16());

                ModObjects.Add(mid, modObjs);
            }
        }
    }
}
