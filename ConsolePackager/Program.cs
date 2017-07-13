using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codenesium.PackageManagementLib;
using CommandLine;
using CommandLine.Text;

namespace Codenesium.PackageManagement.ConsolePackager
{
    internal class Options
    {
        [Option("InputDirectory", SetName = "Package",
          HelpText = "The input directory to zip")]
        public string InputDirectory { get; set; }

        [Option("DestinationDirectory", SetName = "Package",
          HelpText = "The destination directory for the zipped file")]
        public string DestinationDirectory { get; set; }

        [Option("TempDirectory", SetName = "Package",
  HelpText = "The temporary directory where files will be copied before they are zipped")]
        public string TempDirectory { get; set; }

        [Option("PackagePrefix", SetName = "GetFileName",
          HelpText = "The package name prefix will be in format <PackagePrefix>.<MajorVersion>.<MinorVersion.<Month><Day>.<Hour><Minute>")]
        public string PackagePrefix { get; set; }

        [Option("PackageFilename", SetName = "Package",
          HelpText = "The package filename")]
        public string PackageFilename { get; set; }

        [Option("MajorVersion", SetName = "GetFileName",
       HelpText = "The package name prefix will be in format <PackagePrefix>.<MajorVersion>.<MinorVersion.<Month><Day>.<Hour><Minute>")]
        public string MajorVersion { get; set; }

        [Option("MinorVersion", SetName = "GetFileName",
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
                    if (!String.IsNullOrEmpty(options.PackagePrefix))
                    {
                        Console.WriteLine(Packager.PackageFilenameWithExtension(
                                        options.PackagePrefix,
                                        options.MajorVersion,
                                        options.MinorVersion));
                    }
                    else
                    {
                        foreach (string arg in args)
                        {
                            Console.WriteLine(arg);
                        }
                        try
                        {
                            Packager packager = new Packager();
                            Task.WaitAll(
                                Task.Run(async () =>
                                {
                                    await packager.ZipDirectory(options.InputDirectory,
                                        options.DestinationDirectory,
                                        options.PackageFilename);
                                }));

                            Console.WriteLine("Package complete");
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.ToString());
                            Environment.Exit(-1);
                        }
                    }
                })
                .WithNotParsed(errors =>
                {
                    Environment.Exit(-1);
                });
        }
    }
}