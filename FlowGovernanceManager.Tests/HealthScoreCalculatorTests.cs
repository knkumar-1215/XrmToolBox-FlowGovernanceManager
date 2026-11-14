using FlowGovernanceManager.Models;
using FlowGovernanceManager.Services;
using FlowGovernanceManager.Enums;
using System;
using Xunit;

namespace FlowGovernanceManager.Tests
{
    public class HealthScoreCalculatorTests
    {
        private readonly HealthScoreCalculator _calculator;

        public HealthScoreCalculatorTests()
        {
            _calculator = new HealthScoreCalculator();
        }

        private FlowModel HealthyFlow()
        {
            return new FlowModel
            {
                IsOwnerActive = true,
                IsFlowActive = true,
                ErrorRate = 0,
                LastRunDate = DateTime.Now,
                DaysSinceLastRun = 0
            };
        }

        // --- Baseline -----

        [Fact]
        public void Score_AllHealthy_Returns100()
        {
            var flow = HealthyFlow();
            Assert.Equal(100, _calculator.CalculateScore(flow));
        }

        // --- Owner deductions ---

        [Fact]
        public void Score_OwnerInactive_Deducts40()
        {
            var flow = HealthyFlow();
            flow.IsOwnerActive = false;
            Assert.Equal(60, _calculator.CalculateScore(flow));
        }

        // --- Error rate deductions ---

        [Fact]
        public void Score_ErrorRateAbove50_Deducts35()
        {
            var flow = HealthyFlow();
            flow.ErrorRate = 51;
            Assert.Equal(65, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_ErrorRateExactly50_NoHighDeduction()
        {
            var flow = HealthyFlow();
            flow.ErrorRate = 50;
            Assert.Equal(80, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_ErrorRateAbove20_Deducts20()
        {
            var flow = HealthyFlow();
            flow.ErrorRate = 21;
            Assert.Equal(80, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_ErrorRateExactly20_NoDeduction()
        {
            var flow = HealthyFlow();
            flow.ErrorRate = 20;
            Assert.Equal(100, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_ErrorRateBelow20_NoDeduction()
        {
            var flow = HealthyFlow();
            flow.ErrorRate = 10;
            Assert.Equal(100, _calculator.CalculateScore(flow));
        }

        // ---- Run history deductions -----

        [Fact]
        public void Score_NeverRun_Deducts40()
        {
            var flow = HealthyFlow();
            flow.LastRunDate = DateTime.MinValue;
            Assert.Equal(60, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_NoRunIn90Days_Deducts20()
        {
            var flow = HealthyFlow();
            flow.LastRunDate = DateTime.Now.AddDays(-91);
            flow.DaysSinceLastRun = 91;
            Assert.Equal(80, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_NoRunIn30Days_Deducts10()
        {
            var flow = HealthyFlow();
            flow.LastRunDate = DateTime.Now.AddDays(-31);
            flow.DaysSinceLastRun = 31;
            Assert.Equal(90, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_RunWithin30Days_NoDeduction()
        {
            var flow = HealthyFlow();
            flow.LastRunDate = DateTime.Now.AddDays(-10);
            flow.DaysSinceLastRun = 10;
            Assert.Equal(100, _calculator.CalculateScore(flow));
        }

        // ---- Flow off deduction ----

        [Fact]
        public void Score_FlowOff_Deducts20()
        {
            var flow = HealthyFlow();
            flow.IsFlowActive = false;
            Assert.Equal(80, _calculator.CalculateScore(flow));
        }

        // ---- Combined scenarios ----

        [Fact]
        public void Score_FlowOff_NeverRun_Returns40()
        {
            var flow = HealthyFlow();
            flow.IsFlowActive = false;
            flow.LastRunDate = DateTime.MinValue;
            Assert.Equal(40, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_FlowOn_NeverRun_Returns60()
        {
            var flow = HealthyFlow();
            flow.LastRunDate = DateTime.MinValue;
            Assert.Equal(60, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_OwnerInactive_FlowOff_NeverRun_HighError_DoesNotGoBelowZero()
        {
            var flow = HealthyFlow();
            flow.IsOwnerActive = false;
            flow.IsFlowActive = false;
            flow.LastRunDate = DateTime.MinValue;
            flow.ErrorRate = 51;
            Assert.Equal(0, _calculator.CalculateScore(flow));
        }

        [Fact]
        public void Score_NeverGoesNegative()
        {
            var flow = new FlowModel
            {
                IsOwnerActive = false,
                IsFlowActive = false,
                ErrorRate = 100,
                LastRunDate = DateTime.MinValue,
                DaysSinceLastRun = 999
            };
            Assert.True(_calculator.CalculateScore(flow) >= 0);
        }

        // ---- GetHealthStatus ------

        [Fact]
        public void GetHealthStatus_Score100_ReturnsHealthy()
        {
            Assert.Equal(HealthStatus.Healthy, _calculator.GetHealthStatus(100));
        }

        [Fact]
        public void GetHealthStatus_Score70_ReturnsHealthy()
        {
            Assert.Equal(HealthStatus.Healthy, _calculator.GetHealthStatus(70));
        }

        [Fact]
        public void GetHealthStatus_Score69_ReturnsAtRisk()
        {
            Assert.Equal(HealthStatus.AtRisk, _calculator.GetHealthStatus(69));
        }

        [Fact]
        public void GetHealthStatus_Score40_ReturnsAtRisk()
        {
            Assert.Equal(HealthStatus.AtRisk, _calculator.GetHealthStatus(40));
        }

        [Fact]
        public void GetHealthStatus_Score39_ReturnsCritical()
        {
            Assert.Equal(HealthStatus.Critical, _calculator.GetHealthStatus(39));
        }

        [Fact]
        public void GetHealthStatus_Score0_ReturnsCritical()
        {
            Assert.Equal(HealthStatus.Critical, _calculator.GetHealthStatus(0));
        }

        // --- GetHealthStatusText ---

        [Fact]
        public void GetHealthStatusText_Healthy_ReturnsCorrectString()
        {
            Assert.Equal("Healthy", _calculator.GetHealthStatusText(HealthStatus.Healthy));
        }

        [Fact]
        public void GetHealthStatusText_AtRisk_ReturnsCorrectString()
        {
            Assert.Equal("At Risk", _calculator.GetHealthStatusText(HealthStatus.AtRisk));
        }

        [Fact]
        public void GetHealthStatusText_Critical_ReturnsCorrectString()
        {
            Assert.Equal("Critical", _calculator.GetHealthStatusText(HealthStatus.Critical));
        }
    }
}
