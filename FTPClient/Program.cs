using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using System.IO;

namespace FTPClient
{
    internal class Options
    {
        [Option("Username", Required = true,
          HelpText = "The ftp username")]
        public string Username { get; set; }

        [Option("Password", Required = true,
  HelpText = "The ftp password")]
        public string Password { get; set; }

        [Option("Server", Required = true,
HelpText = "The ftp server uri in the format ftp://WEBSERVER")]
        public string Server { get; set; }

        [Option("LocalFilename", Required = true,
HelpText = "The local file to upload")]
        public string LocalFilename { get; set; }

        [Option("RemoteTempFilename", Required = true,
HelpText = "The name of the upload on the server before it is complete")]
        public string RemoteTempFilename { get; set; }

        [Option("RemoteFinalFilename", Required = true,
HelpText = "The name the file will be renamed to when the upload is complete")]
        public string RemoteFinalFilename { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    Console.WriteLine("FTP upload starting");
                    foreach (string arg in args)
                    {
                        Console.WriteLine(arg);
                    }
                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(options.Username, options.Password);
                        Uri remoteTempFilename = new Uri(new Uri(options.Server), options.RemoteTempFilename);
                        client.UploadFile(remoteTempFilename, "STOR", options.LocalFilename);
                        RenameFileName(options.Username, options.Password, remoteTempFilename, options.RemoteFinalFilename);
                    }

                    Console.WriteLine("FTP upload complete");
                })
                .WithNotParsed(errors => { });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //   http://stackoverflow.com/questions/3035610/unable-to-rename-file-with-ftp-methods-when-current-user-directory-is-different
        private static void RenameFileName(string username, string password, Uri remoteTempFilename, string newFilename)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(remoteTempFilename);
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(username, password);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                    ftpStream.Dispose();
                }
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}