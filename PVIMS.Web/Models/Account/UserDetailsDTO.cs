using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class UserDetailsDTO
    {
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Facilities { get; set; }
    }
}