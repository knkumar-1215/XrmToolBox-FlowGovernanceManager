using FlowGovernanceManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGovernanceManager.Models
{
    public  class FlowModel
    {
        public Guid FlowId { get; set; }

        public string FlowName { get; set; }
        public string OwnerName { get; set; }
        public bool IsOwnerActive { get; set; }
        public int HealthScore { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public DateTime LastRunDate { get; set; }
        public decimal ErrorRate { get; set; }
        public bool IsFlowActive { get; set; }
        public string SolutionName { get; set; }
        public int TotalRunsInLast30Days { get; set; }
        public int FailedRunsInLast30Days { get; set; }
        public int DaysSinceLastRun { get; set; }
    }
}
