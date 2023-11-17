namespace Model;

// dotnet pack ./Model.csproj -o /Users/jwallace/other-code/Tools-DotNet/packages -p:Packing=Packaging -p:version=1.1.1
// or in csproj to exclude to file entirely
// dotnet pack Model.csproj --configuration Release --output /nupkgs -p:MODELPACK=true -p:Version=1.1.1
#if !Packaging

interface IAuditable
{
    string AuditName { get; set; }
}

public partial class AppUser : IAuditable
{
    public string AuditName { get; set; } = "Hi, I'm a partial class!2";
}

#endif