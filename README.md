# Prism
Modding API for Terraria 1.3

## Building
In order to build Prism, one must provide their own `Terraria.exe` file and put it in the `References` subdirectory.
The build process will patch it automatically and a `Prism.Terraria.dll` file will be created.
This file is required to successfully build `Prism.csproj`.

If that is done, just build the solution in Visual Studio (or use the MSBuild command-line, but don't forget to restore the NuGet packages).
