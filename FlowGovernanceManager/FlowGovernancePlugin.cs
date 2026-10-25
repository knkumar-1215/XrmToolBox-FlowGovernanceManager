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
    /// </summary>
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Flow Governance Manager"),
        ExportMetadata("Description",
            "Identify and remediate unhealthy " +
            "Power Automate flows at scale"),
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAVZSURBVFhH7ZV9TFV1HMbvWkmEpZklLwrei1RqIiIpGtPUMcmUZtHyrYmWOjS1tFzWprbWSjNfW2vhSwGiwL3c9+t9hcuF+8K9F5R3DEGHgrwpkW3o1OdpZ/eP2smNq//0j5/t2fme83v2OdvZ75wjkTziEffhhS2Ij9yEWVFbMTPYCP3IbZgmdj0wsVn4WvYxBqQb0TM+C73jNuBakOmSfYT+CRvhi1jP0WJv0ExehyNT16Nv3FpErvgeMVnZmLh8L+KGyubjmJS2E5MmfQhP/Hqck+zm42J3UKSuQtisTLQlvocjJMN/rUD7SQe6j5Wg61jpfVKCrhwnek440ETyKcExMxNXZ2eyUOwOmtQVTEhbTSbMxxuKKmxQniNzysgc+3+TV04W+chsI9J+NmBBoRsHno9heOpK3Eldzj1id9AsycD2tzJwd8QIjjztQIGqkswxk7n/joXU+MiTRuz6oQCh+TZ0G2pIbSXWTZmCxIwVZHoGlondQbNqKazL34TzPBAmL0G7ykGeMQVy2kTq3WSBBUahW2BGic5NFllJoedoRlr6PKzJzCCXpSNJ7A6KbYs5OmsxBt9Pxmdl7ZihLyNVFlJuJIVZbUXHYQNCFAbstVaSCmMgdj8pt/A7wbF2IY5uWoKbqxcxXOwPim0LsXDnIjJjJKYY3fjS4Sb1ZtJYQhapME2lRbqtnNSZSLWBtDuFY+CpGGw4VH+J4Vteh2FHGuqmT+cTYn9Q7JqHg3vmoPeTZISabHDU1pAaPTYZlRhnNmPQaiUNetJeSppNuFpYyGFaLb6orSXNNljelXDYrrlo/Wo+FGJ3UOyW8LH9c9Dw7TTkOaoQX3IWvxgMCLEZ0Oi2kxYdaTeRZRbAqMVUswZznSVkqZH0VZB2O37cPZ3R+1Nwc18Kdoj9QXFkBqTHZ5OHZEwRzsu0UJwvJ+1a0qEna8pJmxpZpUqOdJ7FDY/ln7UmD9kKvLp/ArJ+molrYndQ5CZga14i/lopwTNOI7Y3lJEuLenUkI0OskKNU0LPpYW7rjRwXUiDnfRYoD+1jaPzE9D6WyKOit1DUvAKZhkTyNwRWFBdjdl1JrJaT1aqyCYr6dOiSeh5lDh8sYz0qslKdWCt+iwaDUBIcRxsynhU+R90I1pkHGF9GTcMUdjbTQ6v16On2UhWK8lGA1mvw6BTjhifAhltNrJGQ55Tkc1nyQYD+n1AhDkCB8wT0V/64kP8pNyxMFVEo1qYG7WouGIh64rJRjUpzNUFyGiQI6ZZg1struDaBS150UDWXEKyMwRrfHGkQ4p4sXtI3DJ+7h2Pe4USDr9gxIE+G3lBuEExKcxNchwUei1KNHUayWYF2aIkeyzkBR9Wl0qQXB9LVsqwVOweEpcMr9VPIG0SzLnsxTs3TOTlYrJVTgpzmwIuodcqR/6AJXC9TUH+aSUvWbFPJ2G4X4p7Hil2it1DUh7NZ70yDDifwzcDtxDXrcbd61qyU0H268kuJa7XZ2NUeyE237aS15SBtUEz2amD5qSET3qj0eGNRZ7YHRReGYyecagEENalRuMfOrKjCHe6inFnQIebV+SY16FBYp8a93qUQKcc925oyF4d/e1AqCeSdl8s/cIHTOweEk8sPvXKcLsiFJEAnu4uZMrlXCR2nEFiZz6Sfj+OSUKvV4WXBnSY3VmEpE4Vkvq0mFGbjTGVUTjhk6HPNRajxO6gcMtw9Hxg4zS5Y9DqikCdOzIQYXZFocEjRYtrLOorwlHtigjEGUG/PxqX/LHoKo/FZLE3aExjEFYlw9teKda6ZVgjjlOKtZ77xCfFB34pVpZGPMS7/ohH/J/8DTYXoxBe9+s8AAAAAElFTkSuQmCC"),
        ExportMetadata("BigImageBase64", "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"),
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