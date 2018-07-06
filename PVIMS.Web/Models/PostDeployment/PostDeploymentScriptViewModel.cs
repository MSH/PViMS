using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PostDeploymentScriptViewModel
    {
        public int Id { get; set; } 
        public Guid ScriptGuid { get; set; }
        public string ScriptFileName { get; set; }
        public string ScriptDescription { get; set; }
        public DateTime? RunDate { get; set; }
        public int? StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public int RunRank { get; set; } 
    }
}