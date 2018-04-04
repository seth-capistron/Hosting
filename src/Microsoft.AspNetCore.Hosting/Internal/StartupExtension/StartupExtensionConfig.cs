#if FEATURE_LOAD_CONTEXT
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.AspNetCore.Hosting
{
    internal sealed class StartupExtensionConfig
    {
        public static bool TryLoad(string configFile, out StartupExtensionConfig config)
        {
            try
            {
                config = new StartupExtensionConfig(configFile);
                return true;
            }
            catch
            {
                config = null;
                return false;
            }
        }

        public StartupExtensionConfig(string configFile)
        {
            var privateDeps = new HashSet<AssemblyName>();
            PrivateAssemblies = privateDeps;
            var doc = XDocument.Load(configFile, LoadOptions.SetLineInfo);

            if (doc.Root.Name != "StartupExtension")
            {
                throw new InvalidDataException("Root element should be 'StartupExtension'");
            }

            var mainAssembly = doc.Root.Attribute("MainAssembly");
            if (mainAssembly == null || string.IsNullOrEmpty(mainAssembly.Value))
            {
                IXmlLineInfo line = doc.Root;
                throw new InvalidDataException($"Missing required attribute 'MainAssembly' for StartupExtension on line {line.LineNumber}");
            }

            MainAssembly = mainAssembly.Value;

            foreach (var dep in doc.Root.Descendants("PrivateDependency"))
            {
                var identity = dep.Attribute("Identity");
                if (identity == null || string.IsNullOrEmpty(identity.Value))
                {
                    IXmlLineInfo line = dep;
                    throw new InvalidDataException($"Missing required attribute 'Identity' for PrivateDependency on line {line.LineNumber}");
                }

                privateDeps.Add(new AssemblyName(identity.Value));
            }
        }

        public IReadOnlyCollection<AssemblyName> PrivateAssemblies { get; }

        public string MainAssembly { get; }
    }
}
#endif