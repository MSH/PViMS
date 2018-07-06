using System;
using System.ComponentModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class Appointment : AuditedEntityBase
    {
        protected Appointment() { }

        public Appointment(Patient patient)
        {
            Patient = patient;
        }

        [Required]
        public virtual Patient Patient { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(250)]
        public string Reason { get; set; }

        public bool DNA { get; set; }
        public bool Cancelled { get; set; }

        [StringLength(250)]
        public string CancellationReason { get; set; }

        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }
        public int? AuditUser_Id { get; set; }

        public virtual User AuditUser { get; set; }

        public Encounter GetEncounter()
        {
            return Patient.Encounters.Where(e => (e.EncounterDate >= AppointmentDate.AddDays(1) && e.EncounterDate <= AppointmentDate.AddDays(5))).FirstOrDefault();
        }
    }
}
