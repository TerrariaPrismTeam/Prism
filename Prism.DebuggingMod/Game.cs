using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Prism.API.Behaviours;
using Terraria;

namespace Prism.DebuggingMod
{
    sealed class Game : GameBehaviour
    {
        public override bool IsChatAllowed()
        {
            return true;
        }

        public override bool OnLocalChat()
        {
            ChatConsole.ChatConsole.RunCmd(Main.chatText);

            return true;
        }
    }
}
