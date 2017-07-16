using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codenesium.PackageManagementLib;
using CommandLine;
using CommandLine.Text;
using NLog;

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
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            _logger.Info("Starting package");
            foreach (string arg in args)
            {
                _logger.Debug($"Command line argument {arg}");
            }

            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    if (!String.IsNullOrEmpty(options.PackagePrefix))
                    {
                        //return a package name mode
                        Console.WriteLine(Packager.PackageFilenameWithExtension(
                                        options.PackagePrefix,
                                        options.MajorVersion,
                                        options.MinorVersion));
                    }
                    else
                    {
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

                           _logger.Info("Package complete");
                        }
                        catch (Exception ex)
                        {
                            _logger.Fatal(ex.ToString());
                            Environment.Exit(-1);
                        }
                    }
                })
                .WithNotParsed(errors =>
                {
                    _logger.Info("Invalid parameters");
                    Environment.Exit(-1);
                });
        }
    }
}