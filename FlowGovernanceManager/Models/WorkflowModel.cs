using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGovernanceManager.Models
{
    public class WorkflowModel
    {
        public Guid WorkflowId { get; set; }
        public string Name { get; set; }
        public int StateCode { get; set; }
        public int StatusCode { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid SolutionId { get; set; }
    }
}
