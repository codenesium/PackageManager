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
        [Option("InputFilename", Required = true,
          HelpText = "The input filename to modify")]
        public string InputFilename { get; set; }

        [Option("XMLNode", Required = true,
       HelpText = "the node name to replace the value of")]
        public string XMLNode { get; set; }

        [Option("ReplacementValue", Required = true,
        HelpText = "the value to replace with")]
        public string ReplacementValue { get; set; }
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

      if (!File.Exists(options.InputFilename))
      {
          Console.WriteLine("The supplied file {0} does not exist", options.InputFilename);
          Environment.Exit(-1);
      }
      try
      {
          XDocument xDoc = XDocument.Load(options.InputFilename);

          var connectionString = (from c in xDoc.Descendants("connectionStrings").Descendants("add")
                                  where c.Attribute("name").Value.ToString().ToUpper() == options.XMLNode
                                  select c).FirstOrDefault();

          if (connectionString == null)
          {
              Console.WriteLine("Unable to find node {0} in supplied file. Make sure you're passing the node name in upper case.");
              Environment.Exit(-1);
          }

          connectionString.Attribute("connectionString").Value = options.ReplacementValue;
          xDoc.Save(options.InputFilename);
          Console.WriteLine("Connection strings replaced successfully");
          Environment.Exit(0);
      }
      catch (Exception ex)
      {
          Console.WriteLine("Exception changing the connection string file:{0} exception{1}", options.InputFilename, ex.ToString());
          Environment.Exit(-1);
      }
  }).WithNotParsed(errors => { });
        }
    }
}