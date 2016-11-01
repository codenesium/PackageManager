using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;

namespace NotificationUtility
{
    internal class Options
    {
        [Option("SID", Required = true,
          HelpText = "The twilio SID")]
        public string SID { get; set; }

        [Option("Token", Required = true,
  HelpText = "The twilio token")]
        public string Token { get; set; }

        [Option("From", Required = true,
  HelpText = "The twilio number to send from")]
        public string From { get; set; }

        [Option("To", Required = true,
HelpText = "The twilio number to send to")]
        public string To { get; set; }

        [Option("Message", Required = true,
HelpText = "The message to send")]
        public string Message { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    Console.WriteLine("Starting notifications");
                    foreach (string arg in args)
                    {
                        Console.WriteLine(arg);
                    }
                    var twilio = new TwilioRestClient(options.SID, options.Token);

                    var twilioMessage = twilio.SendMessage(
                        options.From,
                        options.To,
                        options.Message);

                    Console.WriteLine("Notifications complete");
                })
                .WithNotParsed(errors => { });
        }
    }
}