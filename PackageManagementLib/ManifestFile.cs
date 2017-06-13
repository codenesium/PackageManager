using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codenesium.PackageManagementLib
{
    public class ManifestFile : IComparable
    {
        public string FileLocation { get; set; }
        public string Name { get; set; }
        public string MD5FileHash { get; set; }
        public string Key { get; set; }
        public decimal SizeInKB { get; set; }
        public int CompareTo(object obj)
        {
            return this.Name.CompareTo(((ManifestFile)obj).Name);
        }
    }

    class ManifestFileComparer : IEqualityComparer<ManifestFile>
    {
        public static readonly ManifestFileComparer Instance = new ManifestFileComparer();

        public bool Equals(ManifestFile x, ManifestFile y)
        {
            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(ManifestFile obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}
