# Prism

Modding API for Terraria 1.3.* (Yet Another Mod To Make Mods,
*batteries not included and some assembly is required*) (not a spying
program, we promise!)

Prism files are placed in the `My Games\Terraria\Prism\ ` folder
(Located in `Documents\ ` on windows, `~/.local/share/Terraria/` on
 Linux and `~/Library/Application Support/Terraria/` on OS X,
but the Windows notation will be used here), so it doesn't mess with
vanilla files.

Prism is licensed under the ~~Artistic License~~ WTFPL, because it
~~is~~ ~~was~~ is no longer in developement. (It is definitely *very*
dead now).

For the latest release, see the `master` branch. The in-developement
version can be found on the `develop` branch. The Terraria 1.3.0 version
is on the `v1-3-0` branch, and the 1.3.1 version is on the `v1-3-1` one.

Binaries can be found [here](https://github.com/TerrariaPrismTeam/Prism/releases).
Modest documentation is available [here](https://github.com/TerrariaPrismTeam/Prism/wiki)

## Support

~~For help specifically with getting **Prism** to work as well as using
it in your own mods, come chat with us on the **`#PrismAPI`** channel
on **`irc.esper.net`** using your favorite IRC client or using
[**this quick online one**](http://chat.mibbit.com/?server=irc.esper.net&channel=%23PrismAPI).~~

To chat with a variety of mod developers and veteran mod developers
about **general Terraria coding**, come chat on the **`#tapi`** channel
on **`irc.esper.net`** using your favorite IRC client or using
[**this quick online one**](http://chat.mibbit.com/?server=irc.esper.net&channel=%23tapi).
This channel is also quite dead, as everyone moved to Discord.

## Building

***Copy your `Terraria.exe` file into `.\References` before attempting to build.***

* _Windows_:
 * Install Visual Studio 2015 if you don't have it (You can get the
   Community version for free).
 * Open the solution in Visual Studio.
 * Click "Build Solution" (you may have to additionally click "Rebuild
   Solution" in order for it to build properly).
 * Prism can also be built using MSBuild from the command-line: `msbuild /m Prism.sln /p:Configuration=DevBuild`. (not including the /p switch will default to `PreRelease` instead of `DevBuild`).
* _OS X & Linux_:
 * Install the Mono Platform if you don't have it (`mono` or
   `mono-complete` on most distros).
 * Execute `xbuild Prism.sln /p:Configuration=DevBuild` to build solution
   with XBuild. (not including the /p switch will default to `PreRelease` instead of `DevBuild`).
* This should automatically load Terraria.exe, patch it, then create a
  `Prism.Terraria.dll` file for `Prism.csproj` to reference.
* The Prism binaries as well as all other required files will be located
  in `.\Bin\Debug`:
 * **`Prism.exe / Prism.pdb`** - Prism
 * **`Prism.Injector.dll / Prism.Injector.pdb`** - Core injection lib
   used by `Prism.TerrariaPatcher.csproj` (and later by the installer).
 * **`Prism.Terraria.dll`** - The patched version of the Terraria.exe
   that was provided.
 * **`Prism.TerrariaPatcher.exe / Prism.TerrariaPatcher.pdb`** - The
   patcher (run when you build `Prism.csproj`)
 * **`Steamworks.NET.dll`** - Steamworks lib required by vanilla
   Terraria (and therefore by Prism as well).
 * **`Ionic.Zip.CF`** - Lib used by vanilla Terraria for compressing saves.
 * **`Newtonsoft.Json.dll`** - Lib used by vanilla Terraria for Json
   support (not to be confused with LitJson, which Prism uses).
 * **`dnlib.dll`** - Very powerful IL manipulation lib used by
   `Prism.Injector.csproj`
 * **[_Windows Only_]: `Microsoft.Xna.Framework.*`** The entire XNA
   Framework. For some reason you have to include the whole thing like
   this if you load an XNA assembly indirectly.
 * **[_OS X & Linux Only_]: `FNA.dll`** - This handy open platform version
   of XNA: https://github.com/flibitijibibo/FNA. It contains some custom
   patches, see `References\FNA.patch` and `References\FNA.patch.readme.txt`.

## Launching Prism

* On _Windows_, choose one of the following options:
 * Run Prism.exe directly from the build folder. You must copy Terraria's
   `.\Content` folder in order for the game to have access to the
   content (and therefore not crash immediately upon opening). Making a
   symlink works as well.
 * Copy **all** of the files from the Prism build folder into your
   Terraria installation's folder and do *one of the following*:
     * Run `Prism.exe` [Recommended, although there is a _very_ small
       chance of Steam refusing to let you launch the game like this
       because of the DRM]
     * Rename your original `Terraria.exe` to something else (e.g.
       `Terraria_Backup.exe`), then rename `Prism.exe` to `Terraria.exe`,
       then launch Terraria from your Steam game library [not
       reccommended, as it's not as easy to go back to original
       Terraria if you wish]
     * Add `Prism.exe` to GameLauncher's App list and run it from there.
* On _OSX & Linux_ you have the same options except:
 * Run the game with `Prism.sh` (sets `$LD_LIBRARY_PATH` path and runs
   `mono Prism.exe`)
 * If you rename `Prism.exe`, open `Prism.sh` in a text editor and edit
   the `mono Prism.exe` line to reflect the change.
 * GameLauncher is Windows-only as far as we know.

## Mods

Mods are loaded in the `My Games\Terraria\Prism\Mods\ ` folder. Each
individual mod is placed in its own folder.

Each mod folder must contain:

* A `manifest.json` file, which contains these information fields: 
 * **"internalName"** - The mod's internal name.
 * **"displayName"** - The mod's display name.
 * **"author"** - The mod's author (optional).
 * **"version"** - The mod's version (optional, as `#.#.#.#`).
 * **"description"** - The mod's description (optional).
 * **"modDefTypeName"** - Name of the class which extends `ModDef` in
   the mod's assembly.
 * **"asmFileName"** - The name of the mod's code assembly. (See
   `ModDef`, `ModData`, `ModInfo` and `ModLoader` for implementation
   details.)

Then, all resource files (any file that is not the manifest file,
assembly file or any other file with an open file handle) are loaded
into Prism (though it's recommended to make a specific folder for your
resources, e.g. `.\Resources`).
These can be accessed and deserialized using a predefined or custom
deserializer. See the `Prism.Mods.Resources` namespace for more info.

When the `ModDef` implementation is instantiated, control is handed to
it (as far as the Prism engine wants to). This includes loading item
definitions.

See the example mod for... an example. The contents of its output folder
should be put in its own folder with in the `Mods` folder, for example
`Mods\Prism.ExampleMod\ `.

Note that on Windows, building `Prism.ExampleMod.csproj` will
automatically copy the build output of the example mod into its own
folder within the mod folder: `Documents\My Games\Terraria\Prism\Mods\Prism.ExampleMod\`.

