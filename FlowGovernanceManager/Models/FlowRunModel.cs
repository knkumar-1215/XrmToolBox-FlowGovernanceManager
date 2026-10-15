using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGovernanceManager.Models
{
    public class FlowRunModel
    {
        public Guid FlowId { get; set; }
        public int Status { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime CompletedOn { get; set; }
        public string ErrorMessage { get; set; }
    }
}
