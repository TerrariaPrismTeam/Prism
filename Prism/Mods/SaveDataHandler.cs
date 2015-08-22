using Prism.API.Behaviours;
using Prism.API.Defs;
using Prism.Mods.BHandlers;
using Prism.Mods.DefHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.IO;

namespace Prism.Mods
{
    /// <summary>
    /// Class containing the hooks used by Prism when saving and/or loading Players and Worlds.
    /// </summary>
    class SaveDataHandler
    {
        /// <summary>
        /// Save file version for .plr.prism files. Change whenever the format changes, and make checks in the loading code for backwards compatibility.
        /// </summary>
        const byte PLAYER_VERSION = 0;
        /// <summary>
        /// Save file version for .wld.prism files. Change whenever the format changes, and make checks in the loading code for backwards compatibility.
        /// </summary>
        const byte WORLD_VERSION = 0;

        /// <summary>
        /// Base key used for file saving/loading.
        /// </summary>
        static byte[] ENCRYPTION_KEY = Encoding.Unicode.GetBytes("wH4t5_uP"); // Not inspired by vanilla at all ;D

        /// <summary>
        /// Save mod data to a .plr.prism file
        /// </summary>
        /// <param name="playerFile">The player being saved</param>
        public static void SavePlayer(PlayerFileData playerFile)
        {
            string path = playerFile.Path;
            Player player = playerFile.Player;

            if (Main.ServerSideCharacter || path == null || path == String.Empty)
            {
                return;
            }

            path += ".prism";

            if (File.Exists(path))
            {
                File.Copy(path, playerFile.Path + ".bak.prism", true);
            }

            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                using (CryptoStream cryptoStream = new CryptoStream(fileStream, rijndaelManaged.CreateEncryptor(GenerateKey(player.name), ENCRYPTION_KEY), CryptoStreamMode.Write))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        binaryWriter.Write(PLAYER_VERSION);

                        #region Player Data
                        // TODO player.P_BHandler + PlayerBehaviour
                        #endregion Player Data

                        #region Item Data
                        SaveItemSlots(binaryWriter, player.armor, player.armor.Length, false, false);
                        SaveItemSlots(binaryWriter, player.dye, player.dye.Length, false, false);
                        SaveItemSlots(binaryWriter, player.inventory, Main.maxInventory, true, true);
                        SaveItemSlots(binaryWriter, player.miscEquips, player.miscEquips.Length, false, false);
                        SaveItemSlots(binaryWriter, player.bank.item, Chest.maxItems, true, false);
                        SaveItemSlots(binaryWriter, player.bank2.item, Chest.maxItems, true, false);
                        #endregion Item Data

                        #region Buff Data
                        // TODO BuffDefs
                        /*
                        for (int i=0; i<Player.maxBuffs; i++)
                        {
                            if (Main.buffNoSave[player.buffType[i]])
                            {
                                binaryWriter.Write(PrismApi.VanillaString);
                                continue;
                            }
                            else
                            {
                                BuffDef buff = BuffDef.ByType[player.buffType[i]];

                                binaryWriter.Write(buff.Mod.InternalName);
                                binaryWriter.Write(buff.InternalName);
                                binaryWriter.Write(player.buffTime[i]);
                            }

                            // TODO Buff Save/Load hooks
                        }
                        */
                        #endregion Buff Data

                    }
                }
            }
        }

        /// <summary>
        /// Load player data from a .plr.prism file
        /// </summary>
        /// <param name="playerPath">The path to the vanilla .plr file</param>
        public static void LoadPlayer(Player player, string playerPath)
        {
            playerPath += ".prism";

            if (!File.Exists(playerPath))
            {
                // If mod data doesn't exist, don't try to load it
                return;
            }

            try
            {
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Padding = PaddingMode.None;
                byte[] buffer = File.ReadAllBytes(playerPath);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(GenerateKey(player.name), ENCRYPTION_KEY), CryptoStreamMode.Read))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(cryptoStream))
                        {
                            byte version = binaryReader.ReadByte();
                            if (version > PLAYER_VERSION)
                            {
                                // Do not attempt to load files from future versions
                                throw new Exception();
                            }

                            #region Player Data
                            // TODO player.P_BHandler + PlayerBehaviour
                            #endregion Player Data

                            #region Item Data
                            LoadItemSlots(binaryReader, player.armor, player.armor.Length, false, false);
                            LoadItemSlots(binaryReader, player.dye, player.dye.Length, false, false);
                            LoadItemSlots(binaryReader, player.inventory, Main.maxInventory, true, true);
                            LoadItemSlots(binaryReader, player.miscEquips, player.miscEquips.Length, false, false);
                            LoadItemSlots(binaryReader, player.bank.item, Chest.maxItems, true, false);
                            LoadItemSlots(binaryReader, player.bank2.item, Chest.maxItems, true, false);
                            #endregion Item Data

                            #region Buff Data
                            // TODO BuffDefs
                            #endregion Buff Data
                        }
                    }
                }
            }
            catch
            {
                // Character could not be properly loaded, report and prevent playing
                // TODO report
                player.loadStatus = 1;
            }
        }

        /// <summary>
        /// Generate encryption key using a string 's'.
        /// </summary>
        /// <param name="s">String to be used for generating the key</param>
        /// <returns></returns>
        static byte[] GenerateKey(string s)
        {
            UnicodeEncoding unicode = new UnicodeEncoding();

            if (s.Length > 8)
            {
                s = s.Substring(0, 8);
            }
            else
            {
                s = s.PadLeft(8);
            }

            byte[] key = unicode.GetBytes(s);

            for (int i = 0; i < key.Length; i++)
            {
                key[i] += ENCRYPTION_KEY[i];
            }

            return key;
        }

        /// <summary>
        /// Save 'slots' items from 'inventory' to the 'binaryWriter'.
        /// </summary>
        /// <param name="binaryWriter">The writer for storing data</param>
        /// <param name="inventory">The array of items</param>
        /// <param name="slots">The amount of items in the inventory to save</param>
        /// <param name="stack">Whether or not the stack size should be saved</param>
        /// <param name="favourited">Whether or not the favourited state should be saved</param>
        static void SaveItemSlots(BinaryWriter binaryWriter, Item[] inventory, int slots, bool stack, bool favourited)
        {
            for (int i = 0; i < slots; i++)
            {
                if (inventory[i].type < ItemID.Count)
                {
                    // No need to re-save vanilla
                    binaryWriter.Write(PrismApi.VanillaString);
                }
                else
                {
                    // Save basic item data
                    ItemDef item = Handler.ItemDef.DefsByType[inventory[i].type];

                    binaryWriter.Write(item.Mod.InternalName);
                    binaryWriter.Write(item.InternalName);
                    if (stack)
                        binaryWriter.Write(inventory[i].stack);
                    binaryWriter.Write(inventory[i].prefix);
                    if (favourited)
                        binaryWriter.Write(inventory[i].favorited);
                }

                // Save Mod Data
                if (inventory[i].P_BHandler != null)
                {
                    ItemBHandler handler = (ItemBHandler)inventory[i].P_BHandler;

                    for (int j = 0; j < handler.behaviours.Count; j++)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream))
                            {
                                handler.behaviours[j].Save(writer);
                                writer.Flush();

                                if (stream.Length > 0)
                                {
                                    binaryWriter.Write(handler.behaviours[j].Mod.Info.InternalName);
                                    binaryWriter.Write(handler.behaviours[j].GetType().FullName);
                                    binaryWriter.Write((int)stream.Length);
                                    binaryWriter.Write(stream.ToArray());
                                }
                            }
                        }
                    }
                }

                // If loaded when checking for mod name, will complete loading this item.
                binaryWriter.Write("__END__");
            }
        }

        /// <summary>
        /// Load 'slots' items to 'inventory' from the 'binaryWriter'.
        /// </summary>
        /// <param name="binaryReader">The reader for loading data</param>
        /// <param name="inventory">The array of items</param>
        /// <param name="slots">The amount of items in the inventory to load</param>
        /// <param name="stack">Whether or not the stack size should be loaded</param>
        /// <param name="favourited">Whether or not the favourited state should be loaded</param>
        static void LoadItemSlots(BinaryReader binaryReader, Item[] inventory, int slots, bool stack, bool favourited)
        {
            for (int i = 0; i < slots; i++)
            {
                // Load basic item data
                string mod = binaryReader.ReadString();
                if (!mod.Equals(PrismApi.VanillaString))
                {
                    string item = binaryReader.ReadString();
                    inventory[i].SetDefaults(ModData.modsFromInternalName[mod].ItemDefs[item].Type);

                    if (stack)
                        inventory[i].stack = binaryReader.ReadInt32();
                    inventory[i].prefix = binaryReader.ReadByte();
                    if (favourited)
                        inventory[i].favorited = binaryReader.ReadBoolean();
                }

                // Load Mod Data
                if (inventory[i].P_BHandler != null)
                {
                    ItemBHandler handler = (ItemBHandler)inventory[i].P_BHandler;
                    while (!(mod = binaryReader.ReadString()).Equals("__END__"))
                    {
                        string type = binaryReader.ReadString();

                        int j;
                        for (j = 0; j < handler.behaviours.Count; j++)
                        {
                            ItemBehaviour behaviour = handler.behaviours[j];
                            if (type.Equals(behaviour.GetType().FullName))
                            {
                                using (MemoryStream stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32())))
                                {
                                    using (BinaryReader reader = new BinaryReader(stream))
                                    {
                                        behaviour.Load(reader);
                                    }
                                }
                                break;
                            }
                        }

                        // Could not find matching behaviour, skip to next
                        if (j == handler.behaviours.Count)
                        {
                            binaryReader.ReadBytes(binaryReader.ReadInt32());
                        }
                    }
                }
                else
                {
                    // No behaviours available, skip mod data
                    while (!(mod = binaryReader.ReadString()).Equals("__END__"))
                    {
                        binaryReader.ReadString();
                        binaryReader.ReadBytes(binaryReader.ReadInt32());
                    }
                }
            }
        }
    }
}
