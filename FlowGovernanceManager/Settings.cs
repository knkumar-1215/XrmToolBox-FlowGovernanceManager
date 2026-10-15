using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGovernanceManager
{
    /// <summary>
    /// This class can help you to store settings for your plugin
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    public class Settings
    {
        public string LastUsedOrganizationWebappUrl { get; set; }
        public int InactiveDaysThreshold { get; set; } = 90;
        public int ErrorRateThreshold { get; set; } = 20;
        public int HealthyScoreMinimum { get; set; } = 70;
        public int AtRiskScoreMinimum { get; set; } = 40;
        public string LastFilterStatus { get; set; } = "All";
    }
}