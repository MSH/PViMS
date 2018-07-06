using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PVIMS.Core.Entities
{
	public class Dataset : AuditedEntityBase
	{
		public Dataset()
		{
            Created = DateTime.Now;
			DatasetCategories = new HashSet<DatasetCategory>();
            DatasetRules = new HashSet<DatasetRule>();
		}

		[Required]
		[StringLength(50)]
		public string DatasetName { get; set; }
		public virtual ContextType ContextType { get; set; }

        [StringLength(10)]
        public string UID { get; set; }

		[StringLength(100)]
		public string InitialiseProcess { get; set; }

		[StringLength(100)]
		public string RulesProcess { get; set; }

		[StringLength(250)]
		public string Help { get; set; }

        public bool IsSystem { get; set; }
        public bool Active { get; set; }

        public EncounterTypeWorkPlan EncounterTypeWorkPlan { get; set; }
        public virtual DatasetXml DatasetXml { get; set; }

		public virtual ICollection<DatasetCategory> DatasetCategories { get; set; }
        public virtual ICollection<DatasetRule> DatasetRules { get; set; }

        public DatasetInstance CreateInstance(int contextId, EncounterTypeWorkPlan encounterTypeWorkPlan)
        {
            var instance = new DatasetInstance
            {
                Dataset = this,
                EncounterTypeWorkPlan = encounterTypeWorkPlan,
                ContextID = contextId
            };

            // Create table elements automatically
            foreach(DatasetCategory cat in DatasetCategories)
            {
                foreach(DatasetCategoryElement ele in cat.DatasetCategoryElements)
                {
                    if(ele.DatasetElement.Field.FieldType.Description == "Table")
                    {
                        instance.AddInstanceValue(ele.DatasetElement, "<<Table>>");
                    }
                }
            }

            return instance;
        }

        public DatasetRule GetRule(DatasetRuleType ruleType)
        {
            var rule = DatasetRules.SingleOrDefault(dr => dr.RuleType == ruleType);
            if (rule == null)
            {
                rule = new DatasetRule()
                {
                    Dataset = this,
                    RuleActive = false,
                    RuleType = ruleType
                };
                DatasetRules.Add(rule);
            }

            return rule;
        }
    }
}