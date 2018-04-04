#if FEATURE_LOAD_CONTEXT

namespace Microsoft.AspNetCore.Hosting
{
    public class RuntimeOptions
    {
        public string Tfm { get; set; }

        public string[] AdditionalProbingPaths { get; set; }
    }
}
#endif
