using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
	public abstract class AuditedEntityBase : AuditedEntity<int, User>
	{
        public string GetCreatedStamp()
        {
            return string.Format("Created by {0} on {1}", CreatedBy != null ? CreatedBy.FullName : "UNKNOWN", Created.ToString("yyyy-MM-dd"));
        }

        public string GetLastUpdatedStamp()
        {
            if (!LastUpdated.HasValue)
                return "NOT UPDATED";

            return string.Format("Updated by {0} on {1}", UpdatedBy != null ? UpdatedBy.FullName : "UNKNOWN", LastUpdated.Value.ToString("yyyy-MM-dd"));
        }
	}
}
