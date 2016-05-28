# How to install:

1. Unzip the archive
2. Drop your `Terraria.exe` file on the `patcher.exe` file.
  * Your `Terraria.exe` file can be found in the steam directory (if you purchased it from Steam):
    * `C:\Program Files (x86)\Steam\SteamApps\common\Terraria\` (by default) on Windows
    * `~/.local/share/Steam/steamapps/common/Terraria/` on Linux
    * `~/Library/Application Support/Steam/steamapps/common/Terraria/` on OS X
  * On Linux/OS X: run `mono` with the `patcher.exe` and your `Terraria.exe` file as the arguments.
  * You can also just launch `patcher.exe`, it will then look for or ask for a `Terraria.exe` file.
3. Copy the `Prism.exe` file to your Terraria binary directory (i.e. where your `Terraria.exe` is). It can now be launched. *Make sure the* **`Prism.Terraria.dll`** *exists in your Terraria binary directory!*
  * The `.pdb` file may be copied, too, it will add extra diagnostic info when an error occurs. **This will be useful for PreRelease builds, as this makes fixing bugs easier**
  * On Linux/OS X, the `Prism.sh` file must be copied, too, and this should be used instead of the `.exe` file to launch Prism.
  * A `.gli3` file is included for Game Launcher users. It currently uses the default directory ```C:\Program Files (x86)\```, thus some may have to edit it.
