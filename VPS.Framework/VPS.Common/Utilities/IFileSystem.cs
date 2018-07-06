using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.Common.Utilities
{
    public interface IFileSystem
    {
        void Save(string path, string contents);

        string ReadAllContents(string path);
    }
}
