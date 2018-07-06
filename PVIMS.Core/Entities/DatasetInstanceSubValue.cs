using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
    public class DatasetInstanceSubValue : EntityBase
    {
        protected DatasetInstanceSubValue() { }

        public DatasetInstanceSubValue(DatasetElementSub datasetElementSub, DatasetInstanceValue datasetInstanceValue, string instanceSubValue)
            : this(datasetElementSub, datasetInstanceValue, instanceSubValue, default(Guid))
        {
        }

        public DatasetInstanceSubValue(DatasetElementSub datasetElementSub, DatasetInstanceValue datasetInstanceValue, string instanceSubValue, Guid contextValue)
        {
            if (contextValue == default(Guid))
            {
                ContextValue = Guid.NewGuid();
            }
            else
            {
                ContextValue = contextValue;
            }

            this.DatasetElementSub = datasetElementSub;
            this.DatasetInstanceValue = datasetInstanceValue;
            this.InstanceValue = instanceSubValue;
        }

        [Required]
        public virtual DatasetInstanceValue DatasetInstanceValue { get; set; }

        [Required]
        public virtual DatasetElementSub DatasetElementSub { get; set; }

        [Required]
        public Guid ContextValue { get; set; }

        [Required]
        public string InstanceValue { get; set; }
    }
}