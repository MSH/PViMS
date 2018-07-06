using System.ComponentModel;

namespace PVIMS.Core.Entities
{
    public enum CausalityCriteria
    {
        [Description("Causality Set")]
        CausalitySet = 1,
        [Description("Causality Not Set")]
        CausalityNotSet = 2
    }
}
