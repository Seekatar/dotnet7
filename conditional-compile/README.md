# Conditional Compile Testing

This tests conditionally including a file in a `pack` vs `build` scenario.

See the [Model.csproj](./Model.csproj) for the conditional compile settings.

Building the NuGet package

```powershell
# Set Packaging so that the #if is used in AppUserPartial.cs
dotnet pack ./Model.csproj -o /Users/jwallace/other-code/Tools-DotNet/packages -p:Packing=Packaging -p:version=1.1.1

# Set MODELPACK exclude to AppUserPartial.cs entirely
dotnet pack Model.csproj --configuration Release --output /nupkgs -p:MODELPACK=true -p:Version=1.1.1
```
