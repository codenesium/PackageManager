using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Codenesium.PackageManagement.DeploymentService
{
    public class Project
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Destination { get; set; }
        public List<string> Notifications = new List<string>();

        public static List<Project> LoadProjects(string filename)
        {
            List<Project> results = new List<Project>();
            XDocument xDoc = XDocument.Load(filename);

            results = (from p in xDoc.Descendants("project")
                       select new Project
                       {
                           Destination = p.Element("destination").Value,
                           Id = Guid.Parse(p.Element("id").Value),
                           Name = p.Element("name").Value,
                           Notifications = (from not in p.Element("notifications").Elements("notification")
                                            select not.Value).ToList()
                       }).ToList();
            return results;
        }
    }
}