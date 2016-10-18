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
        private List<ManifestFile> _fileList = new List<ManifestFile>();
        private string _tmpDirectory;
        private string _destinationDirectory;

        /// <summary>
        /// Extracts the specified input zip file to the specified tmp directory and then rebuilds the
        /// folder structure in the destination directory using the manifest contained in the package.
        /// </summary>
        /// <param name="inputFilename"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="tmpDirectory"></param>
        /// <returns></returns>
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

            IterateAndCreateDirectoryStructure(this._destinationDirectory, manifest);
            IterateAndCreateFileStructure(this._destinationDirectory, manifest);
            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(tmpDirectory);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == String.Empty || Path.GetFileNameWithoutExtension(file).ToUpper() == "MANIFEST")
                    {
                        //this allows us to use the destination directory as the temp directory. The user shouldn't have files without an extension.
                        File.Delete(file);
                    }
                }
                Directory.Delete(tmpDirectory);
            });
        }

        /// <summary>
        /// Uses a manifest definition file to rebuild the directory structure contained in the manifest
        /// </summary>
        /// <param name="destinationDirectory"></param>
        /// <param name="directory"></param>
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

        /// <summary>
        /// Uses a manifest to populate a directory structure with files from a tmp directory
        /// </summary>
        /// <param name="destinationDirectory"></param>
        /// <param name="directory"></param>
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
                File.Copy(Path.Combine(this._tmpDirectory, mainfestFile.Key), Path.Combine(path, mainfestFile.Name), true);
            }

            foreach (XElement subDirectory in directory.Elements("directory"))
            {
                IterateAndCreateFileStructure(path, subDirectory);
            }
        }

        /// <summary>
        /// Creates a zip package with a manifest from an input directory.
        ///
        /// </summary>
        /// <param name="inputDirectory">The directory you want to zip</param>
        /// <param name="destinationDirectory">The destination of the zip file</param>
        /// <param name="tmpDirectory">The directory where renamed files will be copied while zipping</param>
        /// <param name="packageNameWithExtension">The name of the zip file</param>
        /// <returns></returns>
        public async Task CreatePackage(string inputDirectory, string destinationDirectory, string tmpDirectory, string packageNameWithExtension)
        {
            ManifestBuilder builder = new ManifestBuilder();
            List<ManifestFile> fileList = builder.BuildFileList(inputDirectory);

            await CopyFilesToTempDirectory(fileList, tmpDirectory);

            XElement manifest = builder.BuildManifest(inputDirectory);
            File.WriteAllText(Path.Combine(tmpDirectory, "manifest.xml"), manifest.ToString());

            Package packager = new Package();
            await packager.ZipDirectory(tmpDirectory, destinationDirectory, packageNameWithExtension);
            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(tmpDirectory);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == String.Empty || Path.GetFileNameWithoutExtension(file).ToUpper() == "MANIFEST")
                    {
                        //this allows us to use the destination directory as the temp directory. The user shouldn't have files without an extension.
                        File.Delete(file);
                    }
                }
            });
        }

        /// <summary>
        /// Copies a list of files to a directory
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="tmpDirectory"></param>
        /// <returns></returns>
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