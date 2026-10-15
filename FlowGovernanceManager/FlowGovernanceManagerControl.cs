using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
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

        public FlowGovernanceManagerControl()
        {
            InitializeComponent();

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
            }
            else
            {
                ShowInfoNotification(
                    "Connect to an environment to load flows.",
                    null);
            }
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
                }
            }
        }
    }
}