using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class Facility : EntityBase
	{
		public Facility()
		{
			PatientFacilities = new HashSet<PatientFacility>();
            UserFacilities = new HashSet<UserFacility>();
		}

		[Required]
		[StringLength(10)]
		public string FacilityCode { get; set; }

		[Required]
		[StringLength(100)]
		public string FacilityName { get; set; }

        [StringLength(30)]
        public string TelNumber { get; set; }

        [StringLength(30)]
        public string MobileNumber { get; set; }

        [StringLength(30)]
        public string FaxNumber { get; set; }

		public virtual FacilityType FacilityType { get; set; }
        
        public virtual OrgUnit OrgUnit { get; set; }

		public virtual ICollection<PatientFacility> PatientFacilities { get; set; }
        public virtual ICollection<UserFacility> UserFacilities { get; set; }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} ({1})", FacilityName, FacilityCode);
            }
        }
    }
}