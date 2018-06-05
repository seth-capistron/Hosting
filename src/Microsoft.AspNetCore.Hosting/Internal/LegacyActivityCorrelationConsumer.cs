using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Hosting.Internal
{
    internal class LegacyActivityCorrelationConsumer : ICorrelationConsumer
    {
        private const string RequestIdHeaderName = "Request-Id";
        private const string CorrelationContextHeaderName = "Correlation-Context";

        public void BeginRequest( HttpContext httpContext, HostingApplication.Context context )
        {
            StringValues requestId;
            httpContext.Request.Headers.TryGetValue( RequestIdHeaderName, out requestId );

            if ( !StringValues.IsNullOrEmpty( requestId ) )
            {
                context.Activity.SetParentId( requestId );

                // We expect baggage to be empty by default
                // Only very advanced users will be using it in near future, we encourage them to keep baggage small (few items)
                string[] baggage = httpContext.Request.Headers.GetCommaSeparatedValues( CorrelationContextHeaderName );
                if ( baggage != StringValues.Empty )
                {
                    foreach ( var item in baggage )
                    {
                        if ( NameValueHeaderValue.TryParse( item, out var baggageItem ) )
                        {
                            context.Activity.AddBaggage( baggageItem.Name.ToString(), baggageItem.Value.ToString() );
                        }
                    }
                }
            }
        }
    }
}
