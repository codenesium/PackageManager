using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using NLog;

namespace Codenesium.PackageManagement.BuildCopyUtility
{
    public class ProjectManager
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        public List<Project> Projects { get; private set; }
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

            DeleteDirectories(project.DeleteDirectories);

            foreach (CopyTask copyTask in project.CopyTasks)
            {
                _logger.Trace("Executing copy task {0}", copyTask.Name);
                await ExecuteCopyTask(copyTask);
            }
        }

        private void DeleteDirectories(List<string> directories)
        {
            foreach (string directory in directories)
            {
                if (!directory.ToUpper().Contains("TMP") && !directory.ToUpper().Contains("WWWROOT"))
                {
                    _logger.Error("Attempted to delete a directory that does not contain tmp ot wwwroot in the filename");
                    throw new ArgumentException(@"The directories you're deleting must contain tmp or wwwroot in the filename.
                        This is keep us from potentially wrecking a file system.");
                }
                DeleteDirectory(directory);
            }
        }

        private void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                _logger.Trace("Deleting directory {0}", directory);
                int attempts = 0;
                while (attempts < 5)
                {
                    if (attempts >= 5)
                    {
                        throw new Exception("Exceeded attempt count of 5");
                    }
                    if (Directory.Exists(directory))
                    {
                        Directory.Delete(directory, true); //this is potentially dangerous.
                        if (Directory.Exists(directory))
                        {
                            System.Threading.Thread.Sleep(200);
                            attempts++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
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

        private void ReplaceProjectGlobals(Project project)
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
                    taskList.Add(Task.Run(() => { CopyDir.Copy(copyTask.Source, destination); }));
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
                             select new Project
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
                             }).ToList<Project>();

            foreach (string key in this.Globals.Keys)
            {
                _logger.Trace("Global {0}={1}", key, this.Globals[key]);
            }
        }
    }
}