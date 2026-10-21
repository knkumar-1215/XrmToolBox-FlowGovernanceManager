using FlowGovernanceManager.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowGovernanceManager.Services
{
    /// <summary>
    /// Handles all Dataverse queries for the plugin.
    /// All methods use late bound entities only.
    /// Never call these methods on the UI thread —
    /// always use WorkAsync.
    /// </summary>
    public class FlowDataService
    {
        private readonly IOrganizationService _service;

        public FlowDataService(IOrganizationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all Power Automate cloud flows
        /// from the connected environment.
        /// category = 5 filters to cloud flows only.
        /// </summary>

        public List<WorkflowModel> GetAllFlows()
        {
            var query = new QueryExpression("workflow")
            {
                ColumnSet = new ColumnSet(
                    "workflowid",
                    "name",
                    "statecode",
                    "statuscode",
                    "ownerid",
                    "createdon",
                    "modifiedon"),
                NoLock = true
            };

            // category = 5 is for cloud flow
            query.Criteria.AddCondition(
                "category",
                ConditionOperator.Equal, 5);

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Select(e =>
                new WorkflowModel
                {
                    WorkflowId = e.Id,
                    Name = e.GetAttributeValue<string>("name"),
                    StateCode = e.GetAttributeValue<OptionSetValue>(
                                     "statecode")?.Value ?? 0,
                    StatusCode = e.GetAttributeValue<OptionSetValue>(
                                     "statuscode")?.Value ?? 0,
                    OwnerId = e.GetAttributeValue<EntityReference>(
                                     "ownerid")?.Id ?? Guid.Empty,
                    CreatedOn = e.GetAttributeValue<DateTime>("createdon"),
                    ModifiedOn = e.GetAttributeValue<DateTime>("modifiedon")
                }).ToList();
        }

        /// <summary>
        /// Retrieves system users for the given owner IDs.
        /// Used to get owner names and check if they are
        /// still active in the organization.
        /// </summary>
        public List<OwnerModel> GetOwners(List<Guid> ownerIds)
        {
            if (ownerIds == null || ownerIds.Count == 0)
                return new List<OwnerModel>();

            var query = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet(
                    "systemuserid",
                    "fullname",
                    "isdisabled",
                    "internalemailaddress"),
                NoLock = true
            };

            // Filter to only the owners we need
            query.Criteria.AddCondition(
                "systemuserid",
                ConditionOperator.In,
                ownerIds.Cast<object>().ToArray());

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Select(e =>
                new OwnerModel
                {
                    OwnerId = e.Id,
                    FullName = e.GetAttributeValue<string>(
                        "fullname") ?? "Unknown",
                    IsDisabled = e.GetAttributeValue<bool>(
                        "isdisabled"),
                    InternalEmailAddress =
                        e.GetAttributeValue<string>(
                            "internalemailaddress") ?? ""
                }).ToList();
        }

        /// <summary>
        /// Retrieves flow run history for the last 30 days
        /// for all provided flow IDs.
        /// Used to calculate error rate and last run date.
        /// Single query for ALL flows — never query per flow.
        /// </summary>
        public List<FlowRunModel> GetFlowRuns(
            List<Guid> flowIds)
        {
            if (flowIds == null || flowIds.Count == 0)
                return new List<FlowRunModel>();

            var fromDate = DateTime.UtcNow.AddDays(-30);

            var query = new QueryExpression("flowrun")
            {
                ColumnSet = new ColumnSet(
            "flowrunid",
            "workflow",      
            "status",
            "starttime",     
            "endtime",       
            "errormessage"),
                NoLock = true
            };

            // Only last 30 days
            query.Criteria.AddCondition(
                "starttime",
                ConditionOperator.GreaterEqual,
                fromDate);

            var result = _service.RetrieveMultiple(query);

            return result.Entities.Select(e =>
             new FlowRunModel
             {
                 FlowId = e.GetAttributeValue<EntityReference>(
                     "workflow")?.Id ?? Guid.Empty,
                 Status = GetFlowRunStatus(
                             e.GetAttributeValue<string>("status")),
                 StartedOn = e.GetAttributeValue<DateTime>(
                     "starttime"),    
                 CompletedOn = e.GetAttributeValue<DateTime>(
                     "endtime"),     
                 ErrorMessage = e.GetAttributeValue<string>(
                     "errormessage") ?? ""
             }).ToList();
        }


        /// <summary>
        /// Combines workflow, owner and run history data
        /// into a list of FlowModel objects ready for display.
        /// All data joining happens in memory — never per-flow queries.
        /// </summary>
        public List<FlowModel> BuildFlowModels(
            List<WorkflowModel> workflows,
            List<OwnerModel> owners,
            List<FlowRunModel> flowRuns,
            HealthScoreCalculator calculator)
        {
            var result = new List<FlowModel>();

            foreach (var workflow in workflows)
            {
                // Find owner for this flow
                var owner = owners.FirstOrDefault(
                    o => o.OwnerId == workflow.OwnerId);

                // Get all runs for this flow
                var runs = flowRuns.Where(
                    r => r.FlowId == workflow.WorkflowId)
                    .ToList();

                // Calculate metrics
                int totalRuns = runs.Count;
                int failedRuns = runs.Count(
                    r => r.Status == 2); // 2 = Failed

                decimal errorRate = totalRuns > 0
                    ? Math.Round(
                        (decimal)failedRuns / totalRuns * 100, 1)
                    : 0;

                // Last successful run
                var lastRun = runs
                    .Where(r => r.Status == 1) // 1 = Success
                    .OrderByDescending(r => r.StartedOn)
                    .FirstOrDefault();

                DateTime lastRunDate = lastRun?.StartedOn
                    ?? DateTime.MinValue;

                int daysSinceLastRun = lastRunDate == DateTime.MinValue
                    ? 999  // never run
                    : (int)(DateTime.UtcNow - lastRunDate).TotalDays;

                // Build FlowModel
                var flowModel = new FlowModel
                {
                    FlowId = workflow.WorkflowId,
                    FlowName = workflow.Name,
                    OwnerName = owner?.FullName ?? "Unknown",
                    IsOwnerActive = owner != null
                                          && !owner.IsDisabled,
                    IsFlowActive = workflow.StateCode == 1,
                    LastRunDate = lastRunDate,
                    TotalRunsInLast30Days = totalRuns,
                    FailedRunsInLast30Days = failedRuns,
                    ErrorRate = errorRate,
                    DaysSinceLastRun = daysSinceLastRun,
                    SolutionName = "" 
                };

                // Calculate health score
                flowModel.HealthScore =
                    calculator.CalculateScore(flowModel);
                flowModel.HealthStatus =
                    calculator.GetHealthStatus(flowModel.HealthScore);

                result.Add(flowModel);
            }

            // Sort by health score ascending
            // Critical flows appear first
            return result.OrderBy(f => f.HealthScore).ToList();
        }

        /// <summary>
        /// Converts flowrun status string to integer.
        /// flowrun.status is a string not OptionSetValue.
        /// 0=Running, 1=Succeeded, 2=Failed, 3=Cancelled
        /// </summary>
        private int GetFlowRunStatus(string status)
        {
            switch (status?.ToLower())
            {
                case "succeeded": return 1;
                case "failed": return 2;
                case "cancelled": return 3;
                case "running": return 0;
                default: return 0;
            }
        }
    }
}
