﻿using System;
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

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            try
            {
                _logger.Trace("Copy source={0} targetDirectory={1}", sourceDirectory, targetDirectory);
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
                    while (directoryCreationAttempts < 5)
                    {
                        Directory.CreateDirectory(target.FullName);
                        if (Directory.Exists(target.FullName))
                        {
                            directoryCreationAttempts = 5;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                            if (directoryCreationAttempts >= 5)
                            {
                                throw new Exception("Exceeded attempt count of 5");
                            }
                            directoryCreationAttempts++;
                        }
                    }
                }

                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    int attempts = 0;
                    while (attempts < 5)
                    {
                        try
                        {
                            _logger.Trace("Copying file source={0} targetDirectory={1}", fi.Name, Path.Combine(target.FullName, fi.Name));
                            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                            attempts = 5;
                        }
                        catch (Exception)
                        {
                            System.Threading.Thread.Sleep(1000);
                            if (attempts >= 5)
                            {
                                throw new Exception("Exceeded attempt count of 5");
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
                if (!directory.ToUpper().Contains("TMP") && !directory.ToUpper().Contains("WWWROOT"))
                {
                    throw new ArgumentException(@"The directories you're deleting must contain tmp or wwwroot in the filename.
                        This is keep us from potentially wrecking a file system.");
                }
                DeleteDirectory(directory);
            }
        }

        // http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
        public static void DeleteDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            string[] dirs = Directory.GetDirectories(directory);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            int attempts = 0;
            while (attempts < 5)
            {
                try
                {
                    Directory.Delete(directory, false);
                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(200);
                    if (attempts >= 5)
                    {
                        throw new Exception("Exceeded attempt count of 5 trying to delete direcotry " + directory);
                    }
                    attempts++;
                }
            }
        }
    }
}