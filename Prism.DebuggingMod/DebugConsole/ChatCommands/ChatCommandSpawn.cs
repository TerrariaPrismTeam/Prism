﻿using Microsoft.Xna.Framework;
using Prism.API.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Prism.DebuggingMod.ChatConsole.ChatCommands
{
    public class ChatCommandSpawn : ChatCommand
    {
        public enum EntityType
        {
            Npc,
            Item,
            Wall,
            Tile,
            Proj,
            Projectile
        }

        public ChatCommandSpawn()
            : base(name          : "Spawn"
                  ,description   : "Force-spawns an entity where the mouse cursor is pointing."  
                  ,usageText     : "\"/Spawn {EntityType} {EntityID|{(ModInternalName)|Vanilla}.EntityInternalName} [Count]\" e.x. \"/Spawn Item Vanilla.Meowmere 77\""  
                  ,requiresArgs  : true
                  ,caseSensitive : true
                  ,minArgs       : 2   
                  ,maxArgs       : 3   
                  )
        {

        }

        private EntityType EntityTypeFromStr(string str)
        {
            return (EntityType)Enum.Parse(typeof(EntityType), str.ToUpper()[0] + str.ToLower().Substring(1));//"ITEM", "item", "itEM", "IteM", etc all change to "Item"
        }

        public override void Run(string args, List<string> splitArgs)
        {
            int id = -1;
            int amt = 1;

            if (splitArgs.Count == 0)
                return;

            EntityType entityType = EntityType.Npc;//Just so intellisense would stfu since it returns if it fails anyways

            try
            {
                entityType = EntityTypeFromStr(splitArgs[0]);
            }
            catch
            {
                ChatConsole.Error("{0} is not a valid entity type (Npc, Item, Wall, Tile, or Proj)", splitArgs[0]);
                return;
            }

            if (!int.TryParse(splitArgs[1], out id))
            {
                string[] splitID = splitArgs[1].Split('.');



                if (splitID.Length < 2)
                {
                    ChatConsole.Error("If you specify the entity by name rather than by ID you must precede it by either a mod's internal name or 'Vanilla' followed by a dot ('.'). e.x. \"Vanilla.TheDestroyer\" or \"MyTotallyTubularMod.RadicalNpcDude\"");
                    return;
                }

                string modName = splitID[0];
                string entityName = splitID[1];
                bool isVanillaEntity = modName.ToLower() == "vanilla";

                try
                {
                    switch (entityType)
                    {
                        case EntityType.Projectile:
                        case EntityType.Proj: id = ProjectileDef.Defs[splitID[1], isVanillaEntity ? null : splitID[0]].Type; break;
                        case EntityType.Item: id = ItemDef      .Defs[splitID[1], isVanillaEntity ? null : splitID[0]].Type; break;
                        case EntityType.Npc:  id = NpcDef       .Defs[splitID[1], isVanillaEntity ? null : splitID[0]].Type; break;
                        case EntityType.Tile: id = TileDef      .Defs[splitID[1], isVanillaEntity ? null : splitID[0]].Type; break;
                        case EntityType.Wall: id = WallDef      .Defs[splitID[1], isVanillaEntity ? null : splitID[0]].Type; break;
                    }
                }
                catch (InvalidOperationException)
                {
                    ChatConsole.Error(isVanillaEntity ? "Could not find vanilla {2} with internal name '{0}'." : "Could not find {2} with internal name '{0}' in the data of mod '{1}'.", splitID[1], splitID[0], entityType);
                    return;
                }
            }

            if (splitArgs.Count > 2)
            {
                if (!int.TryParse(splitArgs[2], out amt))
                {
                    ChatConsole.Error("'{0}' is not a number.", splitArgs[2]);
                    return;
                }
            }
            
            string finalEntityName = "?";

            for (int i = 0; i < amt; i++)
            {
                int x = (int)Main.MouseWorld.X;
                int y = (int)Main.MouseWorld.Y;
                Point tileCoord = Main.MouseWorld.ToTileCoordinates();

                switch (entityType)
                {
                    case EntityType.Projectile:
                    case EntityType.Proj: finalEntityName = Main.projectile[Projectile.NewProjectile(x, y, 2 * (float)Main.rand.NextDouble() - 1, 2 * (float)Main.rand.NextDouble() - 1, id, ProjectileDef.Defs[id].Damage, ProjectileDef.Defs[id].Knockback, Main.myPlayer)].name; break;
                    case EntityType.Item: finalEntityName = Main.item[Item.NewItem(x, y, 32, 32, id, amt)].name; return; //All in one stack; no need to repeat spawn command.
                    case EntityType.Npc:  finalEntityName = Main.npc[NPC.NewNPC(x, y, id)].name; break;                    
                    case EntityType.Tile: WorldGen.PlaceTile(tileCoord.X, tileCoord.Y, id, false, false, Main.myPlayer); finalEntityName = TileDef.Defs[id].DisplayName.Length > 0 ? TileDef.Defs[id].DisplayName : TileDef.Defs[id].InternalName; return; //You can only place one tile in a location.
                    case EntityType.Wall: WorldGen.PlaceWall(tileCoord.X, tileCoord.Y, id, false); finalEntityName = WallDef.Defs[id].DisplayName.Length > 0 ? WallDef.Defs[id].DisplayName : WallDef.Defs[id].InternalName; return; //You can only place one wall in a location.
                }

                
            }

            Main.NewText(string.Format("Spawned {0} {3} {1}{4} [ID: {2}].", amt, entityType, id, finalEntityName, amt > 1 ? "s" : ""), (byte)Main.DiscoR, (byte)Main.DiscoG, (byte)Main.DiscoB, true);
        }
    }
}
