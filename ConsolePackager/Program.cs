using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codenesium.PackageManagement;
using CommandLine;
using CommandLine.Text;

namespace Codenesium.PackageManagement.ConsolePackager
{
    internal class Options
    {
        [Option('i', "input directory", SetName = "Package",
          HelpText = "The input directory to zip")]
        public string InputDirectory { get; set; }

        [Option('d', "destination directory", SetName = "Package",
          HelpText = "The destination directory for the zipped file")]
        public string DestinationDirectory { get; set; }

        [Option('t', "temp directory", SetName = "Package",
  HelpText = "The temporary directory where files will be copied before they are zipped")]
        public string TempDirectory { get; set; }

        [Option('k', "package prefix", SetName = "GetFileName",
          HelpText = "The package name prefix will be in format <PackagePrefix>.<MajorVersion>.<MinorVersion.<Month><Day>.<Hour><Minute>")]
        public string PackagePrefix { get; set; }

        [Option('f', "package filename", SetName = "Package",
          HelpText = "The package filename")]
        public string PackageFilename { get; set; }

        [Option('m', "major version", SetName = "GetFileName",
       HelpText = "The package name prefix will be in format <PackagePrefix>.<MajorVersion>.<MinorVersion.<Month><Day>.<Hour><Minute>")]
        public string MajorVersion { get; set; }

        [Option('n', "minor version", SetName = "GetFileName",
       HelpText = "The package name prefix will be in format <PackagePrefix>.<MajorVersion>.<MinorVersion.<Month><Day>.<Hour><Minute>")]
        public string MinorVersion { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    foreach (string arg in args)
                    {
                        Console.WriteLine(arg);
                    }
                    if (!String.IsNullOrEmpty(options.PackagePrefix))
                    {
                        Console.WriteLine(Package.PackageFilenameWithExtension(
                                        options.PackagePrefix,
                                        options.MajorVersion,
                                        options.MinorVersion));
                    }
                    else
                    {
                        try
                        {
                            PackageManagement.ManifestPackager packager = new ManifestPackager();
                            Task.WaitAll(
                                Task.Run(async () =>
                                {
                                    await packager.CreatePackage(options.InputDirectory,
                                        options.DestinationDirectory,
                                        options.TempDirectory,
                                        options.PackageFilename);
                                }));
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.ToString());
                        }
                    }
                })
                .WithNotParsed(errors =>
                {
                });
        }
    }
}