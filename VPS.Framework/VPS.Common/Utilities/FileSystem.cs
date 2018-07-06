using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.Common.Utilities
{
    public class FileSystem : IFileSystem
    {
        public void Save(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public string ReadAllContents(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
