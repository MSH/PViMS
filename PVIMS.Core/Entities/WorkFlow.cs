using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class WorkFlow : EntityBase
	{
		public WorkFlow()
		{
            WorkFlowGuid = Guid.NewGuid();

            Activities = new HashSet<Activity>();
		}

        [Required]
		[StringLength(100)]
		public string Description { get; set; }

        public Guid WorkFlowGuid { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
	}
}