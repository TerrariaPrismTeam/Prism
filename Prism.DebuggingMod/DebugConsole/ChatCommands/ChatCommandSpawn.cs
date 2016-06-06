using Microsoft.Xna.Framework;
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
                  ,usageText     : "\"/Spawn {EntityType} {EntityID|{(ModInternalName)|'Vanilla' or 'v'} EntityInternalName} [Count]\" e.x. \"/Spawn Item v Meowmere 77\""  
                  ,requiresArgs  : true
                  ,caseSensitive : true
                  ,minArgs       : 2
                  ,maxArgs       : 4
                  )
        {

        }

        private EntityType EntityTypeFromStr(string str)
        {
            return (EntityType)Enum.Parse(typeof(EntityType), str.ToUpper()[0] + str.ToLower().Substring(1));//"ITEM", "item", "itEM", "IteM", etc all change to "Item"
        }

        public bool CheckEntityID(EntityType type, int id)
        {
            if (id < 0)
                return false;

            switch (type)
            {
                default: return false;
                case EntityType.Projectile:
                case EntityType.Proj: return id < Terraria.ID.ProjectileID.Count;
                case EntityType.Item: return id < Terraria.ID.ItemID.Count;
                case EntityType.Npc:  return id < Terraria.ID.NPCID.Count;
                case EntityType.Tile: return id < Terraria.ID.TileID.Count;
                case EntityType.Wall: return id < Terraria.ID.WallID.Count;
            }
        }

        public override void Run(string args, List<string> splitArgs)
        {
            int id = -1;
            int amt = 1;

            EntityType entityType = (EntityType)0;

            try
            {
                entityType = EntityTypeFromStr(splitArgs[0]);
            }
            catch
            {
                ChatConsole.Error("{0} is not a valid entity type (Npc, Item, Wall, Tile, or Projectile ['Proj' also works])", splitArgs[0]);
                return;
            }

            if (int.TryParse(splitArgs[1], out id))
            {
                //Check ID the player entered.
                if (!CheckEntityID(entityType, id))
                {
                    ChatConsole.Error("{0} is not a valid {1} ID.", id, entityType);
                    return;
                }

                if (splitArgs.Count > 2)
                {
                    if (!int.TryParse(splitArgs[2], out amt))
                    {
                        ChatConsole.Error("'{0}' is not a number.", splitArgs[2]);
                        return;
                    }
                }
            }
            else
            {
                if (splitArgs.Count < 3)
                {
                    ChatConsole.Error("If you specify an entity by name it must be preceded by either 'Vanilla' (or 'v') or the name of the mod in which the entity is defined.");
                    return;
                }

                string modName = splitArgs[1];
                string entityName = splitArgs[2];
                bool isVanillaEntity = modName.ToLower() == "vanilla" || modName.ToLower() == "v";

                try
                {
                    switch (entityType)
                    {
                        case EntityType.Projectile:
                        case EntityType.Proj: id = ProjectileDef.Defs[splitArgs[2], isVanillaEntity ? null : splitArgs[1]].Type; break;
                        case EntityType.Item: id = ItemDef      .Defs[splitArgs[2], isVanillaEntity ? null : splitArgs[1]].Type; break;
                        case EntityType.Npc:  id = NpcDef       .Defs[splitArgs[2], isVanillaEntity ? null : splitArgs[1]].Type; break;
                        case EntityType.Tile: id = TileDef      .Defs[splitArgs[2], isVanillaEntity ? null : splitArgs[1]].Type; break;
                        case EntityType.Wall: id = WallDef      .Defs[splitArgs[2], isVanillaEntity ? null : splitArgs[1]].Type; break;
                    }
                }
                catch
                {
                    ChatConsole.Error(isVanillaEntity ? "Could not find vanilla {2} with internal name '{0}'." : "Could not find {2} with (internal name, mod name) pair ({0}, {1}).", splitArgs[2], splitArgs[1], entityType);
                    return;
                }

                if (splitArgs.Count > 3)
                {
                    if (!int.TryParse(splitArgs[3], out amt))
                    {
                        ChatConsole.Error("'{0}' is not a number.", splitArgs[3]);
                        return;
                    }
                }
            }

            //Check ID found by name
            if (!CheckEntityID(entityType, id))
            {
                ChatConsole.Error("Something went wrong when parsing entity name: {0} is not a valid {1} ID.", id, entityType);
                return;
            }
            
            string finalEntityName = "?";

            for (int i = 0; i < amt; i++)
            {
                int x = (int)Main.MouseWorld.X;
                int y = (int)Main.MouseWorld.Y;
                Point tileCoord = Main.MouseWorld.ToTileCoordinates();

                bool spawnOnlyOne = false;

                switch (entityType)
                {
                    case EntityType.Projectile:
                    case EntityType.Proj: finalEntityName = Main.projectile[Projectile.NewProjectile(x, y, 2 * (float)Main.rand.NextDouble() - 1, 2 * (float)Main.rand.NextDouble() - 1, id, ProjectileDef.Defs[id].Damage, ProjectileDef.Defs[id].Knockback, Main.myPlayer)].name; break;
                    case EntityType.Item: finalEntityName = Main.item[Item.NewItem(x, y, 32, 32, id, amt)].name; spawnOnlyOne = true; break; //All in one stack; no need to repeat spawn command.
                    case EntityType.Npc:  finalEntityName = Main.npc[NPC.NewNPC(x, y, id)].name; break;                    
                    case EntityType.Tile: WorldGen.PlaceTile(tileCoord.X, tileCoord.Y, id, false, false, Main.myPlayer); finalEntityName = TileDef.Defs[id].DisplayName.Length > 0 ? TileDef.Defs[id].DisplayName : TileDef.Defs[id].InternalName; spawnOnlyOne = true; break; //You can only place one tile in a location.
                    case EntityType.Wall: WorldGen.PlaceWall(tileCoord.X, tileCoord.Y, id, false); finalEntityName = WallDef.Defs[id].DisplayName.Length > 0 ? WallDef.Defs[id].DisplayName : WallDef.Defs[id].InternalName; spawnOnlyOne = true; break; //You can only place one wall in a location.
                }

                if (spawnOnlyOne)
                    break;
            }

            Main.NewText(string.Format("Spawned {0} {3} {1}{4} [ID: {2}].", amt, entityType, id, finalEntityName, amt > 1 ? "s" : ""), (byte)Main.DiscoR, (byte)Main.DiscoG, (byte)Main.DiscoB, true);
        }
    }
}
