using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Prism.API.Defs;
using Prism.Debugging;
using Prism.Mods;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;
using Terraria;
using Terraria.ID;
using Terraria.IO;

namespace Prism.IO
{
    /// <summary>
    /// Class containing the hooks used by Prism when saving and/or loading Players and Worlds.
    /// </summary>
    static class SaveDataHandler
    {
        /// <summary>
        /// Save file version for .plr.prism files. Change whenever the format changes, and make checks in the loading code for backwards compatibility.
        /// </summary>
        const byte PLAYER_VERSION = 1;
        /// <summary>
        /// Save file version for .wld.prism files. Change whenever the format changes, and make checks in the loading code for backwards compatibility.
        /// </summary>
        const byte WORLD_VERSION = 0;

        const byte
            MIN_PLAYER_SUPPORT_VER = 1,
            MIN_WORLD_SUPPORT_VER  = 0;

        /// <summary>
        /// Base key used for file saving/loading.
        /// </summary>
        static byte[] ENCRYPTION_KEY = Encoding.Unicode.GetBytes("wH4t5_uP"); // Not inspired by vanilla at all ;D

        /// <summary>
        /// Generate encryption key using a string <paramref name="s" />.
        /// </summary>
        /// <param name="s">String to be used for generating the key</param>
        /// <returns></returns>
        static byte[] GenerateKey(string s)
        {
            if (s.Length > 8)
                s = s.Substring(0, 8);
            else //if (s.Length < 8)
                s = s.PadLeft(8);

            byte[] key = Encoding.Unicode.GetBytes(s);

            // dat encryption
            for (int i = 0; i < key.Length; i++)
                key[i] += ENCRYPTION_KEY[i];

            return key;
        }

        /// <summary>
        /// Save <paramref name="slots" /> items from <paramref name="inventory" /> to the <paramref name="bb" />.
        /// </summary>
        /// <param name="bb">The writer for storing data</param>
        /// <param name="inventory">The array of items</param>
        /// <param name="slots">The amount of items in the inventory to save</param>
        /// <param name="stack">Whether or not the stack size should be saved</param>
        /// <param name="favourited">Whether or not the favourited state should be saved</param>
        static void SaveItemSlots(BinBuffer bb, Item[] inventory, int slots, bool stack, bool favourited)
        {
            for (int i = 0; i < slots; i++)
            {
                if (inventory[i].type < ItemID.Count)
                    bb.Write(String.Empty); // write an empty string instead of 'Vanilla'
                else
                {
                    // Save basic item data
                    ItemDef item = Handler.ItemDef.DefsByType[inventory[i].type];

                    bb.Write(item.Mod.InternalName);
                    bb.Write(item.InternalName);

                    // why, vanilla writes these already?
                    // only type + mod data is needed imo (and prefix type (+ data?) later on)
                    if (stack)
                        bb.Write(inventory[i].stack);
                    bb.WriteByte(inventory[i].prefix);
                    if (favourited)
                        bb.Write(inventory[i].favorited);
                }

                // Save Mod Data
                if (inventory[i].P_BHandler != null)
                {
                    ItemBHandler handler = (ItemBHandler)inventory[i].P_BHandler;

                    handler.Save(bb);
                }
                else
                    bb.Write(0);
            }
        }
        /// <summary>
        /// Load <paramref name="slots" /> items to <paramref name="inventory" /> from the <paramref name="bb" />.
        /// </summary>
        /// <param name="bb">The reader for loading data</param>
        /// <param name="inventory">The array of items</param>
        /// <param name="slots">The amount of items in the inventory to load</param>
        /// <param name="stack">Whether or not the stack size should be loaded</param>
        /// <param name="favourited">Whether or not the favourited state should be loaded</param>
        static void LoadItemSlots(BinBuffer bb, Item[] inventory, int slots, bool stack, bool favourited)
        {
            for (int i = 0; i < slots; i++)
            {
                // Load basic item data
                string mod = bb.ReadString();
                if (!String.IsNullOrEmpty(mod) && mod != PrismApi.VanillaString)
                {
                    string item = bb.ReadString();

                    if (!ModData.modsFromInternalName.ContainsKey(mod) || !ModData.modsFromInternalName[mod].ItemDefs.ContainsKey(item))
                    {
                        inventory[i].SetDefaults(ItemDefHandler.UnknownItemID);

                        inventory[i].toolTip = mod;
                        inventory[i].toolTip2 = item;
                    }
                    else
                        inventory[i].SetDefaults(ModData.modsFromInternalName[mod].ItemDefs[item].Type);

                    if (stack)
                        inventory[i].stack = bb.ReadInt32();
                    inventory[i].prefix = bb.ReadByte();
                    if (favourited)
                        inventory[i].favorited = bb.ReadBoolean();
                }

                // Load Mod Data
                if (inventory[i].P_BHandler == null)
                {
                    inventory[i].P_BHandler = new ItemBHandler();

                    ((ItemBHandler)inventory[i].P_BHandler).Create();
                }

                ((ItemBHandler)inventory[i].P_BHandler).Load(bb);
            }
        }

        /// <summary>
        /// Save mod data to a .plr.prism file
        /// </summary>
        /// <param name="playerFile">The player being saved</param>
        internal static void SavePlayer(PlayerFileData playerFile)
        {
            string path = playerFile.Path;
            Player player = playerFile.Player;

            if (Main.ServerSideCharacter || String.IsNullOrEmpty(path))
                return;

            path += ".prism";

            if (File.Exists(path))
                File.Copy(path, playerFile.Path + ".bak.prism", true);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                fileStream.WriteByte(PLAYER_VERSION); // write this before doing the crypto stuff, so we can change it between versions

                // can't we just get rid of this?
                using (CryptoStream cryptoStream = new CryptoStream(fileStream, new RijndaelManaged() /*{ Padding = PaddingMode.None }*/.CreateEncryptor(GenerateKey(player.name), ENCRYPTION_KEY), CryptoStreamMode.Write))
                using (BinBuffer bb = new BinBuffer(cryptoStream))
                {
                    #region Player Data
                    if (player.P_BHandler != null)
                    {
                        var bh = (PlayerBHandler)player.P_BHandler;

                        bh.Save(bb);
                    }
                    else
                        bb.Write(0);
                    #endregion Player Data

                    #region Item Data
                    SaveItemSlots(bb, player.armor, player.armor.Length, false, false);
                    SaveItemSlots(bb, player.dye, player.dye.Length, false, false);
                    SaveItemSlots(bb, player.inventory, Main.maxInventory, true, true);
                    SaveItemSlots(bb, player.miscEquips, player.miscEquips.Length, false, false);
                    SaveItemSlots(bb, player.bank.item, Chest.maxItems, true, false);
                    SaveItemSlots(bb, player.bank2.item, Chest.maxItems, true, false);
                    #endregion Item Data

                    #region Buff Data
                    for (int i = 0; i < Player.maxBuffs; i++)
                    {
                        if (Main.buffNoSave[player.buffType[i]] || player.buffType[i] < BuffID.Count || player.buffTime[i] <= 0)
                            bb.Write(String.Empty);
                        else
                        {
                            var buff = Handler.BuffDef.DefsByType[player.buffType[i]];

                            bb.Write(buff.Mod.InternalName);
                            bb.Write(buff.InternalName);
                            bb.Write(player.buffTime[i]);
                        }

                        if (player.P_BuffBHandler[i] != null)
                        {
                            var bh = (BuffBHandler)player.P_BuffBHandler[i];

                            bh.Save(bb);
                        }
                        else
                            bb.Write(0);
                    }
                    #endregion Buff Data
                }
            }
        }
        /// <summary>
        /// Load player data from a .plr.prism file
        /// </summary>
        /// <param name="playerPath">The path to the vanilla .plr file</param>
        internal static void LoadPlayer(Player player, string playerPath)
        {
            playerPath += ".prism";

            // If mod data doesn't exist, don't try to load it
            if (!File.Exists(playerPath))
                return;

            try
            {
                using (FileStream fileStream = File.OpenRead(playerPath))
                {
                    byte version = (byte)fileStream.ReadByte(); // should be safe to cast here, very unlikely that the file is empty
                    if (version > PLAYER_VERSION)
                        throw new FileFormatException("Tried to load a player file from a future version of Prism.");
                    if (version < MIN_PLAYER_SUPPORT_VER)
                        throw new FileFormatException("This player is saved in a format that is too old and unsupported.");

                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, new RijndaelManaged() { Padding = PaddingMode.None }.CreateDecryptor(GenerateKey(player.name), ENCRYPTION_KEY), CryptoStreamMode.Read))
                    using (BinBuffer bb = new BinBuffer(cryptoStream))
                    {
                        #region Player Data
                        if (player.P_BHandler == null)
                        {
                            player.P_BHandler = new PlayerBHandler();

                            ((PlayerBHandler)player.P_BHandler).Create();
                        }

                        ((PlayerBHandler)player.P_BHandler).Load(bb);
                        #endregion Player Data

                        #region Item Data
                        LoadItemSlots(bb, player.armor, player.armor.Length, false, false);
                        LoadItemSlots(bb, player.dye, player.dye.Length, false, false);
                        LoadItemSlots(bb, player.inventory, Main.maxInventory, true, true);
                        LoadItemSlots(bb, player.miscEquips, player.miscEquips.Length, false, false);
                        LoadItemSlots(bb, player.bank.item, Chest.maxItems, true, false);
                        LoadItemSlots(bb, player.bank2.item, Chest.maxItems, true, false);
                        #endregion Item Data

                        #region Buff Data
                        for (int i = 0; i < Player.maxBuffs; i++)
                        {
                            var mod = bb.ReadString();

                            if (String.IsNullOrEmpty(mod) || !ModData.modsFromInternalName.ContainsKey(mod))
                                continue;

                            var md = ModData.modsFromInternalName[mod];

                            var buff = bb.ReadString();
                            var t = bb.ReadInt32();

                            if (!md.BuffDefs.ContainsKey(buff))
                                continue;

                            player.AddBuff(md.BuffDefs[buff].Type, t);

                            if (player.P_BuffBHandler[i] == null)
                            {
                                player.P_BuffBHandler[i] = new BuffBHandler();

                                ((BuffBHandler)player.P_BuffBHandler[i]).Create();
                            }

                            ((BuffBHandler)player.P_BuffBHandler[i]).Save(bb);
                        }
                        #endregion Buff Data
                    }
                }
            }
            catch (Exception e)
            {
                // Character could not be properly loaded, report and prevent playing
                //TODO: report
                Logging.LogError("Could not load player " + player.name + ": " + e);
                Trace.WriteLine("Could not load player " + player.name + ": " + e.Message);
                player.loadStatus = 1;
            }
        }

        internal static void SaveWorld(BinaryWriter w, int[] sections)
        {

        }
        internal static void LoadWorld(BinaryReader r)
        {

        }
    }
}
