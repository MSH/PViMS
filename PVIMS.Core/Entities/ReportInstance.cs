using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace PVIMS.Core.Entities
{
	public class ReportInstance : AuditedEntityBase
	{
        public ReportInstance()
        {
            ReportInstanceGuid = Guid.NewGuid();
            Identifier = "TBD";

            Activities = new HashSet<ActivityInstance>();
            Medications = new HashSet<ReportInstanceMedication>();
        }

        public ReportInstance(WorkFlow workFlow, User currentUser)
		{
            ReportInstanceGuid = Guid.NewGuid();
            Identifier = "TBD";

            WorkFlow = workFlow;

            Activities = new HashSet<ActivityInstance>();
            Activities.Add(InitialiseWithFirstActivity(workFlow, currentUser));
        }

        [Required]
        public Guid ReportInstanceGuid { get; set; }

        [Required]
        public virtual WorkFlow WorkFlow { get; set; }

        public DateTime? Finished { get; set; }

        public Guid ContextGuid { get; set; }

        [Required]
        public string Identifier { get; set; }

        [Required]
        public string PatientIdentifier { get; set; }

        [Required]
        public string SourceIdentifier { get; set; }

        public virtual TerminologyMedDra TerminologyMedDra { get; set; }

        public virtual ICollection<ActivityInstance> Activities { get; set; }
        public virtual ICollection<ReportInstanceMedication> Medications { get; set; }

        public void SetIdentifier()
        {
            if(!string.IsNullOrWhiteSpace(Identifier))
            {
                Identifier = string.Format("{0}/{1}/{2}", WorkFlow.Id, Created.Year.ToString("D4"), Id.ToString("D5"));
            }
        }

        private ActivityInstance InitialiseWithFirstActivity(WorkFlow workFlow, User currentUser)
        {
            var activity = workFlow.Activities.OrderBy(a => a.Id).First();
            var status = activity.ExecutionStatuses.Single(es => es.Description == "UNCONFIRMED");
            var activityInstance = new ActivityInstance(activity, currentUser)
            {
                QualifiedName = activity.QualifiedName,
                ReportInstance = this,
                CurrentStatus = status,
                Current = true
            };
            return activityInstance;
        }

        public void SetNewActivity(Activity activity, User currentUser)
        {

            var status = activity.ExecutionStatuses.OrderBy(es => es.Id).First();
            var activityInstance = new ActivityInstance(activity, currentUser)
            {
                QualifiedName = activity.QualifiedName,
                ReportInstance = this,
                CurrentStatus = status,
                Current = true
            };

            Activities.Add(activityInstance);
        }

        public ActivityInstance CurrentActivity
        {
            get
            {
                return Activities.Single(a => a.Current == true);
            }
        }

        [NotMapped]
        public string DisplayStatus
        {
            get
            {
                return String.Format("{0} | {1}", CurrentActivity.QualifiedName, CurrentActivity.CurrentStatus.Description);
            }
        }
    }
}