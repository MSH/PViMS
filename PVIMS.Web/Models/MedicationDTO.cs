using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class MedicationDTO
    {
        public int MedicationId { get; set; }
       
        public string DrugName { get; set; }

        public bool Active { get; set; }

        public int PackSize { get; set; }

        public string Strength { get; set; }

        public string CatalogNo { get; set; }
    }
}