using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Prism.Mods
{
    public class LoaderError
    {
        public string ModPath
        {
            get;
            private set;
        }
        public string Message
        {
            get;
            private set;
        }

        public ModInfo? Mod
        {
            get;
            private set;
        }
        public object Data
        {
            get;
            private set;
        }

        public LoaderError(string modPath, string message = null, object data = null, ModInfo? info = null)
        {
            if (String.IsNullOrEmpty(modPath))
                throw new ArgumentNullException("modPath");

            ModPath = modPath;
            Message = message;

            Mod = info;
            Data = data;
        }
        public LoaderError(ModInfo info, string message = null, object data = null)
            : this(info.ModPath, message, data, info)
        {

        }
        public LoaderError(string modPath, Exception e, ModInfo? info = null)
            : this(modPath, e == null ? null : e.Message, e, info)
        {

        }
        public LoaderError(ModInfo info, Exception e)
            : this(info.ModPath, e, info)
        {

        }

        public override string ToString()
        {
            return "An error occured when trying to load mod "
                + (Mod.HasValue ? Mod.Value.InternalName : Path.GetFileName(ModPath))
                + (Message == null ? String.Empty : ": " + Message)
                + (Data == null ? String.Empty : ": {" + Data + "}") + ".";
        }
    }
}
