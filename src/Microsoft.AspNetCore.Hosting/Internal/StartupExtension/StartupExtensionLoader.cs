#if FEATURE_LOAD_CONTEXT
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.AspNetCore.Hosting
{
    internal class StartupExtensionLoader
    {
        private readonly StartupExtensionConfig _config;
        private readonly string _extensionDir;
        private ManagedLoadContext _context;

        public StartupExtensionLoader(string configPath)
        {
            _config = new StartupExtensionConfig(configPath);
            _extensionDir = Path.GetDirectoryName(configPath);

            var depsJsonFile = Path.Combine(_extensionDir, Path.GetFileNameWithoutExtension(_config.MainAssembly) + ".deps.json");

            var builder = new ManagedLoadContextBuilder();

            builder.PreferDefaultLoadContext(true);
            builder.TryAddDependencyContext(depsJsonFile);
            builder.SetBaseDirectory(_extensionDir);

            foreach (var ext in _config.PrivateAssemblies)
            {
                builder.PreferLoadContextAssembly(ext);
            }

            var runtimeConfigFile = Path.Combine(_extensionDir, Path.GetFileNameWithoutExtension(_config.MainAssembly) + ".runtimeconfig.json");

            builder.TryAddAdditionalProbingPathFromRuntimeConfig(runtimeConfigFile, includeDevConfig: true);

            _context = builder.Build();
        }

        public IReadOnlyList<IHostingStartup> GetStartups()
        {
            var assembly = _context.LoadFromAssemblyPath(Path.Combine(_extensionDir, _config.MainAssembly));
            var startup = new List<IHostingStartup>();
            foreach (var attr in assembly.GetCustomAttributes<HostingStartupAttribute>())
            {
                startup.Add((IHostingStartup)Activator.CreateInstance(attr.HostingStartupType));
            }
            return startup;
        }
    }
}
#endif
