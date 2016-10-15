using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using CommandLine;
using CommandLine.Text;

namespace Codenesium.PackageManagement.BuildCopyUtility
{
    internal class Options
    {
        [Option('p', "Project Name", Required = true,
          HelpText = "The project name. Should match a project in the config.xml.")]
        public string ProjectName { get; set; }

        [Option('o', "Output Directory", Required = true,
       HelpText = "The directory where the output will be copied.")]
        public string OutputDirectory { get; set; }

        [Option('r', "Repository root directory",
        HelpText = "Optional. The root directory of your repository. This can be referenced in your config.xml")]
        public string RepositoryRootDirectory { get; set; }

        [Option('s', "Source directory",
        HelpText = "Optional. Any directory that you reference in your conifig.xml.")]
        public string SourceDirectory { get; set; }
    }

    internal class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            { // option
                try
                {
                    foreach (string arg in args)
                    {
                        Console.WriteLine(arg);
                    }
                    ProjectManager manager = new ProjectManager();
                    manager.LoadProjects(options.OutputDirectory, options.RepositoryRootDirectory, options.SourceDirectory);
                    Console.WriteLine("Starting:" + options.ProjectName);
                    _logger.Trace("Starting project {0}", options.ProjectName);
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    Task.WaitAll(manager.ExecuteProject(options.ProjectName));
                    stopwatch.Stop();
                    Console.WriteLine(String.Format("{0} completed in {1} ms", options.ProjectName, stopwatch.ElapsedMilliseconds));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in BuildCopyUtility");
                    _logger.Error("Exception in BuildCopyUtility", ex);
                    Console.Write(ex.ToString());
                    Environment.Exit(-1);
                }
            }).WithNotParsed(errors => { });
        }
    }
}