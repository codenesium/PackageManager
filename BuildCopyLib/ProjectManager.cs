using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using NLog;

namespace Codenesium.PackageManagement.BuildCopyLib
{
    public class ProjectManager
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        public List<CopyProject> Projects { get; private set; }
        public Dictionary<string, string> Globals { get; private set; } = new Dictionary<string, string>();

        public async Task ExecuteProject(string name)
        {
            var project = this.Projects.FirstOrDefault(x => x.Name == name);
            if (project == null)
            {
                _logger.Error("Project {0} not found", name);
                throw new ArgumentException("Project {0} not found", name);
            }

            ReplaceProjectGlobals(project);

            foreach (CopyTask copyTask in project.CopyTasks)
            {
                ReplaceTaskGlobals(copyTask);
            }

            foreach (CopyTask copyTask in project.CopyTasks)
            {
                VerifyPaths(copyTask);
            }

            DirectoryHelper.DeleteDirectories(project.DeleteDirectories);

            foreach (CopyTask copyTask in project.CopyTasks)
            {
                _logger.Trace("Executing copy task {0}", copyTask.Name);
                await ExecuteCopyTask(copyTask);
            }
        }

        private void VerifyPaths(CopyTask copyTask)
        {
            if (!File.Exists(copyTask.Source) && !Directory.Exists(copyTask.Source))
            {
                _logger.Error("Source file / directory not found {0}", copyTask.Source);
                throw new ArgumentException("Source file/directory not found");
            }
            foreach (string destination in copyTask.Destinations)
            {
                if (copyTask.IsDirectory)
                {
                    if (!Directory.Exists(destination))
                    {
                        _logger.Trace("Creating directory {0}", destination);
                        Directory.CreateDirectory(destination);
                    }
                }
            }
        }

        private void ReplaceTaskGlobals(CopyTask copyTask)
        {
            foreach (string key in this.Globals.Keys)
            {
                copyTask.Source = copyTask.Source.Replace(key, this.Globals[key]);
                for (int i = 0; i < copyTask.Destinations.Count; i++)
                {
                    _logger.Trace("Replacing global task {0} {1}", copyTask.Destinations[i], this.Globals[key]);
                    copyTask.Destinations[i] = copyTask.Destinations[i].Replace(key, this.Globals[key]);
                }
            }
        }

        private void ReplaceProjectGlobals(CopyProject project)
        {
            foreach (string key in this.Globals.Keys)
            {
                for (int i = 0; i < project.DeleteDirectories.Count; i++)
                {
                    _logger.Trace("Replacing global key {0} {1}", project.DeleteDirectories[i], this.Globals[key]);
                    project.DeleteDirectories[i] = project.DeleteDirectories[i].Replace(key, this.Globals[key]);
                }
            }
        }

        private async Task ExecuteCopyTask(CopyTask copyTask)
        {
            List<Task> taskList = new List<Task>();

            foreach (string destination in copyTask.Destinations)
            {
                if (copyTask.IsDirectory)
                {
                    taskList.Add(Task.Run(() => { DirectoryHelper.Copy(copyTask.Source, destination); }));
                }
                else
                {
                    _logger.Trace("Copying from {0} to {1}", copyTask.Source, destination);
                    taskList.Add(Task.Run(() => { File.Copy(copyTask.Source, destination, true); }));
                }
            }
            await Task.WhenAll(taskList.ToArray());
        }

        public void LoadProjects(string outputDirectory, string repositoryRootDirectory, string sourceDirectory)
        {
            string filename = Path.Combine(Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), "config.xml");
            if (!File.Exists(filename))
            {
                _logger.Error("File config.xml not found in executable directory");
                throw new ArgumentException("File config.xml not found in executable directory");
            }

            XDocument xDoc = null;

            try
            {
                xDoc = XDocument.Load(filename);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ex.Data.Add("message", "There was an exception loading the config.xml file. This likely means your XML is not valid.");
                throw;
            }

            //add any globals from the config file
            this.Globals = (from g in xDoc.Element("projects").Element("globals").Elements("global")
                            select new
                            {
                                Key = g.Element("key").Value,
                                Value = g.Element("value").Value
                            }).ToDictionary(o => o.Key, o => o.Value);

            //add the command line globals
            this.Globals["$(OutputDirectory)"] = outputDirectory;
            this.Globals["$(RepositoryRootDirectory)"] = repositoryRootDirectory;
            this.Globals["$(SourceDirectory)"] = sourceDirectory;

            this.Projects = (from p in xDoc.Element("projects").Elements("project")
                             select new CopyProject
                             {
                                 Name = p.Element("name").Value,
                                 DeleteDirectories = p.Element("deleteDirectories").Elements("directory").Select(x => x.Value).ToList(),

                                 CopyTasks = (from ct in p.Element("copyTasks").Elements("copyTask")
                                              select new CopyTask
                                              {
                                                  Name = ct.Element("name").Value,
                                                  IsDirectory = Convert.ToBoolean(ct.Element("isDirectory").Value),
                                                  Description = ct.Element("description").Value,
                                                  Destinations = (from dest in ct.Element("destinations").Elements("destination")
                                                                  select dest.Value).ToList<string>(),
                                                  Source = ct.Element("source").Value
                                              }).ToList<CopyTask>()
                             }).ToList<CopyProject>();

            foreach (string key in this.Globals.Keys)
            {
                _logger.Trace("Global {0}={1}", key, this.Globals[key]);
            }
        }
    }
}