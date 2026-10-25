using FlowGovernanceManager.Enums;
using FlowGovernanceManager.Models;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;

namespace FlowGovernanceManager.Services
{
    public class ExcelExporter
    {
        public string Export(List<FlowModel> flows, string filePath)
        {
            var rows = BuildRows(flows);

            MiniExcel.SaveAs(filePath, rows, overwriteFile: true);

            return filePath;
        }

        private List<Dictionary<string, object>> BuildRows(
            List<FlowModel> flows)
        {
            var rows = new List<Dictionary<string, object>>();

            foreach (var flow in flows)
            {
                var row = new Dictionary<string, object>
                {
                    { "Flow Name",         flow.FlowName },
                    { "Owner",             flow.OwnerName },
                    { "Health Score",      flow.HealthScore },
                    { "Health Status",     GetHealthStatusText(flow.HealthStatus) },
                    { "Last Run Date",     GetLastRunDateText(flow.LastRunDate) },
                    { "Error Rate %",      flow.ErrorRate },
                    { "Flow Status",       flow.IsFlowActive ? "On" : "Off" },
                    { "Solution",          flow.SolutionName ?? "" },
                    { "Days Since Last Run", flow.DaysSinceLastRun == 0 || flow.DaysSinceLastRun >= 999
                         ? "N/A"
                         : flow.DaysSinceLastRun.ToString() }
                };

                rows.Add(row);
            }

            return rows;
        }

        private string GetLastRunDateText(DateTime lastRunDate)
        {
            if (lastRunDate == DateTime.MinValue)
                return "Never";

            return lastRunDate.ToString("MM/dd/yyyy hh:mm tt");
        }

        private string GetHealthStatusText(HealthStatus status)
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