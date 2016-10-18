using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codenesium.PackageManagement.BuildCopyLib
{
    public class CopyProject
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        public List<CopyTask> CopyTasks { get; set; }
        public List<String> DeleteDirectories { get; set; }

        public string Name { get; set; }

        public CopyProject()
        {
        }
    }
}