
set PKG_VER=2019.1.7

nuget pack ../BuildCopyUtility/BuildCopyUtility.nuspec  -Version %PKG_VER% -Prop Configuration=Release -o  ../../NugetPackages
nuget pack ../BuildCopyLib/BuildCopyLib.nuspec   -Version %PKG_VER% -Prop Configuration=Release -o  ../../NugetPackages
nuget pack ../ConnectionStringUtility/ConnectionStringUtility.nuspec  -Version %PKG_VER%  -Prop Configuration=Release  -o  ../../NugetPackages
nuget pack ../ConsolePackager/ConsolePackager.nuspec  -Version %PKG_VER%   -Prop Configuration=Release -o  ../../NugetPackages
nuget pack ../FTPClient/FTPClient.nuspec  -Version %PKG_VER%  -o  ../../NugetPackages
nuget pack ../PackageManagementLib/PackageManagementLib.nuspec  -Version %PKG_VER%  -Prop Configuration=Release  -o ../../NugetPackages
nuget pack ../NotificationUtility/NotificationUtility.nuspec  -Version %PKG_VER% -Prop Configuration=Release   -o ../../NugetPackages
