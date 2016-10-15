using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Codenesium.PackageManagement.BuildCopyUtility
{
    public class CopyTask
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        public string Name { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public bool IsDirectory { get; set; }
        public List<string> FileExtensionIncludes { get; set; } //files with this extension will be included. Defaults to all.
        public List<string> FileExtensionExcludes { get; set; } //files with this extension will be excluded. Defaults to none.
        public List<string> Destinations { get; set; }

        public CopyTask()
        {
        }
    }
}