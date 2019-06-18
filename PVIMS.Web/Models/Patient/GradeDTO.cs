using System.Collections.Generic;

namespace PVIMS.Web.Models
{
    public class GradeDTO
    {
        public GradeDTO()
        {
            MeddraGradeItems = new List<MeddraGradeItem>();
        }

        public string MeddraTerm { get; set; }
        public string GradingScale { get; set; }

        public List<MeddraGradeItem> MeddraGradeItems { get; set; }

        public class MeddraGradeItem
        {
            public string Grade { get; set; }
            public string Description { get; set; }
        }
    }
}