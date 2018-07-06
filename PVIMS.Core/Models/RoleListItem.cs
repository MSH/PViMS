using System.ComponentModel;

namespace PVIMS.Core.Models
{
    public class RoleListItem
    {
        public long RoleId { get; set; }
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
        [DisplayName("Role Key")]
        public string RoleKey { get; set; }
    }
}
