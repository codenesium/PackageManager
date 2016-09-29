using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace Codenesium.PackageManagement
{
    public class ManifestBuilder
    {

        private string _rootDirectory;

        List<ManifestFile> _fileList = new List<ManifestFile>();
        public XElement BuildManifest(string rootDirectory)
        {
            this._fileList.Clear();
            this._rootDirectory = rootDirectory;

            XElement root = new XElement("root");
            XElement xFileList = new XElement("fileList");

            this._fileList = BuildFileList(rootDirectory);
            foreach (ManifestFile file in this._fileList)
            {
                XElement xFile = new XElement("file");
                xFile.SetAttributeValue("name", file.Name);
                xFile.SetAttributeValue("fileHash", file.MD5FileHash);
                xFile.SetAttributeValue("key", file.Key);
                xFile.SetAttributeValue("size", file.SizeInKB.ToString("0.###"));
                xFileList.Add(xFile);
            }

            root.Add(xFileList);

            root.Add(RecursiveDirectory(rootDirectory));
            return root;
        }

        public List<ManifestFile> BuildFileList(string directory)
        {
            List<ManifestFile> fileList = new List<ManifestFile>();
            string[] files = Directory.GetFiles(directory);

            foreach (string file in files)
            {
                ManifestFile manifestFile = new ManifestFile();
                manifestFile.Name = Path.GetFileName(file);
                manifestFile.MD5FileHash = CalculateFileMD5(file);
                manifestFile.Key = CalculateMD5Hash(manifestFile.Name + manifestFile.MD5FileHash);
                manifestFile.SizeInKB = ((decimal)(new FileInfo(file)).Length) / 1024;
                manifestFile.FileLocation = file;
                fileList.Add(manifestFile);
            }

            string[] directories = Directory.GetDirectories(directory);

            foreach (string iterDirectory in directories)
            {
                fileList.AddRange(BuildFileList(iterDirectory));
            }

            fileList.Sort();
            return fileList.Distinct(ManifestFileComparer.Instance).ToList();
        }

        private XElement RecursiveDirectory(string directory)
        {
            XElement response = new XElement("directory");
            response.SetAttributeValue("name", (new DirectoryInfo(directory)).Name);
            string[] files = Directory.GetFiles(directory);

            foreach (string file in files)
            {
                XElement xFile = new XElement("file");
                string fileMD5Hash = CalculateFileMD5(file);
                string key = CalculateMD5Hash(Path.GetFileName(file) + fileMD5Hash);

                ManifestFile lookupTableFile = this._fileList.FirstOrDefault(x => x.Key == key);

                xFile.SetAttributeValue("key", lookupTableFile.Key);
                response.Add(xFile);
            }

            string[] directories = Directory.GetDirectories(directory);

            foreach (string iterDirectory in directories)
            {
                response.Add(RecursiveDirectory(iterDirectory));
            }
            return response;
        }

        private string CalculateFileMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                }
            }
        }

        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
