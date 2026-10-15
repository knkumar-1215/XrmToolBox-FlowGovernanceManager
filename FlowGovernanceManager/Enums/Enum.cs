using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGovernanceManager.Enums
{
    public enum HealthStatus
    {
        Healthy = 0,
        AtRisk = 1,
        Critical = 2
    }

    public enum FlowStateCode
    {
        Draft = 0,
        Activated = 1
    }
}
