using System;
using System.Collections.Generic;

namespace PVIMS.Web.Models
{
    public class DeploymentSummaryViewModel
    {
        public IEnumerable<String> CreatedTables { get; set; }
        public bool IsFirstRun { get; set; }
        public IEnumerable<PostDeploymentScriptViewModel> PendingPostDeploymentScripts { get; set; } 
        public IEnumerable<PostDeploymentScriptViewModel> ExecutedPostDeploymentScripts { get; set; } 
    }
}