using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.IO;
using Codenesium.PackageManagement;
using System.Configuration;
using System.Timers;
using Codenesium.PackageManagement.DeploymentService;
using System.Reflection;
using System.Xml.Linq;
using Codenesium.PackageManagement.BuildCopyLib;
using Twilio;
using Codenesium.IISIntegration;

namespace DeploymentService
{
    public partial class DeploymentService : ServiceBase
    {
        private string _tmpDirectory;//files being extracted go here
        private string _iisRootDirectory;//where we will copy the extracted applications
        private string _monitorDirectory;//we monitor this directory for zip files
        private string _extractDirectory;//where the rebuilt packages will go
        private string _twilioSID; //used for notifications
        private string _twilioAuthToken;//used for notifications
        private string _twilioNumber;//used for notifications
        private Timer _fileWatcher;
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        private Dictionary<string, DateTime> _fileProcessedDictionary = new Dictionary<string, DateTime>();

        public DeploymentService()
        {
            InitializeComponent();
        }

        public void ConsoleStart()
        {
            OnStart(null);
            while (true)
            {
            }
        }

        protected override void OnStart(string[] args)
        {
            LoadSettings();
            _logger.Info("Service started");
            _logger.Info("Settings _tmpDirectory={0},_monitorDirectory={1},_extractDirectory={2}", this._tmpDirectory, this._monitorDirectory, this._extractDirectory);

            SqlLiteManager.GetInstance().Migrate("database.sqlite");
            this._fileWatcher = new Timer();
            this._fileWatcher.Elapsed += _fileWatcher_Elapsed;
            this._fileWatcher.Interval = 3000;
            this._fileWatcher.Enabled = true;
        }

        private void _fileWatcher_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() => ProcessFileChange());
        }

        private void LoadSettings()
        {
            this._tmpDirectory = ConfigurationManager.AppSettings["tmpDirectory"].ToString();
            this._monitorDirectory = ConfigurationManager.AppSettings["monitorDirectory"].ToString();
            this._extractDirectory = ConfigurationManager.AppSettings["extractDirectory"].ToString();
            this._twilioAuthToken = ConfigurationManager.AppSettings["twilioAuthToken"].ToString();
            this._twilioSID = ConfigurationManager.AppSettings["twilioSID"].ToString();
            this._twilioNumber = ConfigurationManager.AppSettings["twilioNumber"].ToString();
            this._iisRootDirectory = ConfigurationManager.AppSettings["iisRootDirectory"].ToString();
        }

        private async void ProcessFileChange()
        {
            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(this._monitorDirectory);

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).ToUpper() != ".ZIP")
                    {
                        continue;
                    }

                    string packageName = Path.GetFileName(file);

                    if (!SqlLiteManager.GetInstance().GetPackageExists(packageName))
                    {
                        Task.Run(async () =>
                        {
                            _logger.Info("Processsing {0}", packageName);
                            SqlLiteManager.GetInstance().InsertPackage(packageName);
                            SqlLiteManager.GetInstance().UpdateStatus(SqlLiteManager.UPGRADE_STATUS_RUNNING, packageName);
                            await UnzipPackage(file);
                        }
                        ).ContinueWith((x) =>
                        {
                            if (x.IsFaulted)
                            {
                                _logger.Info("Processsing complete with error {0}", packageName);
                                SqlLiteManager.GetInstance().UpdateStatus(SqlLiteManager.UPGRADE_STATUS_FAILED, packageName);
                            }
                            else
                            {
                                FinalizeDeployment(packageName);
                                _logger.Info("Processsing complete {0}", packageName);
                                SqlLiteManager.GetInstance().UpdateStatus(SqlLiteManager.UPGRADE_STATUS_COMPLETE, packageName);
                            }
                        });
                    }
                }
            });
        }

        private void SetupIIS(Project project)
        {
            IISIntegration iisManager = new IISIntegration();
            var siteList = iisManager.GetSiteList();
            var applicationList = iisManager.GetApplicationListForSite(project.Site);
            var appPoolList = iisManager.GetApplicationPoolList();
            if (applicationList.Any(x => x == project.Name))
            {
                iisManager.DeleteApplication(project.Site, project.Name);
            }

            if (appPoolList.Any(x => x == project.Name))
            {
                iisManager.DeleteAppPool(project.Name);
            }
            if (!Directory.Exists(this._iisRootDirectory))
            {
                Directory.CreateDirectory(this._iisRootDirectory);
            }
            iisManager.CreateAppPool(project.Name, "v4.0", false);

            if (!siteList.Any(x => x == project.Site))
            {
                iisManager.CreateWebSite(project.Site, this._iisRootDirectory, 80);
            }

            iisManager.CreateApplication(project.Site, Path.Combine(this._iisRootDirectory, project.Destination), "/" + project.Name, project.Name);
        }

        private void FinalizeDeployment(string packageName)
        {
            try
            {
                string extractDirectory = Path.Combine(this._extractDirectory, Path.GetFileNameWithoutExtension(packageName));

                var s = Path.Combine(extractDirectory, Directory.GetDirectories(extractDirectory).FirstOrDefault());
                string deploymentInstructions = Directory.GetFiles(Path.Combine(extractDirectory, Directory.GetDirectories(extractDirectory).FirstOrDefault()), "deployment.xml").FirstOrDefault();
                if (!String.IsNullOrEmpty(deploymentInstructions))
                {
                    Project project = Project.LoadProjects(deploymentInstructions).FirstOrDefault();

                    _logger.Info("deployment.xml found. Deploying {0} to {1} for project id={2}", extractDirectory, project.Destination, project.Id);

                    try
                    {
                        string destination = Path.Combine(this._iisRootDirectory, project.Destination);
                        DirectoryHelper.DeleteDirectory(destination);
                        DirectoryHelper.Copy(Path.Combine(extractDirectory, Directory.GetDirectories(extractDirectory).FirstOrDefault()), destination); //copy the contents of the first directory found in the package
                        SetupIIS(project);
                        Notify(String.Format("Deployment complete on {0} for project {1}. Package {2}", Environment.MachineName, project.Name, Path.GetFileName(packageName)), project.Notifications);
                    }
                    catch (Exception ex)
                    {
                        Notify(String.Format("Deployment failed on {0} for project {1}. Package {2}. Error {3}.", Environment.MachineName, project.Name, Path.GetFileName(packageName), project.Notifications, ex.Message), project.Notifications);
                    }
                }
                else
                {
                    _logger.Info("deployment.xml not found in extracted folder");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Notify(string message, List<String> phoneNumbers)
        {
            try
            {
                var twilio = new TwilioRestClient(this._twilioSID, this._twilioAuthToken);

                foreach (var number in phoneNumbers)
                {
                    var twilioMessage = twilio.SendMessage(
                        this._twilioNumber, number,
                        message);

                    _logger.Info("Twilio result status={0},ErrorMessage={0}", twilioMessage.Status, twilioMessage.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// Trys to obtain a lock on the file by opening it for writing.
        /// Attempts 5 times in 1 second increments to obtain a log. If it cannot obtain a lock throws an IOException
        /// </summary>
        /// <param name="filename"></param>
        private void CheckLockedFile(string filename)
        {
            int counter = 0;
            while (counter < 5)
            {
                try
                {
                    FileStream fileStream = new FileStream(
                      filename, FileMode.Open,
                      FileAccess.Write);
                    fileStream.Close();
                    break;
                }
                catch (IOException)
                {
                    counter++;
                    System.Threading.Thread.Sleep(1000);
                }
            }

            if (counter == 5)
            {
                throw new IOException(String.Format("We were unable to obtain a file lock on {0}", filename));
            }
        }

        private async Task UnzipPackage(string filename)
        {
            CheckLockedFile(filename);

            ManifestPackager packager = new ManifestPackager();
            string filenameWithoutExtenstion = Path.GetFileNameWithoutExtension(filename);
            string packageTmpDirectory = Path.Combine(this._tmpDirectory, filenameWithoutExtenstion);
            string packageExtractDirectory = Path.Combine(this._extractDirectory, filenameWithoutExtenstion);

            if (!Directory.Exists(packageTmpDirectory))
            {
                Directory.CreateDirectory(packageTmpDirectory);
            }

            if (!Directory.Exists(packageExtractDirectory))
            {
                Directory.CreateDirectory(packageExtractDirectory);
            }

            await packager.ExtractPackage(filename, packageExtractDirectory, packageTmpDirectory);

            File.Delete(filename);

            _logger.Info("Package extraction complete. Filename={0},ExtractDirectory directory={1},tmpDirectory={2}", filename, packageExtractDirectory, packageTmpDirectory);
        }

        protected override void OnStop()
        {
            _logger.Info("Service stopped");
        }
    }
}