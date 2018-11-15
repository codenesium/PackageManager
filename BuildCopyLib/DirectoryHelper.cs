using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog;

namespace Codenesium.PackageManagement.BuildCopyLib
{
    public class DirectoryHelper
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        //ripped from http://stackoverflow.com/questions/58744/best-way-to-copy-the-entire-contents-of-a-directory-in-c-sharp
        private const int MAX_RETRY = 10;
        private const int RETRY_DELAY_MS = 200;
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            try
            {
                _logger.Trace($"Copy source={sourceDirectory} targetDirectory={targetDirectory}");
                DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
                DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
                CopyAll(diSource, diTarget);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Uses recursion to copy a directory to another directory. Throws exceptions.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                // Check if the target directory exists; if not, create it.
                if (!Directory.Exists(target.FullName))
                {
                    int directoryCreationAttempts = 0;
                    while (directoryCreationAttempts < MAX_RETRY)
                    {
                        Directory.CreateDirectory(target.FullName);
                        if (Directory.Exists(target.FullName))
                        {
                            break;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(RETRY_DELAY_MS);
                            if (directoryCreationAttempts >= MAX_RETRY)
                            {
                                throw new Exception($"Exceeded attempt count of {MAX_RETRY}");
                            }
                            directoryCreationAttempts++;
                        }
                    }
                }

                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    int attempts = 0;
                    while (attempts < MAX_RETRY)
                    {
                        try
                        {
                            _logger.Trace("$Copying file source={fi.Name},targetDirectory={Path.Combine(target.FullName, fi.Name)}");
                            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                            File.SetAttributes(Path.Combine(target.FullName, fi.Name), FileAttributes.Normal);
                            attempts = MAX_RETRY;
                        }
                        catch (Exception ex)
                        {
                            _logger.Trace($"Exception copying. Will retry. directory={fi.FullName},message={ex.Message}");
                            System.Threading.Thread.Sleep(RETRY_DELAY_MS);
                            if (attempts >= MAX_RETRY)
                            {
                                throw new Exception($"Exceeded attempt count of {MAX_RETRY}. filename={fi.FullName}");
                            }
                            attempts++;
                        }
                    }
                }

                // Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteDirectories(List<string> directories)
        {
            foreach (string directory in directories)
            {
                if (!directory.ToUpper().Contains("TMP") && !directory.ToUpper().Contains("WWWROOT") && !directory.ToUpper().Contains("NEBULA"))
                {
                    throw new ArgumentException(@"The directories you're deleting must contain tmp or wwwroot in the filename.
                        This is keep us from potentially wrecking a file system.");
                }
                DeleteDirectory(directory);
            }
        }


        public static void DeleteFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            _logger.Trace($"Deleting file={filename}");
            int fileAttempts = 0;
            while (fileAttempts < MAX_RETRY)
            {
                try
                {
                    if (!File.Exists(filename))
                    {
                        return;
                    }
                    File.SetAttributes(filename, FileAttributes.Normal);
                    File.Delete(filename);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Trace($"Exception deleting file. Will retry. filename={filename},message={ex.Message}");
                    System.Threading.Thread.Sleep(RETRY_DELAY_MS);
                    if (fileAttempts >= MAX_RETRY)
                    {
                        throw new Exception($"Exceeded attempt count of {MAX_RETRY} trying to delete filename={filename}");
                    }
                    fileAttempts++;
                }
            }
        }

        /// <summary>
        /// Deletes the contents of a directory including files and folders but leaves the root directory
        /// </summary>
        /// <param name="directory"></param>
        public static void DeleteDirectoryContents(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            string[] files = Directory.GetFiles(directory);
            string[] dirs = Directory.GetDirectories(directory);

            foreach (string file in files)
            {
               DeleteFile(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
        }

        /// <summary>
        /// Deletes a single directory without attempting to delete the contents
        /// </summary>
        /// <param name="directory"></param>
        private static void DeleteDirectorySingle(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            int attempts = 0;
            while (attempts < MAX_RETRY)
            {
                try
                {
                    _logger.Trace($"Deleting directory={directory}");
                    if (Directory.Exists(directory))
                    {
                        Directory.Delete(directory, false);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Trace($"Exception deleting. Will retry. directory={directory},message={ex.Message}");
                    System.Threading.Thread.Sleep(RETRY_DELAY_MS);
                    if (attempts >= MAX_RETRY)
                    {
                        throw new Exception($"Exceeded attempt count of {MAX_RETRY} trying to delete directory={directory}");
                    }
                    attempts++;
                }
            }
        }

        /// <summary>
        /// Deletes the contents of a directory and the root directory
        /// </summary>
        /// <param name="directory"></param>
        // http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
        public static void DeleteDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            DeleteDirectoryContents(directory);

            DeleteDirectorySingle(directory);
        }
    }
}