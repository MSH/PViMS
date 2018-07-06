using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class ActivityExecutionStatus : EntityBase
	{
        [Required]
        public virtual Activity Activity { get; set; }

        [Required]
		[StringLength(50)]
		public string Description { get; set; }

        [StringLength(100)]
        public string FriendlyDescription { get; set; }

    }
}