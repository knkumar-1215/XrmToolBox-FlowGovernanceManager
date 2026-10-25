using FlowGovernanceManager.Enums;
using FlowGovernanceManager.Models;
using FlowGovernanceManager.Services;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace FlowGovernanceManager
{
    /// <summary>
    /// Main control for Flow Governance Manager plugin.
    /// Handles UI interaction, connection management,
    /// and orchestrates flow data loading and display.
    /// </summary>
    public partial class FlowGovernanceManagerControl :
        PluginControlBase,
        IGitHubPlugin,
        IHelpPlugin,
        IAboutPlugin
    {
        private Settings mySettings;

        // UI Controls
        private ToolStripButton tsbLoadFlows;
        private ToolStripButton tsbExportExcel;
        private ToolStripButton tsbApplyFilter;
        private ToolStripSeparator tssSeparator2 = new ToolStripSeparator();
        private ToolStripSeparator tssSeparator3 = new ToolStripSeparator();
        private ToolStripLabel tslHealthStatus;
        private ToolStripComboBox cboHealthStatus;
        private ToolStripLabel tslFlowStatus;
        private ToolStripComboBox cboFlowStatus;

        private DataGridView dgvFlows;
        private TextBox txtStatistics;
        private Panel pnlStatistics;

        private List<FlowModel> _allFlows = new List<FlowModel>();

        public FlowGovernanceManagerControl()
        {
            InitializeComponent();
            InitializeUI();

            // Load saved settings or initialize with defaults
            if (!SettingsManager.Instance.TryLoad(
                typeof(FlowGovernanceManagerControl),
                out mySettings))
            {
                mySettings = new Settings();
                LogWarning("Settings not found — using defaults");
            }
            else
            {
                LogInfo("Settings loaded successfully");
            }
        }

        private void FlowGovernanceManagerControl_Load(
            object sender, EventArgs e)
        {
            // Show appropriate notification based on
            // whether XrmToolBox is already connected
            if (Service != null)
            {
                ShowInfoNotification(
                    "Click Load Flows to begin.", null);
                // Enable controls if already connected
                tsbLoadFlows.Enabled = true;
                tsbExportExcel.Enabled = true;
                tsbApplyFilter.Enabled = true;
                cboHealthStatus.Enabled = true;
                cboFlowStatus.Enabled = true;
            }
            else
            {
                ShowInfoNotification(
                    "Connect to an environment to load flows.",
                    null);
            }
        }

        private void InitializeUI()
        {
            // ---Toolbar Buttons------

            tsbLoadFlows = new ToolStripButton
            {
                Text = "Load Flows",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Enabled = false  // disabled until connected
            };
            tsbLoadFlows.Click += btnLoadFlows_Click;

            tsbExportExcel = new ToolStripButton
            {
                Text = "Export to Excel",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Enabled = false
            };
            tsbExportExcel.Click += btnExportExcel_Click;

            // ---- Dropdown filters --------

            tslHealthStatus = new ToolStripLabel
            {
                Text = "Health Status:"
            };

            cboHealthStatus = new ToolStripComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            cboHealthStatus.Items.AddRange(new object[]
            {
                "All", "Healthy", "At Risk", "Critical"
            });
            cboHealthStatus.SelectedIndex = 0;

            tslFlowStatus = new ToolStripLabel
            {
                Text = "Flow Status:"
            };

            cboFlowStatus = new ToolStripComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            cboFlowStatus.Items.AddRange(new object[]
            {
                "All Flows", "Active Only", "Inactive Only"
            });
            cboFlowStatus.SelectedIndex = 0;
            
            tsbApplyFilter = new ToolStripButton
            {
                Text = "Apply Filter",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Enabled = false
            };
            tsbApplyFilter.Click += btnApplyFilter_Click;
            // Add all to toolbar
            toolStripMenu.Items.AddRange(new ToolStripItem[]
            {
                tssSeparator2,
                tsbLoadFlows,
                tsbExportExcel,
                tsbApplyFilter,
                tssSeparator3,
                tslHealthStatus,
                cboHealthStatus,
                tslFlowStatus,
                cboFlowStatus
            });

            // ── Statistics panel (top right) ─────────────

            txtStatistics = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font(
                                  "Consolas", 9f),
                BackColor = System.Drawing.Color.WhiteSmoke,
                Text = "Load flows to see statistics."
            };

            pnlStatistics = new Panel
            {
                Width = 250,
                Dock = DockStyle.Right,
                Padding = new Padding(5)
            };
            pnlStatistics.Controls.Add(txtStatistics);

            //-----DataGridView ----------------

            dgvFlows = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                
            };
                dgvFlows.AlternatingRowsDefaultCellStyle.BackColor =
                System.Drawing.Color.FromArgb(245, 245, 245);

            // Columns
            dgvFlows.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name       = "colFlowName",
                    HeaderText = "Flow Name",
                    FillWeight = 25
                },
                new DataGridViewTextBoxColumn
                {
                    Name       = "colOwner",
                    HeaderText = "Owner",
                    FillWeight = 15
                },
                new DataGridViewTextBoxColumn
                {
                    Name       = "colHealthScore",
                    HeaderText = "Health Score",
                    FillWeight = 10
                },
                new DataGridViewTextBoxColumn
                {
                    Name       = "colHealthStatus",
                    HeaderText = "Status",
                    FillWeight = 10
                },
                new DataGridViewTextBoxColumn
                {
                    Name       = "colLastRunDate",
                    HeaderText = "Last Run Date",
                    FillWeight = 15
                },
                new DataGridViewTextBoxColumn
                {
                    Name       = "colErrorRate",
                    HeaderText = "Error Rate %",
                    FillWeight = 10
                },
                new DataGridViewTextBoxColumn
                {
                    Name       = "colFlowStatus",
                    HeaderText = "Flow Status",
                    FillWeight = 8
                },
                new DataGridViewTextBoxColumn
                {
                    Name       = "colSolution",
                    HeaderText = "Solution",
                    FillWeight = 15
                }
            });

            dgvFlows.ColumnHeadersVisible = true;
            dgvFlows.ColumnHeadersHeight = 30;
            dgvFlows.ColumnHeadersDefaultCellStyle.BackColor =
                System.Drawing.Color.FromArgb(51, 51, 51);
            dgvFlows.ColumnHeadersDefaultCellStyle.ForeColor =
                System.Drawing.Color.White;
            dgvFlows.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 9f,
                    System.Drawing.FontStyle.Bold);
            dgvFlows.EnableHeadersVisualStyles = false;

            // Create a panel to hold the grid
            var pnlGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 35, 0, 0) // top padding
            };
            pnlGrid.Controls.Add(dgvFlows);

            //-- Add Controls to the Form -----
            // TO:
            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlStatistics);

        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (_allFlows == null || _allFlows.Count == 0)
            {
                ShowWarningNotification("No flows loaded. Click Load Flows first.", null);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Export Flow Governance Report";
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.FileName = string.Format(
                    "FlowGovernanceReport_{0}.xlsx",
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"));

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    tsbExportExcel.Enabled = false;

                    var exporter = new ExcelExporter();
                    exporter.Export(_allFlows, saveDialog.FileName);

                    ShowInfoNotification(
                        string.Format("Report exported to {0}", saveDialog.FileName),
                        null);

                    // Open the file in Excel after export
                    System.Diagnostics.Process.Start(saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    ShowErrorDialog(ex, "Export to Excel failed");
                }
                finally
                {
                    tsbExportExcel.Enabled = true;
                }
            }
        }

        private void btnLoadFlows_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadFlows);
        }

        private void LoadFlows()
        {
            var calculator = new HealthScoreCalculator();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Power Automate flows...",
                Work = (worker, args) =>
                {
                    var service = new FlowDataService(Service);

                    // Step 1 — Get all cloud flows
                    worker.ReportProgress(0,
                        "Querying flows...");
                    var workflows = service.GetAllFlows();

                    // Step 2 — Get owners for these flows
                    worker.ReportProgress(33,
                        "Loading owner details...");
                    var ownerIds = workflows
                        .Select(w => w.OwnerId)
                        .Distinct()
                        .ToList();
                    var owners = service.GetOwners(ownerIds);

                    // Step 3 — Get flow run history
                    worker.ReportProgress(66,
                        "Loading run history...");
                    var flowIds = workflows
                        .Select(w => w.WorkflowId)
                        .ToList();
                    var flowRuns = service.GetFlowRuns(flowIds);

                    // Step 4 — Get solution names
                    worker.ReportProgress(80,
                        "Loading solution names...");
                    var solutionNames = service.GetSolutionNames(flowIds);

                    // Step 5 — Build combined FlowModel list

                    worker.ReportProgress(90,
                        "Calculating health scores...");
                    args.Result = service.BuildFlowModels(
                        workflows, owners, flowRuns, calculator, solutionNames);
                },
                ProgressChanged = e =>
                {
                    // Update loading message as each step completes
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorDialog(args.Error);
                        LogError("Failed to load flows: {0}",
                            args.Error.Message);
                        return;
                    }

                    _allFlows = args.Result as List<FlowModel>;
                    LogInfo("Loaded {0} flows", _allFlows.Count);

                    PopulateGrid(_allFlows);
                    UpdateStatistics(_allFlows);

                    // Toast notification
                    try
                    {
                        var criticalCount = _allFlows.Count(
                            f => f.HealthStatus == HealthStatus.Critical);

                        if (criticalCount > 0)
                        {
                            ShowWarningNotification(
                                $"{_allFlows.Count} flows loaded. " +
                                $"{criticalCount} need attention.", null);
                        }
                        else
                        {
                            ShowInfoNotification(
                                $"{_allFlows.Count} flows loaded successfully.",
                                null);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWarning("Notification failed: {0}", ex.Message);
                    }
                }
            });
        }

        /// <summary>
        /// Filters _allFlows based on dropdown selections.
        /// Always filters from _allFlows (full list)
        /// never from already filtered results.
        /// Statistics panel always shows ALL flows totals.
        /// </summary>
        private void ApplyFilters()
        {
            if (_allFlows == null || _allFlows.Count == 0)
                return;

            var filtered = _allFlows.AsEnumerable();

            // Health Status dropdown filter
            var healthFilter = cboHealthStatus
                .SelectedItem?.ToString();

            if (healthFilter != "All")
            {
                filtered = filtered.Where(f =>
                {
                    switch (healthFilter)
                    {
                        case "Healthy":
                            return f.HealthStatus ==
                                   HealthStatus.Healthy;
                        case "At Risk":
                            return f.HealthStatus ==
                                   HealthStatus.AtRisk;
                        case "Critical":
                            return f.HealthStatus ==
                                   HealthStatus.Critical;
                        default:
                            return true;
                    }
                });
            }

            // Flow Status dropdown filter
            var flowFilter = cboFlowStatus
                .SelectedItem?.ToString();

            if (flowFilter != "All Flows")
            {
                filtered = filtered.Where(f =>
                    flowFilter == "Active Only"
                        ? f.IsFlowActive
                        : !f.IsFlowActive);
            }

            // Populate grid with filtered results
            PopulateGrid(filtered.ToList());

            // Statistics always shows ALL flows
            UpdateStatistics(_allFlows);
        }

        /// <summary>
        /// Populates the grid with FlowModel data.
        /// Applies color coding based on health status.
        /// </summary>
        private void PopulateGrid(List<FlowModel> flows)
        {
            dgvFlows.Rows.Clear();

            foreach (var flow in flows)
            {
                int rowIndex = dgvFlows.Rows.Add(
                    flow.FlowName,
                    flow.OwnerName,
                    flow.HealthScore,
                    flow.HealthStatus == HealthStatus.Healthy
                        ? "Healthy"
                        : flow.HealthStatus == HealthStatus.AtRisk
                            ? "At Risk"
                            : "Critical",
                    flow.LastRunDate == DateTime.MinValue
                        ? "Never"
                        : flow.LastRunDate
                            .ToLocalTime()
                            .ToString("MM/dd/yyyy HH:mm"),
                    $"{flow.ErrorRate}%",
                    flow.IsFlowActive ? "On" : "Off",
                    flow.SolutionName);

                // Color code the row
                var row = dgvFlows.Rows[rowIndex];
                switch (flow.HealthStatus)
                {
                    case HealthStatus.Healthy:
                        row.DefaultCellStyle.BackColor =
                            System.Drawing.Color.FromArgb(
                                198, 239, 206); // light green
                        row.DefaultCellStyle.ForeColor =
                            System.Drawing.Color.FromArgb(
                                0, 97, 0);      // dark green
                        break;

                    case HealthStatus.AtRisk:
                        row.DefaultCellStyle.BackColor =
                            System.Drawing.Color.FromArgb(
                                255, 235, 156); // light yellow
                        row.DefaultCellStyle.ForeColor =
                            System.Drawing.Color.FromArgb(
                                156, 87, 0);    // dark orange
                        break;

                    case HealthStatus.Critical:
                        row.DefaultCellStyle.BackColor =
                            System.Drawing.Color.FromArgb(
                                255, 199, 206); // light red
                        row.DefaultCellStyle.ForeColor =
                            System.Drawing.Color.FromArgb(
                                156, 0, 6);     // dark red
                        break;
                }
            }
            // Clear automatic selection after populating
            dgvFlows.ClearSelection();
        }

        

        /// <summary>
        /// Updates the statistics panel with flow counts.
        /// Always calculated from ALL flows regardless
        /// of current filter selection.
        /// </summary>
        private void UpdateStatistics(List<FlowModel> flows)
        {
            int total = flows.Count;
            int healthy = flows.Count(
                f => f.HealthStatus == HealthStatus.Healthy);
            int atRisk = flows.Count(
                f => f.HealthStatus == HealthStatus.AtRisk);
            int critical = flows.Count(
                f => f.HealthStatus == HealthStatus.Critical);
            int ownerless = flows.Count(
                f => !f.IsOwnerActive);
            int inactive = flows.Count(
                f => f.DaysSinceLastRun > 90);

            txtStatistics.Text =
                $"Total Flows:      {total}\r\n" +
                $"Healthy:          {healthy} " +
                    $"({Percent(healthy, total)}%)\r\n" +
                $"At Risk:          {atRisk} " +
                    $"({Percent(atRisk, total)}%)\r\n" +
                $"Critical:         {critical} " +
                    $"({Percent(critical, total)}%)\r\n" +
                $"─────────────────────\r\n" +
                $"Ownerless:        {ownerless}\r\n" +
                $"Inactive 90+ days:{inactive}";
        }

        private int Percent(int part, int total)
        {
            return total == 0
                ? 0
                : (int)Math.Round((double)part / total * 100);
        }



        #region IGitHubPlugin
        // Links plugin to GitHub repo — adds GitHub
        // Issues menu so users can report bugs directly
        public string RepositoryName =>
            "XrmToolBox-FlowGovernanceManager";
        public string UserName => "knkumar-1215";
        #endregion

        #region IHelpPlugin
        // Adds Help menu linking to GitHub documentation
        public string HelpUrl =>
            "https://github.com/knkumar-1215/" +
            "XrmToolBox-FlowGovernanceManager";
        #endregion

        #region IAboutPlugin
        // Displays plugin information dialog
        public void ShowAboutDialog()
        {
            MessageBox.Show(
                "Flow Governance Manager v1.0\n" +
                "by Nagendra Kumar Kosaraju\n\n" +
                "Identify and remediate unhealthy\n" +
                "Power Automate flows at scale.\n\n" +
                "github.com/knkumar-1215",
                "About Flow Governance Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        #endregion

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void FlowGovernanceManagerControl_OnCloseTool(
            object sender, EventArgs e)
        {
            // Save settings when plugin is closed
            SettingsManager.Instance.Save(
                typeof(FlowGovernanceManagerControl),
                mySettings);
        }

        /// <summary>
        /// Fires when user connects or switches environment
        /// in XrmToolBox. base.UpdateConnection() must be
        /// called first — it sets the Service property.
        /// </summary>
        public override void UpdateConnection(
            IOrganizationService newService,
            ConnectionDetail detail,
            string actionName,
            object parameter)
        {
            // Must call base first — sets this.Service
            base.UpdateConnection(
                newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                // Save last used environment URL
                mySettings.LastUsedOrganizationWebappUrl =
                    detail.WebApplicationUrl;

                LogInfo("Connected to: {0}",
                    detail.OrganizationFriendlyName);

                // IsHandleCreated check prevents
                // notification errors during first load
                // when control is not yet fully rendered
                if (IsHandleCreated)
                {
                    HideNotification();
                    ShowInfoNotification(
                        $"Connected to " +
                        $"{detail.OrganizationFriendlyName}. " +
                        "Click Load Flows to begin.", null);

                    // Enable buttons after connection
                    if (tsbLoadFlows != null)
                    {
                        tsbLoadFlows.Enabled = true;
                        tsbExportExcel.Enabled = true;
                        tsbApplyFilter.Enabled = true;
                        cboHealthStatus.Enabled = true;
                        cboFlowStatus.Enabled = true;
                    }
                }
            }
        }
    }
}