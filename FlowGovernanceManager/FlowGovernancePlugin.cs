using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace FlowGovernanceManager
{
    /// <summary>
    /// Plugin registration class for Flow Governance Manager.
    /// Defines metadata displayed in the XrmToolBox tool list
    /// and creates the plugin control instance.
    /// Note: Add Base64 icon strings before publishing to store.
    /// Generate at: https://www.base64-image.de/
    /// </summary>
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Flow Governance Manager"),
        ExportMetadata("Description",
            "Identify and remediate unhealthy " +
            "Power Automate flows at scale"),
        ExportMetadata("SmallImageBase64", null), // 32x32 px — add before publishing
        ExportMetadata("BigImageBase64", null),   // 80x80 px — add before publishing
        ExportMetadata("BackgroundColor", "DodgerBlue"),
        ExportMetadata("PrimaryFontColor", "White"),
        ExportMetadata("SecondaryFontColor", "WhiteSmoke")]
    public class FlowGovernancePlugin : PluginBase
    {
        /// <summary>
        /// Creates and returns a new instance of the plugin control.
        /// Called by XrmToolBox each time the plugin tab is opened.
        /// </summary>
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new FlowGovernanceManagerControl();
        }

        public FlowGovernancePlugin()
        {
            // Uncomment to handle third party assembly load failures.
            // Required only if plugin uses external DLLs not merged via ILMerge.
            // AppDomain.CurrentDomain.AssemblyResolve +=
            //     new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Handles CLR assembly resolution failures.
        /// Looks for missing DLLs in a subfolder named after this plugin assembly.
        /// Only needed when using third party libraries not merged into the plugin DLL.
        /// </summary>
        private Assembly AssemblyResolveEventHandler(
            object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            List<AssemblyName> refAssemblies = currAssembly
                .GetReferencedAssemblies().ToList();

            var refAssembly = refAssemblies
                .FirstOrDefault(a => a.Name == argName);

            if (refAssembly != null)
            {
                string dir = Path.GetDirectoryName(
                    currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(
                    currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException(
                        $"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}