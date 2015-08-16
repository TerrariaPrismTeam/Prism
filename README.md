# Prism
Modding API for Terraria 1.3.0.* (Yet Another Mod To Make Mods!)

Prism files are placed in the ```My Games\Terraria\Prism\``` folder (Located in `Documents\` on windows, `~/.local/share/Terraria/` on Linux and `~/Library/Application Support/Terraria/` on OS X,
but the Windows notation will be used here), so it doesn't mess with vanilla files.

Prism is licensed under the Artistic License.

## Building
Copy your `Terraria.exe` file into `.\References` before attempting to build.
* _Windows_:
 * Install Visual Studio 2015 if you don't have it (You can get the Community version for free).
 * Open the solution in Visual Studio.
 * Click "Build Solution" (you may have to additionally click "Rebuild Solution" in order for it to build properly).
 * Prism can also be built using MSBuild from the command-line: ```msbuild /m Prism.sln /p:Configuration=DevBuild```. (not including the /p switch will default to `PreRelease` instead of `DevBuild`).
* _OS X & Linux_:
 * Install the Mono Platform if you don't have it.
 * Execute ```xbuild Prism.sln /p:Configuration=DevBuild``` to build solution with XBuild. (not including the /p switch will default to `PreRelease` instead of `DevBuild`).
* This should automatically load Terraria.exe, patch it, then create a `Prism.Terraria.dll` file for `Prism.csproj` to reference.
* The Prism binaries as well as all other required files will be located in `.\Bin\Debug`:
 * **`Prism.exe / Prism.pdb`** - Prism
 * **`Prism.Injector.dll / Prism.Injector.pdb`** - Core injection lib used by `Prism.TerrariaPatcher.csproj` (and later by the installer).
 * **`Prism.Terraria.dll`** - The patched version of the Terraria.exe that was provided.
 * **`Prism.TerrariaPatcher.exe / Prism.TerrariaPatcher.pdb`** - The patcher (run when you build `Prism.csproj`)
 * **`Steamworks.NET.dll`** - Steamworks lib required by vanilla Terraria (and therefore by Prism as well) [May be removed in the future as vanilla has it embedded into the assembly]
 * **`Ionic.Zip.CF`** - Lib used by vanilla Terraria for compressing saves.
 * **`Newtonsoft.Json.dll`** - Lib used by vanilla Terraria for Json support (not to be confused with LitJson, which Prism uses)
 * **`Mono.Cecil.dll`** - Powerful IL manipulation lib used by `Prism.Injector.csproj`
 * **[_Windows Only_]: `Microsoft.Xna.Framework.*`** The entire Xna Framework. For some reason you have to include the whole thing like this if you load an Xna assembly indirectly.
 * **[_OS X & Linux Only_]: `FNA.dll`** - This handy open platform version of Xna: https://github.com/flibitijibibo/FNA

## Launching Prism
* On _Windows_, you have 4 different options:
 * Run Prism.exe directly from the build folder. You must copy Terraria's `.\Content` folder in order for the game to have access to the content (and therefore not crash immediately upon opening).
 * Copy **all** of the files from the Prism build folder into your Terraria installation's folder and: 
   * Run `Prism.exe` [Recommended, although there is a _very_ small chance of Steam refusing to let you launch the game like this because of the DRM]
    * [_Windows Only_]: Rename your original `Terraria.exe` to something else (e.g. `Terraria_Backup.exe`), rename `Prism.exe` to `Terraria.exe`, then launch Terraria from your Steam game library [not reccommended, as it's not as easy to go back to original Terraria if you wish]
    * [_Windows Only_]: Add `Prism.exe` to GameLauncher's App list and run it from there.

* On _OSX & Linux_ you have the same options except:
 * Run the game with `Prism.sh` (sets the lib path and runs `mono Prism.exe`)
 * If you rename `Prism.exe`, open `Prism.sh` in a text editor and edit the `mono Prism.exe` line to reflect the change.

## Mods
Mods are loaded in the ```My Games\Terraria\Prism\Mods\``` folder. Each individual mod is placed in its own folder.
Each mod folder must contain:
* A ```manifest.json``` file, which contains these information fields: 
 * **"internalName"** - The mod's internal name.
 * **"displayName"** - The mod's display name.
 * **"author"** - The mod's author (optional).
 * **"version"** - The mod's version (optional, as #.#.#.#).
 * **"description"** - The mod's description (optional).
 * **"modDefTypeName"** - Name of the class which extends ```ModDef``` in the mod's assembly.
 * **"asmFileName"** - The name of the mod's code assembly. (See ```ModDef```, ```ModData```, ```ModInfo``` and ```ModLoader``` for implementation details.)

Then, all resource files (any file that is not the manifest file, assembly file or any other file with an open file handle) are loaded into Prism (though it's recommended to make a specific folder for your resources, e.g. `.\Resources`). 
These can be accessed and deserialized using a predefined or custom deserializer. 
See the ```Prism.Mods.Resources``` namespace for more info.

When the ```ModDef``` implementation is instantiated, control is handed to it (as far as the Prism engine wants to). This includes loading item definitions.

See the example mod for... an example. The contents of its output folder should be put in its own folder with in the `Mods` folder, for example ```Mods\Prism.ExampleMod\```. 

Note that on Windows, building `Prism.ExampleMod.csproj` will automatically copy the build output of the example mod into its own folder within the mod folder: ```Documents\My Games\Terraria\Prism\Mods\Prism.ExampleMod\```.
