using OpenFaaS.Configuration;
using System;

namespace Microsoft.Extensions.Configuration
{
    internal static class SecretsServiceConfigurationExtensions
    {
        public static IConfigurationBuilder AddOpenFaaSSecrets( this IConfigurationBuilder configurationBuilder )
        {
            configurationBuilder.Add( new SecretsConfigurationSource() );

            return ( configurationBuilder );
        }
    }
}
