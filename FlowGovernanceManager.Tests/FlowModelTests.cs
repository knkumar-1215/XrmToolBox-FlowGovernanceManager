using FlowGovernanceManager.Models;
using System;
using Xunit;

namespace FlowGovernanceManager.Tests
{
    public class FlowModelTests
    {
        [Fact]
        public void FlowModel_DefaultValues_AreExpected()
        {
            var flow = new FlowModel();
            Assert.Equal(Guid.Empty, flow.FlowId);
            Assert.Null(flow.FlowName);
            Assert.Equal(0, flow.HealthScore);
            Assert.Equal(0, flow.DaysSinceLastRun);
            Assert.Equal(0m, flow.ErrorRate);
        }

         
    }
}
