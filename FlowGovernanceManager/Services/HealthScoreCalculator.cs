using FlowGovernanceManager.Enums;
using FlowGovernanceManager.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FlowGovernanceManager.Services
{
    /// <summary>
    /// Calculates health scores for Power Automate flows.
    /// Score starts at 100 and deductions are applied
    /// based on flow health indicators.
    /// Minimum score is 0 — cannot go negative.
    /// </summary>
    public class HealthScoreCalculator
    {
        // Health score band thresholds
        private const int HealthyMinimum = 70;
        private const int AtRiskMinimum = 40;

        // Deduction values per issue
        private const int OwnerInactiveDeduction = 40;
        private const int ErrorRateHighDeduction = 35;
        private const int NoRun90DaysDeduction = 20;
        private const int ErrorRateMediumDeduction = 20;
        private const int NoRun30DaysDeduction = 10;
        private const int FlowOffDeduction = 10;

        // Thresholds
        private const int ErrorRateHighThreshold = 50;
        private const int ErrorRateMediumThreshold = 20;
        private const int InactiveDaysLong = 90;
        private const int InactiveDaysShort = 30;

        /// <summary>
        /// Calculates health score for a single flow.
        /// Combines workflow state, owner status,
        /// and run history into a 0-100 score.
        /// </summary>
        public int CalculateScore(FlowModel flow)
        {
            int score = 100;

            // Deduct for inactive owner
            if (!flow.IsOwnerActive)
                score -= OwnerInactiveDeduction;

            // Deduct for high error rate
            if (flow.ErrorRate > ErrorRateHighThreshold)
                score -= ErrorRateHighDeduction;
            else if (flow.ErrorRate > ErrorRateMediumThreshold)
                score -= ErrorRateMediumDeduction;

            // Deduct for no recent runs
            if (flow.DaysSinceLastRun > InactiveDaysLong)
                score -= NoRun90DaysDeduction;
            else if (flow.DaysSinceLastRun > InactiveDaysShort)
                score -= NoRun30DaysDeduction;

            // Deduct for flow being OFF
            if (!flow.IsFlowActive)
                score -= FlowOffDeduction;

            // Cannot go below 0
            return Math.Max(0, score);
        }

        /// <summary>
        /// Returns health status enum based on score.
        /// </summary>
        public HealthStatus GetHealthStatus(int score)
        {
            if (score >= HealthyMinimum)
                return HealthStatus.Healthy;
            if (score >= AtRiskMinimum)
                return HealthStatus.AtRisk;
            return HealthStatus.Critical;
        }

        /// <summary>
        /// Returns display text for health status.
        /// </summary>
        public string GetHealthStatusText(HealthStatus status)
        {
            switch (status)
            {
                case HealthStatus.Healthy: return "Healthy";
                case HealthStatus.AtRisk: return "At Risk";
                case HealthStatus.Critical: return "Critical";
                default: return "Unknown";
            }
        }
    }
}