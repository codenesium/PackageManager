using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using CommandLine;
using CommandLine.Text;
using Codenesium.PackageManagement.BuildCopyLib;

namespace Codenesium.PackageManagement.BuildCopyUtility
{
    internal class Options
    {
        [Option("ProjectName", Required = true,
          HelpText = "The project name. Should match a project in the config.xml.")]
        public string ProjectName { get; set; }

        [Option("OutputDirectory", Required = true,
       HelpText = "The directory where the output will be copied.")]
        public string OutputDirectory { get; set; }

        [Option("RepositoryRootDirectory",
        HelpText = "Optional. The root directory of your repository. This can be referenced in your config.xml")]
        public string RepositoryRootDirectory { get; set; }

        [Option("SourceDirectory",
        HelpText = "Optional. Any directory that you reference in your conifig.xml.")]
        public string SourceDirectory { get; set; }
    }

    internal class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            _logger.Info("Starting copy");
            foreach (string arg in args)
            {
                _logger.Debug($"Command line argument {arg}");
            }

            var result = Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                try
                {
                    ProjectManager manager = new ProjectManager();
                    manager.LoadProjects(options.OutputDirectory, options.RepositoryRootDirectory, options.SourceDirectory);
                    _logger.Info($"Starting project { options.ProjectName}");
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    Task.WaitAll(manager.ExecuteProject(options.ProjectName));
                    stopwatch.Stop();
                    _logger.Info($"{options.ProjectName} completed in {stopwatch.ElapsedMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex);
                    Environment.Exit(-1);
                }
            }).WithNotParsed(errors => 
            {
                _logger.Fatal("Invalid arguments");
            });
        }
    }
}