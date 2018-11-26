using System;

namespace PVIMS.Core.Dto
{
    public class DatasetInstanceValueDto
    {
        public string ElementName { get; set; }

        public DateTime? CollectionDate { get; set; }

        public string Value { get; set; }
    }
}
