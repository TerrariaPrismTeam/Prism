# Prism
Modding API for Terraria 1.3.0.* (Yet Another Mod To Make Mods!)

Prism files are placed in the ```Documents\My Games\Terraria\Prism\``` folder, so it doesn't mess with vanilla files.

On Unix platforms, this is the ```~/My Games/Terraria/Prism/``` folder, but the Windows notation will be used in this file.

## Building
In order to build Prism, one must provide their own `Terraria.exe` file and put it in the `References` subdirectory.
The build process will patch it automatically and a `Prism.Terraria.dll` file will be created.
This file is required to successfully build `Prism.csproj`.

If that is done, just build the solution in Visual Studio.

Building on the Mono platform can be done by using XBuild (simply execute ```xbuild Prism.sln```).

## Mods
Mods are loaded in the ```Documents\My Games\Terraria\Prism\Mods\``` folder. Every mod is placed in its own folder, and every mod folder must contain a ```manifest.json``` file, that contains information about the mod: its internal name, display name, author (optional), version (optional), description (optional), assembly file name and ```ModDef``` full type name.

The assembly file (```asmFileName``` property) must point to the .NET assembly that contains a type that inherits from ```ModDef```, which is specified with the ```modDefTypeName``` property. (See ```ModDef```, ```ModData```, ```ModInfo``` and ```ModLoader``` for implementation details.)

Then, all resource files (any file that is not the manifest file, assembly file or any other file with an open file handle) are loaded into Prism. These can be accessed and deserialized using a predefined or custom deserializer. See the ```Prism.Mods.Resources``` namespace for more info.

When the ```ModDef``` implementation is instantiated, control is handed to it (as far as the Prism engine wants to). This includes loading item definitions.

See the example mod for... an example. (The contents of its output folder should be put in a folder in ```Documents\My Games\Terraria\Prism\Mods\```, for example ```Documents\My Games\Terraria\Prism\Mods\Prism.ExampleMod\```.)
