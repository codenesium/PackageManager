


$copyFoundation = cmd /c   "..\packages\Codenesium.BuildCopyUtility.1.0.0-Beta\lib\net461\BuildCopyUtility.exe"  --ProjectName FoundationPackage --OutputDirectory "C:\tmp\builds\applications\FermataFish" --RepositoryRootDirectory "C:\Users\Administrator\Documents\Mercurial\Foundation" 
Write-Host $copyFoundation

$copyFermataFish =  cmd /c "..\packages\Codenesium.BuildCopyUtility.1.0.0-Beta\lib\net461\BuildCopyUtility.exe" --ProjectName FermataFishPackage --OutputDirectory "C:\tmp\builds\applications\FermataFish" --RepositoryRootDirectory "C:\Users\Administrator\Documents\Mercurial\Foundation" 
Write-Host $copyFermataFish

$changeFermataFishConnectionString =  cmd /c "..\packages\Codenesium.ConnectionStringUtility.1.0.0-Beta\lib\net461\ConnectionStringUtility.exe" --InputFilename "C:\tmp\builds\applications\FermataFish\web.config" --XMLNode "FERMATAFISHENTITIES" --ReplacementValue "data source=localhost;initial catalog=FermataFish;persist security info=True;user id=test;password=test;MultipleActiveResultSets=True;App=EntityFramework"
Write-Host $changeFermataFishConnectionString

$changeFoundationFishConnectionString = cmd /c "..\packages\Codenesium.ConnectionStringUtility.1.0.0-Beta\lib\net461\ConnectionStringUtility.exe" --InputFilename "C:\tmp\builds\applications\FermataFish\web.config" --XMLNode "FOUNDATIONENTITIES" --ReplacementValue "metadata=res://*/FoundationModel.csdl|res://*/FoundationModel.ssdl|res://*/FoundationModel.msl;provider=System.Data.SqlClient;provider connection string='data source=localhost;initial catalog=FermataFish;user id=test;password=test;MultipleActiveResultSets=True;App=EntityFramework'"
Write-Host $changeFoundationFishConnectionString

$getPackageFilename =  cmd /c "..\packages\Codenesium.ConsolePackager.1.0.0-Beta\lib\net461\ConsolePackager.exe" --PackagePrefix "FermataFish" --MajorVersion "1" --MinorVersion "0"
Write-Host "PackageFilename=" $getPackageFilename
$package = cmd /c "..\packages\Codenesium.ConsolePackager.1.0.0-Beta\lib\net461\ConsolePackager.exe" --InputDirectory "C:\tmp\builds\applications\FermataFish" --DestinationDirectory "C:\tmp\builds\packages\FermataFish" --TempDirectory "c:\tmp\builds\tmp" --PackageFilename $getPackageFilename
Write-Host $package

$foundationScripts  =  cmd /c "..\packages\FluentMigrator.1.6.2\tools\migrate" migrate --conn "server=WEBSERVER;user id=test;password=test;database=FermataFish" --provider sqlserver2012 --assembly "C:\Users\Administrator\Documents\Mercurial\Foundation\FoundationWeb\FoundationDatabaseMigrations\bin\Debug\FoundationDatabaseMigrations.dll" --task migrate
Write-Host $foundationScripts

$fermataFishScripts = cmd /c  "..\packages\FluentMigrator.1.6.2\tools\migrate" migrate --conn "server=WEBSERVER;user id=test;password=test;database=FermataFish" --provider sqlserver2012 --assembly "C:\Users\Administrator\Documents\Mercurial\Foundation\\FoundationWeb\FermataFish\FFDatabaseMigrations\bin\Debug\Codenesium.FF.Migrations.dll" --task migrate
Write-Host $fermataFishScripts

$remoteFile = [io.path]::combine("C:\tmp\builds\packages\FermataFish" ,$getPackageFilename);
$deployPackage = cmd /c "..\packages\Codenesium.FTPClient.1.0.0-Beta\lib\net461\FTPClient.exe" --Username "test" --Password "Passw0rd" --Server "ftp://WEBSERVER" --RemoteTempFilename "monitor\FermataFish.tmp" --RemoteFinalFilename $getPackageFilename --LocalFilename $remoteFile
Write-Host $deployPackage 





