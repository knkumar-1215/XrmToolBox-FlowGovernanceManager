using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGovernanceManager.Models
{
    public class OwnerModel
    {
        public Guid OwnerId { get; set; }
        public string FullName { get; set; }
        public bool IsDisabled { get; set; }
        public string InternalEmailAddress { get; set; }
    }
}
