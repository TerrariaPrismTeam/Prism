using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.API;
using Prism.API.Defs;
using Prism.Util;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Prism.Mods.DefHandlers
{
    sealed class BuffDefHandler
    {
        internal int NextTypeIndex;

#pragma warning disable 414
        static bool FillingVanilla = false;
#pragma warning restore 414

        public Dictionary<int, BuffDef> DefsByType = new Dictionary<int, BuffDef>();
        public Dictionary<string, BuffDef> VanillaDefsByName = new Dictionary<string, BuffDef>();

        int? minVanillaId = null, maxVanillaId = null;
        FieldInfo[] idFields = null;
        int[] idValues = null;
        string[] idNames = null;
        Dictionary<int, string> idLut = null;

        internal Dictionary<int, string> IDLUT
        {
            get
            {
                if (idLut == null)
                    idLut = IDFields.Select(fi => new KeyValuePair<int, string>(
                        (int)Convert.ChangeType(fi.GetValue(null), typeof(int)),
                        fi.Name
                    )).ToDictionary();

                return idLut;
            }
        }
        internal FieldInfo[] IDFields
        {
            get
            {
                if (idFields == null)
                    idFields = typeof(BuffID).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType.IsPrimitive && f.FieldType != typeof(bool) && f.Name != "Count").ToArray();

                return idFields;
            }
        }
        internal int[] IDValues
        {
            get
            {
                if (idValues == null)
                    idValues = IDFields.Select(f => (int)Convert.ChangeType(f.GetValue(null), typeof(int))).ToArray();

                return idValues;
            }
        }
        internal string[] IDNames
        {
            get
            {
                if (idNames == null)
                    idNames = IDFields.Select(f => f.Name).ToArray();

                return idNames;
            }
        }

        int MinVanillaID
        {
            get
            {
                if (minVanillaId == null)
                    minVanillaId = IDValues.Min();

                return minVanillaId.Value;
            }
        }
        int MaxVanillaID
        {
            get
            {
                if (maxVanillaId == null)
                    maxVanillaId = IDValues.Max();

                return maxVanillaId.Value;
            }
        }

        internal BuffDefHandler()
        {
            Reset();
        }

        void ExtendVanillaArrays(int amt = 1)
        {
            if (amt == 0)
                return;

            int newLen = amt > 0 ? Main.debuff.Length + amt : BuffID.Count;

            if (!Main.dedServ)
                Array.Resize(ref Main.buffTexture, newLen);

            Array.Resize(ref Main.buffNoSave       , newLen);
            Array.Resize(ref Main.buffNoTimeDisplay, newLen);
            Array.Resize(ref Main.debuff           , newLen);
            Array.Resize(ref Main.lightPet         , newLen);
            Array.Resize(ref Main.vanityPet        , newLen);
            Array.Resize(ref Main.meleeBuff        , newLen);
            Array.Resize(ref Main.persistentBuff   , newLen);
            Array.Resize(ref Lang._buffNameCache   , newLen);
            Array.Resize(ref Lang._buffDescriptionCache, newLen);
            Array.Resize(ref Main.pvpBuff          , newLen);
        }

        void CopyEntityToDef(int id, BuffDef def)
        {
            def.Type = id;

            def.DoesNotSave        = Main.buffNoSave       [id];
            def.HideTimeDisplay    = Main.buffNoTimeDisplay[id];
            def.IsDebuff           = Main.debuff           [id];
            def.IsLightPet         = Main.lightPet         [id];
            def.IsVanityPet        = Main.vanityPet        [id];
            def.IsWeaponImbuement  = Main.meleeBuff        [id];
            def.PersistsAfterDeath = Main.persistentBuff   [id];
            def.WorksInPvP         = Main.pvpBuff          [id];

            // TODO: make this keep all translations
            def.Tooltip            = new ObjectName(Lang._buffDescriptionCache[id]);
            def.DisplayName        = new ObjectName(Lang._buffNameCache       [id]);
        }
        void CopySetProperties(BuffDef def)
        {
            Main.buffNoSave       [def.Type] = def.DoesNotSave       ;
            Main.buffNoTimeDisplay[def.Type] = def.HideTimeDisplay   ;
            Main.debuff           [def.Type] = def.IsDebuff          ;
            Main.lightPet         [def.Type] = def.IsLightPet        ;
            Main.vanityPet        [def.Type] = def.IsVanityPet       ;
            Main.meleeBuff        [def.Type] = def.IsWeaponImbuement ;
            Main.persistentBuff   [def.Type] = def.PersistsAfterDeath;
            Main.pvpBuff          [def.Type] = def.WorksInPvP        ;

            Lang._buffDescriptionCache[def.Type] = (LocalizedText)def.Tooltip;
            Lang._buffNameCache       [def.Type] = (LocalizedText)def.DisplayName;
        }

        internal void FillVanilla()
        {
            FillingVanilla = true;

            int id = 0;

            var def = new BuffDef(ObjectName.Empty);
            def.InternalName = String.Empty;

            DefsByType.Add(id, def);
            VanillaDefsByName.Add(String.Empty, def);

            var byDisplayName = new Dictionary<string, BuffDef>();

            for (id = MinVanillaID; id < MaxVanillaID; id++)
            {
                if (id == 0)
                    continue;

                var index = Array.IndexOf(IDValues, id);
                if (index == -1)
                    continue;

                def = new BuffDef(new ObjectName(Lang.GetBuffName(id)),
                        null, () => Main.buffTexture[id]);

                DefsByType.Add(id, def);
                VanillaDefsByName.Add(IDNames[index], def);

                var n = Lang.GetBuffName(id);
                if (!byDisplayName.ContainsKey(n) && !VanillaDefsByName.ContainsKey(n))
                    byDisplayName.Add(n, def);

                def.Mod = PrismApi.VanillaInfo;

                CopyEntityToDef(id, def); // TEntityDef is a class -> dictionary entries are updated, too

                def.InternalName = IDNames[index];
            }

            foreach (var kvp in byDisplayName)
                if (!VanillaDefsByName.ContainsKey(kvp.Key))
                    VanillaDefsByName.Add(kvp.Key, kvp.Value);

            FillingVanilla = false;
        }

        internal void Reset()
        {
            ExtendVanillaArrays(-1);

            NextTypeIndex = MaxVanillaID;

            DefsByType.Clear();
        }

        List<LoaderError> CheckTextures(BuffDef def)
        {
            var ret = new List<LoaderError>();

            if (def.GetTexture == null)
                ret.Add(new LoaderError(def.Mod, "GetTexture of BuffDef " + def + " is null."));

            return ret;
        }
        List<LoaderError> LoadTextures (BuffDef def)
        {
            var ret = new List<LoaderError>();

            var t = def.GetTexture();
            if (t == null)
            {
                ret.Add(new LoaderError(def.Mod, "GetTexture return value is null for BuffDef " + def + "."));
                return ret;
            }

            Main.buffTexture[def.Type] = def.GetTexture();

            return ret;
        }

        internal IEnumerable<LoaderError> Load(Dictionary<string, BuffDef> dict)
        {
            var err = new List<LoaderError>();

            ExtendVanillaArrays(dict.Count);

            foreach (var def in dict.Values)
            {
                if (!Main.dedServ)
                {
                    var cterrs = CheckTextures(def);

                    if (cterrs.Count > 0)
                    {
                        err.AddRange(cterrs);
                        continue;
                    }
                }

                def.Type = NextTypeIndex;

                if (!Main.dedServ)
                {
                    var lterrs = LoadTextures(def);

                    if (lterrs.Count > 0)
                    {
                        err.AddRange(lterrs);
                        continue;
                    }
                }

                CopySetProperties(def);
                DefsByType.Add(NextTypeIndex++, def);
            }

            return err;
        }
    }
}

