# Prism
Modding API for Terraria 1.3.0.* (Yet Another Mod To Make Mods!)

Prism files are placed in the ```Documents\My Games\Terraria\Prism\``` folder, so it doesn't mess with vanilla files.

On Unix platforms, this is the ```~/My Games/Terraria/Prism/``` folder, but the Windows notation will be used in this file.

Prism is licensed under the Artistic License.

## Building
In order to build Prism, one must provide their own `Terraria.exe` file and copy it to the `References` subdirectory.
The build process will patch it automatically and a `Prism.Terraria.dll` file will be created.
This file is required to successfully build `Prism.csproj`.

If that is done, just build the solution in Visual Studio.

Building on the Mono platform can be done by using XBuild (simply execute ```xbuild Prism.sln```).

## Launching Prism
Prism can be launched by simply running the .exe file, using Steam (by replacing `Terraria.exe`), or by adding it to the App list in the Game Launcher.
All the .dll files in the output folder are required for Prism to run.

On Linux and OS X, `Prism.sh` must be ran instead of directly executing the .exe file using mono. `Prism.exe` (and its dependencies) must be in the same directory as the shell script.

The 'Content' directory of vanilla terraria must be in the same directory as `Prism.exe`, too. Otherwise, it will not be able to load all content files (sprites, sound effects, shaders, fonts, background music, etc).

## Mods
Mods are loaded in the ```Documents\My Games\Terraria\Prism\Mods\``` folder. Every mod is placed in its own folder, and every mod folder must contain a ```manifest.json``` file, that contains information about the mod: its internal name, display name, author (optional), version (optional), description (optional), assembly file name and ```ModDef``` full type name.

The assembly file (```asmFileName``` property) must point to the .NET assembly that contains a type that inherits from ```ModDef```, which is specified with the ```modDefTypeName``` property. (See ```ModDef```, ```ModData```, ```ModInfo``` and ```ModLoader``` for implementation details.)

Then, all resource files (any file that is not the manifest file, assembly file or any other file with an open file handle) are loaded into Prism. These can be accessed and deserialized using a predefined or custom deserializer. See the ```Prism.Mods.Resources``` namespace for more info.

When the ```ModDef``` implementation is instantiated, control is handed to it (as far as the Prism engine wants to). This includes loading item definitions.

See the example mod for... an example. (The contents of its output folder should be put in a folder in ```Documents\My Games\Terraria\Prism\Mods\```, for example ```Documents\My Games\Terraria\Prism\Mods\Prism.ExampleMod\```.)
