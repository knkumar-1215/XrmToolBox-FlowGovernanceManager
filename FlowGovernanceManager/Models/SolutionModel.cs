using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGovernanceManager.Models
{
    public class SolutionModel
    {
        public Guid SolutionId { get; set; }
        public string UniqueName { get; set; }
        public string FriendlyName { get; set; }
    }
}
