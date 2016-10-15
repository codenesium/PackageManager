using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace Codenesium.PackageManagement.ConnectionStringUtility
{
    internal class Options
    {
        [Option('f', "Input Filename", Required = true,
          HelpText = "The input filename to modify")]
        public string InputFile { get; set; }

        [Option('r', "XML Node", Required = true,
       HelpText = "the node name to replace the value of")]
        public string ReplaceNode { get; set; }

        [Option('v', "Replacement Value", Required = true,
        HelpText = "the value to replace with")]
        public string ReplaceValue { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
  .WithParsed(options =>
  { // options is an instance of Options type
      foreach (string arg in args)
      {
          Console.WriteLine(arg);
      }

      if (!File.Exists(options.InputFile))
      {
          Console.WriteLine("The supplied file {0} does not exist", options.InputFile);
          Environment.Exit(-1);
      }
      try
      {
          XDocument xDoc = XDocument.Load(options.InputFile);

          var connectionString = (from c in xDoc.Descendants("connectionStrings").Descendants("add")
                                  where c.Attribute("name").Value.ToString().ToUpper() == options.ReplaceNode
                                  select c).FirstOrDefault();

          if (connectionString == null)
          {
              Console.WriteLine("Unable to find node {0} in supplied file. Make sure you're passing the node name in upper case.");
              Environment.Exit(-1);
          }

          connectionString.Attribute("connectionString").Value = options.ReplaceValue;
          xDoc.Save(options.InputFile);
          Console.WriteLine("Connection strings replaced successfully");
          Environment.Exit(0);
      }
      catch (Exception ex)
      {
          Console.WriteLine("Exception changing the connection string file:{0} exception{1}", options.InputFile, ex.ToString());
          Environment.Exit(-1);
      }
  }).WithNotParsed(errors => { });
        }
    }
}