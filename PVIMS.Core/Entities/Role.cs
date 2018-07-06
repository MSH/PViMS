using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class Role : Entity<int>
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        // Getting a false non-match with this regular expression. Need to investigate
        //[RegularExpression(@"^[A-Za-z][\w]", ErrorMessage = "{0} must start with an alphabetic character and may only contain letters, numbers and underscores (_).")]
        [StringLength(30, ErrorMessage = "{0} can be at most {1} characters long.")]
        public string Key { get; set; }
    }
}
