# PackageManager
This repo is a collection of tools for deploying software on windows.

The entire chain looks like this.

1. Build your software
2. Use the BuildCopyUtility to combine the files into a deployable directory structure
3. Use the ConnectionStringUtility to replace the connection strings for production
4. Use the ConsolePackager to package this directory into a minified zip file
5. Update your database using FluentDatabase or some other tool
6. Transfer the zip file using the FTPClient
7. Extract and deploy the zip file using the deployment service which runs on your server

I combine these different actions using a powershell script I run from Visual Studio to have a one click build and deploy.

To use the projects I suggest 

1. Building these projects as nuget packages using the BuildNugetPackages.bat
2. Creating a nuget package source in Visual Studio and pointing it to the nuget output
3. Adding a reference to these packages in your solution.
4. Creating a powershell script to combine the various pieces to fit your needs referencing the executables in the packages folder in your solution


##BuildCopyLib
Library of file copy functions

##BuildCopyUtility
Command line utility that uses a config file to copy files around. 

##ConnectionStringUtility
Opens an xml file and replaces a value. I use it for connection string replacement.

##ConsolePackager
Uses command line arguments to create a minified zip file from a directory

##DeploymentService
Windows service that runs on a server. It monitors a directory for zip files and then extracts/recombines/moves
them to the correct directories.

##PackageManagementLib
Library of functionality to create and extract minified zip files

##PackageManagementTester
Windows form tester for the PackageManagementLib






