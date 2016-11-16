using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;

namespace Codenesium.PackageManagement
{
    public class Package
    {
        public async Task UnZipDirectory(string inputFilename, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            if (!File.Exists(inputFilename))
            {
                throw new FileNotFoundException("Input filename was not found " + inputFilename);
            }

            await Task.Run(() =>
            {
                using (ZipFile zip = ZipFile.Read(inputFilename))
                {
                    zip.ExtractAll(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            });
        }

        public async Task UnZipDirectoryLegacyEncrypted(string inputFilename, string outputDirectory,string password)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            if (!File.Exists(inputFilename))
            {
                throw new FileNotFoundException("Input filename was not found " + inputFilename);
            }

            await Task.Run(() =>
            {
                using (ZipFile zip = ZipFile.Read(inputFilename))
                {
                    zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                    zip.Password = password;
                    zip.ExtractAll(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            });
        }

        public async Task ZipDirectory(string inputDirectory, string outputDirectory, string filename)
        {
            if (!Directory.Exists(inputDirectory))
            {
                throw new DirectoryNotFoundException(String.Format("Input directory was not found {0}", inputDirectory));
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string outputFilename = Path.Combine(outputDirectory, filename);

            await Task.Factory.StartNew(() =>
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(inputDirectory);
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    zip.Save(outputFilename);
                }
            });
        }

        public async Task ZipDirectoryLegacyEncryptedPackage(string inputDirectory, string outputDirectory, string filename,string password)
        {
            if (!Directory.Exists(inputDirectory))
            {
                throw new DirectoryNotFoundException(String.Format("Input directory was not found {0}", inputDirectory));
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string outputFilename = Path.Combine(outputDirectory, filename);

            await Task.Factory.StartNew(() =>
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                    zip.Password = password;
                    zip.AddDirectory(inputDirectory);
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    zip.Save(outputFilename);
                }
            });
        }


        public static string PackageFilename(string prefix, string majorVersion, string minorVersion)
        {
            return prefix + "." + majorVersion + "." + minorVersion + "." + DateTime.Now.ToString("Mdd") + "." + DateTime.Now.ToString("Hmm");
        }

        public static string PackageFilenameWithExtension(string prefix, string majorVersion, string minorVersion)
        {
            return PackageFilename(prefix, majorVersion, minorVersion) + ".zip";
        }
    }
}