using Prism.API.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Prism.DebuggingMod.ChatConsole.ChatCommands
{
    public class ChatCommandNpc : ChatCommand
    {
        public ChatCommandNpc()
            : base(name          : "NPC"
                  ,description   : "Force-spawns an NPC where the mouse cursor is pointing."  
                  ,usageText     : "\"/NPC {ID|{(ModInternalName)|Vanilla}.InternalName} [Count]\" e.x. \"/NPC Vanilla.TheDestroyer 5\" (not recommended for new players)"  
                  ,requiresArgs  : true
                  ,caseSensitive : true
                  ,minArgs       : 1   
                  ,maxArgs       : 3   
                  )
        {

        }

        public override void Run(string args, List<string> splitArgs)
        {
            int entityID = -1;
            int entityCount = 1;

            if (splitArgs.Count == 0)
                return;

            if (!int.TryParse(splitArgs[0], out entityID))
            {
                string[] splitID = splitArgs[0].Split('.');

                if (splitID.Length < 2)
                {
                    ChatConsole.Error("If you specify the NPC by name rather than by ID you must precede it by either a mod's internal name or 'Vanilla' followed by a dot ('.'). e.x. \"Vanilla.TheDestroyer\"");
                    return;
                }

                string modName = splitID[0];
                string entityName = splitID[1];
                bool isVanillaNpc = modName.ToLower() == "vanilla";

                try
                {
                    entityID = NpcDef.Defs[splitID[1], isVanillaNpc ? null : splitID[0]].Type;
                }
                catch (InvalidOperationException)
                {
                    ChatConsole.Error(isVanillaNpc ? "Could not find a vanila NPC with internal name '{0}'." : "Could not find an NPC with internal name '{0}' in the data of mod '{1}'.", splitID[1], splitID[0]);
                    return;
                }
            }

            if (splitArgs.Count > 1)
            {
                if (!int.TryParse(splitArgs[1], out entityCount))
                {
                    ChatConsole.Error("'{0}' is not a number.", splitArgs[1]);
                    return;
                }
            }

            for (int i = 0; i < entityCount; i++)
            {
                NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, entityID);
            }
        }
    }
}
