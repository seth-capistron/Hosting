#if FEATURE_LOAD_CONTEXT
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.AspNetCore.Hosting
{
    internal sealed class StartupExtensionConfig
    {
        public StartupExtensionConfig(TextReader configFile)
        {
            var privateDeps = new HashSet<AssemblyName>();
            PrivateAssemblies = privateDeps;
            var doc = XDocument.Load(configFile, LoadOptions.SetLineInfo);

            if (doc.Root.Name != "StartupExtension")
            {
                throw new InvalidDataException("Root element should be 'StartupExtension'");
            }

            var mainAssemblyAttr = doc.Root.Attribute("MainAssembly");
            if (mainAssemblyAttr == null || string.IsNullOrEmpty(mainAssemblyAttr.Value))
            {
                IXmlLineInfo line = doc.Root;
                throw new InvalidDataException($"Missing required attribute 'MainAssembly' for StartupExtension on line {line.LineNumber}");
            }

            var hostingVersionAttr = doc.Root.Attribute("MinHostingVersion");
            Version hostingVersion = null;
            if (hostingVersionAttr != null && !Version.TryParse(hostingVersionAttr.Value, out hostingVersion))
            {
                IXmlLineInfo line = hostingVersionAttr;
                throw new InvalidDataException($"Value for MinHostingVersion '{hostingVersionAttr.Value}' is not a valid assembly version on line {line.LineNumber}");
            }

            MinHostingVersion = hostingVersion;
            MainAssembly = mainAssemblyAttr.Value;

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
        public Version MinHostingVersion { get; private set; }
        public string MainAssembly { get; }
    }
}
#endif