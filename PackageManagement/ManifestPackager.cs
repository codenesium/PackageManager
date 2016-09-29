using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Codenesium.PackageManagement
{
    public class ManifestPackager
    {

        List<ManifestFile> _fileList = new List<ManifestFile>();
        string _tmpDirectory;
        string _destinationDirectory;
        public async Task ExtractPackage(string inputFilename, string destinationDirectory, string tmpDirectory)
        {
            this._tmpDirectory = tmpDirectory;
            this._destinationDirectory = destinationDirectory;
            this._fileList.Clear();

            Package packager = new Package();
            await packager.UnZipDirectory(inputFilename, this._tmpDirectory);

            string manifestFilename = Path.Combine(this._tmpDirectory, "manifest.xml");

            XElement manifest = XElement.Load(manifestFilename);

            foreach (XElement file in manifest.Element("fileList").Elements("file"))
            {
                ManifestFile newFile = new ManifestFile();
                newFile.Name = file.Attribute("name").Value;
                newFile.Key = file.Attribute("key").Value;
                newFile.MD5FileHash = file.Attribute("fileHash").Value;
                newFile.SizeInKB = decimal.Parse(file.Attribute("size").Value);
                this._fileList.Add(newFile);
            }

             IterateAndCreateDirectoryStructure(this._destinationDirectory,manifest);
             IterateAndCreateFileStructure(this._destinationDirectory, manifest);
            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(tmpDirectory);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            });
        }


        private void IterateAndCreateDirectoryStructure(string destinationDirectory, XElement directory)
        {
            string path = destinationDirectory;
            if (directory.Attribute("name") != null)
            {
                path = Path.Combine(destinationDirectory, directory.Attribute("name")?.Value.ToString());
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (XElement subDirectory in directory.Elements("directory"))
            {
                IterateAndCreateDirectoryStructure(path, subDirectory);
            }
        }

        private void IterateAndCreateFileStructure(string destinationDirectory, XElement directory)
        {
            string path = destinationDirectory;
            if (directory.Attribute("name") != null)
            {
                path = Path.Combine(destinationDirectory, directory.Attribute("name")?.Value.ToString());
            }

            foreach (XElement file in directory.Elements("file"))
            {
                string key = file.Attribute("key").Value;

                ManifestFile mainfestFile = this._fileList.Where(x => x.Key == key).FirstOrDefault();
                File.Copy(Path.Combine(this._tmpDirectory, mainfestFile.Key), Path.Combine(path, mainfestFile.Name),true);

            }

            foreach (XElement subDirectory in directory.Elements("directory"))
            {
                IterateAndCreateFileStructure(path, subDirectory);
            }
        }
        public async Task CreatePackage(string inputDirectory,string destinationDirectory, string tmpDirectory,string prefix, string majorVersion,string minorVersion)
        {
            ManifestBuilder builder = new ManifestBuilder();
            List<ManifestFile> fileList = builder.BuildFileList(inputDirectory);

            await CopyFilesToTempDirectory(fileList, tmpDirectory);

            XElement manifest = builder.BuildManifest(inputDirectory);
            File.WriteAllText(Path.Combine(tmpDirectory,"manifest.xml"), manifest.ToString());

            Package packager = new Package();
            await packager.ZipDirectory(tmpDirectory, destinationDirectory, Package.PackageFilenameWithExtension(prefix,majorVersion, minorVersion));
            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(tmpDirectory);
                foreach(string file in files)
                {
                    File.Delete(file);
                }
            });
        }

        private async Task CopyFilesToTempDirectory(List<ManifestFile> fileList, string tmpDirectory)
        {
            await Task.Run(() =>
            {
                foreach (ManifestFile file in fileList)
                {
                    File.Copy(file.FileLocation, Path.Combine(tmpDirectory, file.Key), true);
                }
            });

        }
    }
}
