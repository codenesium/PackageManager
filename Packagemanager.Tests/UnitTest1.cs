using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codenesium.PackageManagement;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace Packagemanager.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PackageFilename()
        {
            string name = Package.PackageFilenameWithExtension("Package", "2016", "3");
            string testName = "Package.2016.3." + DateTime.Now.ToString("Mdd") + "." + DateTime.Now.ToString("Hmm") + ".zip";

            Assert.AreEqual(name, testName);
        }

        [TestMethod]
        public void CreateAndExtractPackageWithManifest()
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var inputDirectory = Path.Combine(baseDir, "input");
            var createPackageOutput = Path.Combine(baseDir, "CreatePackageOutput");
            var extractPackageOutput = Path.Combine(baseDir, "ExtractPackageOutput");
            var tmpDirectory = Path.Combine(baseDir, "tmpDirectory");

            if (Directory.Exists(inputDirectory))
            {
                DeleteFolder(inputDirectory);
            }
            if (Directory.Exists(createPackageOutput))
            {
                DeleteFolder(createPackageOutput);
            }
            if (Directory.Exists(extractPackageOutput))
            {
                DeleteFolder(extractPackageOutput);
            }
            if (Directory.Exists(tmpDirectory))
            {
                DeleteFolder(tmpDirectory);
            }
            Directory.CreateDirectory(inputDirectory);
            Directory.CreateDirectory(createPackageOutput);
            Directory.CreateDirectory(extractPackageOutput);
            Directory.CreateDirectory(tmpDirectory);

            CreateTestFiles(inputDirectory);

            string packageName = Package.PackageFilenameWithExtension("Package_", "2016", "3");
            ManifestPackager packager = new ManifestPackager();

            ManualResetEvent waiter = new ManualResetEvent(false);//this is needed to stop thread abort exceptions see http://www.dankemper.net/t3/index.php/tutorials-menu-top/unit-testing-async-menu-item

            var task = Task.Run(async () =>
            {
                await packager.CreatePackage(inputDirectory, createPackageOutput, tmpDirectory, packageName);
            }).ContinueWith(async action =>
            {
                string filename = Path.Combine(createPackageOutput, packageName);
                await packager.ExtractPackage(filename, extractPackageOutput, tmpDirectory);
                VerifyTestFiles(extractPackageOutput);
                waiter.Set();
            });

            waiter.WaitOne();
        }

        private void CreateTestFiles(string rootDirectory)
        {
            string sub1 = Path.Combine(rootDirectory, "sub1");
            string sub2 = Path.Combine(rootDirectory, "sub2");

            string sub1A = Path.Combine(rootDirectory, "sub1", "A");
            string sub2A = Path.Combine(rootDirectory, "sub2", "A");

            Directory.CreateDirectory(sub1);
            Directory.CreateDirectory(sub2);
            Directory.CreateDirectory(sub1A);
            Directory.CreateDirectory(sub2A);

            File.WriteAllText(Path.Combine(sub1, "1.txt"), "test");
            File.WriteAllText(Path.Combine(sub1A, "2.txt"), "test");
        }

        private void DeleteFolder(string FolderName)
        {
            //http://stackoverflow.com/questions/1288718/how-to-delete-all-files-and-folders-in-a-directory
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                DeleteFolder(di.FullName);
                di.Delete();
            }
        }

        private void VerifyTestFiles(string rootDirectory)
        {
            Assert.IsTrue(Directory.Exists(Path.Combine(rootDirectory, "input", "sub1")));
            Assert.IsTrue(Directory.Exists(Path.Combine(rootDirectory, "input", "sub2")));
            Assert.IsTrue(Directory.Exists(Path.Combine(rootDirectory, "input", "sub1", "A")));
            Assert.IsTrue(Directory.Exists(Path.Combine(rootDirectory, "input", "sub2", "A")));
            Assert.IsTrue(File.Exists(Path.Combine(rootDirectory, "input", "sub1", "1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(rootDirectory, "input", "sub1", "A", "2.txt")));
        }
    }
}