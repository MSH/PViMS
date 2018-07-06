namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatasetCategory")]
    public class DatasetCategory : EntityBase
    {
        public DatasetCategory()
        {
            DatasetCategoryElements = new HashSet<DatasetCategoryElement>();
            WorkPlanCareEventDatasetCategories = new HashSet<WorkPlanCareEventDatasetCategory>();
            Conditions = new HashSet<DatasetCategoryCondition>();
        }

        [Required]
        [StringLength(50)]
        public string DatasetCategoryName { get; set; }

        [StringLength(150)]
        public string FriendlyName { get; set; }
        [StringLength(350)]
        public string Help { get; set; }

        [StringLength(10)]
        public string UID { get; set; }

        public bool System { get; set; }
        public bool Public { get; set; }
        public bool Acute { get; set; }
        public bool Chronic { get; set; }

        public short CategoryOrder { get; set; }

        public virtual Dataset Dataset { get; set; }

        public virtual ICollection<DatasetCategoryElement> DatasetCategoryElements { get; set; }
        public virtual ICollection<WorkPlanCareEventDatasetCategory> WorkPlanCareEventDatasetCategories { get; set; }
        public virtual ICollection<DatasetCategoryCondition> Conditions { get; set; }
    }
}
