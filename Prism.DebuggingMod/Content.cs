using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API;
using Prism.API.Behaviours;

namespace Prism.DebuggingMod
{
    sealed class Content : ContentHandler
    {
        protected override GameBehaviour CreateGameBehaviour()
        {
            return new Game();
        }
    }
}
