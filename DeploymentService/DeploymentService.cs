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

namespace DeploymentService
{
    public partial class DeploymentService : ServiceBase
    {
        private string _tmpDirectory;//files being extracted go here
        private string _monitorDirectory;//we monitor this directory for zip files
        private string _extractDirectory;//where the rebuilt packages will go
        private FileSystemWatcher _fileSystemMonitor;
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

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
            this._fileSystemMonitor = new FileSystemWatcher(this._monitorDirectory, "*.zip");
            this._fileSystemMonitor.Changed += FileSystemWatcherBuilds_Changed;
            this._fileSystemMonitor.EnableRaisingEvents = true;
            this._fileSystemMonitor.NotifyFilter = NotifyFilters.Attributes |
    NotifyFilters.CreationTime |
    NotifyFilters.FileName |
    NotifyFilters.LastAccess |
    NotifyFilters.LastWrite |
    NotifyFilters.Size |
    NotifyFilters.Security;
            this._fileSystemMonitor.BeginInit();
            this._fileSystemMonitor.EndInit();
        }

        private void LoadSettings()
        {
            this._tmpDirectory = ConfigurationManager.AppSettings["tmpDirectory"].ToString();
            this._monitorDirectory = ConfigurationManager.AppSettings["monitorDirectory"].ToString();
            this._extractDirectory = ConfigurationManager.AppSettings["extractDirectory"].ToString();
        }

        private void FileSystemWatcherBuilds_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            _logger.Info("Detected file change. Filename={0}", e.FullPath);
            Task.Run(() => UnzipPackage(e.FullPath));
        }

        private async void UnzipPackage(string filename)
        {
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
            _logger.Info("Package extraction complete. Filename={0},ExtractDirectory directory={1},tmpDirectory={2}", filename, packageExtractDirectory, packageTmpDirectory);
        }

        protected override void OnStop()
        {
            _logger.Info("Service stopped");
        }
    }
}