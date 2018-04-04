#if FEATURE_LOAD_CONTEXT
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Microsoft.AspNetCore.Hosting
{
    partial class ManagedLoadContext
    {
        [DebuggerDisplay("{Name} = {AdditionalProbingPath}")]
        public class ManagedLibrary
        {
            /// <summary>
            /// Name of the managed library
            /// </summary>
            public AssemblyName Name { get; private set; }

            /// <summary>
            /// Contains path to file within an additional probing path root.
            /// microsoft.data.sqlite/1.0.0/lib/netstandard1.3/Microsoft.Data.Sqlite.dll
            /// </summary>
            public string AdditionalProbingPath { get; private set; }

            public static ManagedLibrary Create(string name, string version, string assetPath)
            {
                return new ManagedLibrary
                {
                    Name = new AssemblyName(name),
                    AdditionalProbingPath = Path.Combine(name.ToLowerInvariant(), version, assetPath),
                };
            }
        }
    }
}
#endif
