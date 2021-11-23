using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OpenFaaS.Hosting
{
    internal static class RunnerServiceExtensions
    {
        private static object functionStartup;

        public static IServiceCollection AddFunction( this IServiceCollection services, IConfiguration configuration )
        {
            functionStartup = Assembly.GetEntryAssembly().CreateInstance( "OpenFaaS.Startup", false, BindingFlags.CreateInstance, null, new object[]
            {
                configuration
            }, null, null );

            return ( services );
        }

        public static IMvcBuilder AddFunctionControllers( this IMvcBuilder builder )
        {
            return builder.AddApplicationPart( functionStartup.GetType().Assembly );
        }

        public static IServiceCollection ConfigureFunctionServices( this IServiceCollection services )
        {
            var configureServicesMethod = functionStartup.GetType().GetMethod( "ConfigureServices", BindingFlags.Public | BindingFlags.Instance, null, new Type[]
            {
                typeof( IServiceCollection )
            }, null );

            configureServicesMethod?.Invoke( functionStartup, new object[]
            {
                services
            } );

            return ( services );
        }

        public static IApplicationBuilder ConfigureFunction( this IApplicationBuilder app, IWebHostEnvironment env )
        {
            var configureMethod = functionStartup.GetType().GetMethod( "Configure", BindingFlags.Public | BindingFlags.Instance, null, new Type[]
            {
                typeof( IApplicationBuilder ), typeof( bool )
            }, null );

            // the Configure( IApplicationBuilder, bool ) method is optional
            configureMethod?.Invoke( functionStartup, new object[]
            { 
                app, env.IsDevelopment()
            } );

            return ( app );
        }
    }
}
