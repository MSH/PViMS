using System;

namespace PVIMS.Web.Models
{
    public class PageHeaderDetail
    {
        public PageHeaderDetail()
        {
            MetaPageId = 0;
            MetaReportId = 0;
        }

        public string Icon { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int MetaPageId { get; set; }
        public int MetaReportId { get; set; }

        public string MetaDataLastUpdated { get; set; }
    }
}
