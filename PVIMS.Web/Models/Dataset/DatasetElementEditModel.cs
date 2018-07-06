using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DatasetElementEditModel : IValidatableObject
    {
        public DatasetElementEditModel()
        {
            DatasetElementSubs = new DatasetElementSubEditModel[0];
            TableHeaderColumns = new DatasetElementTableHeaderRowModel[0];
            InstanceSubValues = new DatasetInstanceSubValueGroupingModel[0];
        }
        public int DatasetElementId { get; set; }
        public string DatasetElementName { get; set; }
        public string DatasetElementDisplayName { get; set; }
        public string DatasetElementHelp { get; set; }
        public bool DatasetElementSystem { get; set; }
        public bool DatasetElementRequired { get; set; }
        public bool DatasetElementDisplayed { get; set; }
        public bool DatasetElementChronic { get; set; }
        public string DatasetElementType { get; set; }
        public string DatasetElementValue { get; set; }
        public DatasetElementSubEditModel[] DatasetElementSubs { get; set; }
        public DatasetElementTableHeaderRowModel[] TableHeaderColumns { get; set; }
        public DatasetInstanceSubValueGroupingModel[] InstanceSubValues { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DatasetElementRequired && string.IsNullOrWhiteSpace(DatasetElementValue))
            {
                yield return new ValidationResult(string.Format("{0} is required", DatasetElementDisplayName), new[] { "DatasetElementValue" });
            }
        }
    }
}