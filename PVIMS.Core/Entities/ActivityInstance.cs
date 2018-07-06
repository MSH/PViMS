using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PVIMS.Core.Entities
{
	public class ActivityInstance : AuditedEntityBase
	{
        public ActivityInstance()
        {
            ExecutionEvents = new HashSet<ActivityExecutionStatusEvent>();
            Current = true;
        }

        public ActivityInstance(Activity activity, User currentUser)
        {
            ExecutionEvents = new HashSet<ActivityExecutionStatusEvent>();
            ExecutionEvents.Add(InitialiseWithFirstExecutionStatus(activity, currentUser));
        }

        [Required]
        public ReportInstance ReportInstance { get; set; }

        [Required]
        [StringLength(50)]
        public string QualifiedName { get; set; }

        public bool Current { get; set; }

        public virtual ActivityExecutionStatus CurrentStatus { get; set; }

        public virtual ICollection<ActivityExecutionStatusEvent> ExecutionEvents { get; set; }

        private ActivityExecutionStatusEvent InitialiseWithFirstExecutionStatus(Activity activity, User currentUser)
        {
            var status = activity.ExecutionStatuses.OrderBy(es => es.Id).First();
            var statusEvent = new ActivityExecutionStatusEvent()
            {
                ActivityInstance = this,
                Comments = "",
                EventCreatedBy = currentUser,
                EventDateTime = DateTime.Now,
                ExecutionStatus = status
            };
            return statusEvent;
        }

        public ActivityExecutionStatusEvent AddNewEvent(ActivityExecutionStatus newStatus, User currentUser, string comments, DateTime? contextDate, string contextCode)
        {
            if(CurrentStatus.Description == newStatus.Description) { return null; };

            var statusEvent = new ActivityExecutionStatusEvent()
            {
                ActivityInstance = this,
                Comments = comments,
                EventCreatedBy = currentUser,
                EventDateTime = DateTime.Now,
                ExecutionStatus = newStatus,
                ContextCode = contextCode,
                ContextDateTime = contextDate
            };

            CurrentStatus = newStatus;
            ExecutionEvents.Add(statusEvent);

            return statusEvent;
        }
    }
}