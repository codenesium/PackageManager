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
            this._fileSystemMonitor = new FileSystemWatcher(this._monitorDirectory, "*.zip");
            this._fileSystemMonitor.Changed += FileSystemWatcherBuilds_Changed;
            this._fileSystemMonitor.Renamed += _fileSystemMonitor_Renamed;
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

        private void _fileSystemMonitor_Renamed(object sender, RenamedEventArgs e)
        {
            ProcessFileChange(e.FullPath);
        }

        private void LoadSettings()
        {
            this._tmpDirectory = ConfigurationManager.AppSettings["tmpDirectory"].ToString();
            this._monitorDirectory = ConfigurationManager.AppSettings["monitorDirectory"].ToString();
            this._extractDirectory = ConfigurationManager.AppSettings["extractDirectory"].ToString();
        }

        private void ProcessFileChange(string filename)
        {
            _logger.Info("Detected file change. Filename={0}", filename);
            DateTime lastWriteTime = System.IO.File.GetLastWriteTime(filename);

            if (this._fileProcessedDictionary.Keys.Contains(filename))
            {
                _logger.Info("File exists in dictionary. Filename={0},LastWriteTime={1}", filename, lastWriteTime);
                if (lastWriteTime != this._fileProcessedDictionary[filename])
                {
                    _logger.Info("LastWriteTime Differs.Processing package. Old={0},New={1}", this._fileProcessedDictionary[filename], lastWriteTime);
                    this._fileProcessedDictionary[filename] = lastWriteTime;
                    Task.Run(() => UnzipPackage(filename));
                }
                else
                {
                    _logger.Info("LastWriteTime is the same.Skipping.");
                }
            }
            else
            {
                _logger.Info("File does not exist in dictionary. Processing package");
                this._fileProcessedDictionary[filename] = lastWriteTime;
                Task.Run(() => UnzipPackage(filename));
            }
        }

        private void FileSystemWatcherBuilds_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            ProcessFileChange(e.FullPath);
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

        private async void UnzipPackage(string filename)
        {
            //  CheckLockedFile(filename);

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

            if (Directory.Exists(packageTmpDirectory))
            {
                Directory.Delete(packageTmpDirectory);
            }
            _logger.Info("Package extraction complete. Filename={0},ExtractDirectory directory={1},tmpDirectory={2}", filename, packageExtractDirectory, packageTmpDirectory);
        }

        protected override void OnStop()
        {
            _logger.Info("Service stopped");
        }
    }
}