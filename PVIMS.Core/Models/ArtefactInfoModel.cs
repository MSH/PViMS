using System;

namespace PVIMS.Core.Models
{
    public class ArtefactInfoModel
    {
        public string FileName { get; set; }
        public string RelativePath { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; }

        public string FullPath
        {
            get { return String.Format("{0}{1}", Path, FileName); }
        }
    }
}
