using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace OpenFaaS
{
    public static class FunctionConfigurationExtensions
    {
        /// <summary>
        /// Retrieves an OpenFaaS.IFunctionBuilder to allow further customization
        /// </summary>
        /// <returns>An OpenFaaS.IFunctionBuilder instance</returns>
        public static IFunctionBuilder ConfigureFunction( this IServiceCollection services )
        {
            return new FunctionBuilder( services );
        }

        /// <summary>
        /// Configures Json serialization options
        /// </summary>
        /// <param name="configure">Callback to configure <see cref="JsonOptions"/></param>
        public static IFunctionBuilder ConfigureJsonOptions( this IFunctionBuilder builder, Action<JsonOptions> configure )
        {
            builder.Services.Configure<JsonOptions>( configure );

            return ( builder );
        }

        /// <summary>
        /// Configures MVC options
        /// </summary>
        /// <param name="configure">Callback to configure <see cref="MvcOptions"/></param>
        public static IFunctionBuilder ConfigureMvcOptions( this IFunctionBuilder builder, Action<MvcOptions> configure )
        {
            builder.Services.Configure<MvcOptions>( configure );

            return ( builder );
        }

        /// <summary>
        /// Configures Route options
        /// </summary>
        /// <param name="configure">Callback to configure <see cref="RouteOptions"/></param>
        public static IFunctionBuilder ConfigureRouteOptions( this IFunctionBuilder builder, Action<RouteOptions> configure )
        {
            builder.Services.Configure<RouteOptions>( configure );

            return ( builder );
        }
    }
}
