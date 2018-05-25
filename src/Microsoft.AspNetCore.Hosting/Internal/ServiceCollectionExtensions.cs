// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Hosting.Internal
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddCorrelationConsumer<T>( this IServiceCollection serviceCollection )
            where T : ICorrelationConsumer, new()
        {
            serviceCollection.TryAddSingleton<IList<ICorrelationConsumer>>(
                new List<ICorrelationConsumer>() );

            IList<ICorrelationConsumer> correlationConsumerList = serviceCollection.FirstOrDefault(
                descriptor =>
                    descriptor.ServiceType == typeof( IList<ICorrelationConsumer> ) )
                ?.ImplementationInstance as IList<ICorrelationConsumer>;

            if ( correlationConsumerList != null )
            {
                correlationConsumerList.Add( new T() );
            }
        }

        public static IServiceCollection Clone(this IServiceCollection serviceCollection)
        {
            IServiceCollection clone = new ServiceCollection();
            foreach (var service in serviceCollection)
            {
                clone.Add(service);
            }
            return clone;
        }
    }
}
