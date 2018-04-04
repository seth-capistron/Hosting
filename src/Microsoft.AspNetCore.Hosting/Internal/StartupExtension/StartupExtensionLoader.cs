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

        private static readonly Version _currentHostingVersion = typeof(IHostingStartup).Assembly.GetName().Version;

        public StartupExtensionLoader(string configPath)
        {
            using (var reader = File.OpenText(configPath))
            {
                _config = new StartupExtensionConfig(reader);
            }

            _extensionDir = Path.GetDirectoryName(configPath);
        }

        internal StartupExtensionLoader(StartupExtensionConfig config, string extPath)
        {
            _config = config;
            _extensionDir = extPath;
        }

        public IReadOnlyList<IHostingStartup> GetStartups()
        {
            if (_config.MinHostingVersion > _currentHostingVersion)
            {
                return Array.Empty<IHostingStartup>();
            }

            EnsureInitialized();

            var assembly = _context.LoadFromAssemblyPath(Path.Combine(_extensionDir, _config.MainAssembly));
            var startup = new List<IHostingStartup>();
            foreach (var attr in assembly.GetCustomAttributes<HostingStartupAttribute>())
            {
                startup.Add((IHostingStartup)Activator.CreateInstance(attr.HostingStartupType));
            }
            return startup;
        }

        private void EnsureInitialized()
        {
            if (_context != null)
            {
                return;
            }

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
    }
}
#endif
