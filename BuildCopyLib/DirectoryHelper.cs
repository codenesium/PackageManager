using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Runtime.InteropServices;
using ZetaLongPaths;

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
                ZlpDirectoryInfo diSource = new ZlpDirectoryInfo(sourceDirectory);
                ZlpDirectoryInfo diTarget = new ZlpDirectoryInfo(targetDirectory);
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
        private static void CopyAll(ZlpDirectoryInfo source, ZlpDirectoryInfo target)
        {
            try
            {
                // Check if the target directory exists; if not, create it.
                if (!ZlpIOHelper.DirectoryExists(target.FullName))
                {
                    int directoryCreationAttempts = 0;
                    while (directoryCreationAttempts < MAX_RETRY)
                    {
                        ZlpIOHelper.CreateDirectory(target.FullName);
                        if (ZlpIOHelper.DirectoryExists(target.FullName))
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
                foreach (ZlpFileInfo fi in source.GetFiles())
                {
                    int attempts = 0;
                    while (attempts < MAX_RETRY)
                    {
                        try
                        {
                            _logger.Trace($"Copying file source={fi.Name},targetDirectory={ZlpPathHelper.Combine(target.FullName, fi.Name)}");
                            fi.CopyTo(ZlpPathHelper.Combine(target.FullName, fi.Name), true);
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
                foreach (ZlpDirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    ZlpDirectoryInfo nextTargetSubDir =
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
      
            string longFilename = @"\\?\" + filename;
            _logger.Trace($"Deleting file={longFilename}");

            int fileAttempts = 0;
            while (fileAttempts < MAX_RETRY)
            {
                try
                {
                    ZlpFileInfo file = new ZlpFileInfo(longFilename);
                    ZlpIOHelper.DeleteFile(longFilename);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Trace($"Exception deleting file. Will retry. filename={longFilename}, message={ex.Message}");
                    System.Threading.Thread.Sleep(RETRY_DELAY_MS);
                    if (fileAttempts >= MAX_RETRY)
                    {
                        throw new Exception($"Exceeded attempt count of {MAX_RETRY} trying to delete filename={longFilename}");
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
            _logger.Trace($"Deleting directory={directory}.");

            if (!ZlpIOHelper.DirectoryExists(directory))
            {
                _logger.Trace($"Directory doesn't exist. Returning. directory={directory}.");
                return;
            }

            ZlpDirectoryInfo source = new ZlpDirectoryInfo(directory);

            ZlpFileInfo [] files = source.GetFiles(directory);
            ZlpDirectoryInfo[] dirs = source.GetDirectories(directory);

            _logger.Trace($"File count={files.Length}. Directory count={dirs.Length}");

            foreach (var file in files)
            {
               DeleteFile(file.FullName);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir.FullName);
            }
        }

        /// <summary>
        /// Deletes a single directory without attempting to delete the contents
        /// </summary>
        /// <param name="directory"></param>
        private static void DeleteDirectorySingle(string directory)
        {
            _logger.Trace($"Deleting directory={directory}");

            if (!ZlpIOHelper.DirectoryExists(directory))
            {
                _logger.Trace($"Directory doesn't exist. Returning. directory={directory}.");
                return;
            }

            ZlpDirectoryInfo source = new ZlpDirectoryInfo(directory);
            if(source.GetFiles(directory).Length > 0)
            {
                throw new Exception("Directory contains files.");
            }

            int attempts = 0;
            while (attempts < MAX_RETRY)
            {
                try
                {
                    _logger.Trace($"Deleting directory={directory}. Attempts={attempts}");
                    if (ZlpIOHelper.DirectoryExists(directory))
                    {
                        ZlpIOHelper.DeleteDirectory(directory, false);
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
            _logger.Trace($"Deleting directory={directory}");

            if (!ZlpIOHelper.DirectoryExists(directory))
            {
                _logger.Trace($"Directory doesn't exist. Returning. directory={directory}.");
                return;
            }

            DeleteDirectoryContents(directory);

            DeleteDirectorySingle(directory);
        }
    }
}