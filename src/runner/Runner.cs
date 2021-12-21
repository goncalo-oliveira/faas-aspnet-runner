using System;
using System.Reflection;
using CommandLine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenFaaS.Hosting
{
    public static class Runner
    {
        public static void Run( string[] args, Type startupType ) =>
            RunParsed( args, options =>
                Run( WebApplication.CreateBuilder( args )
                    , options
                    , null
                    , null
                    , startupType ) );

        public static void Run( string[] args
            , Action<WebApplicationBuilder> builderAction = null
            , Action<WebApplication> appAction = null )
            =>
            RunParsed( args, options =>
                Run( WebApplication.CreateBuilder( args )
                    , options
                    , builderAction
                    , appAction
                    , null ) );

        private static void RunParsed( string[] args, Action<RunnerOptions> runAction )
        {
            var parsed = new Parser( settings =>
            {
                settings.IgnoreUnknownArguments = true;
            } )
            .ParseArguments<RunnerOptions>( args );

            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString();

            Console.WriteLine( $"OpenFaaS ASPNET Function Runner v{version}" );
            Console.WriteLine();

            parsed.WithParsed( options =>
                {
                    if ( options.Version )
                    {
                        return;
                    }

                    runAction( options );
                } );
        }

        private static void Run( WebApplicationBuilder builder
            , RunnerOptions options
            , Action<WebApplicationBuilder> builderAction
            , Action<WebApplication> appAction
            , Type startupType )
        {
            builder.Logging.AddFilter( "Microsoft.AspNetCore.DataProtection", LogLevel.Warning );
            builder.Logging.AddFilter( "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogLevel.Warning );

            if ( System.IO.File.Exists( options.Config ) )
            {
                Console.WriteLine( $"Using '{options.Config}' configuration file." );
            }

            builder.Configuration.SetBasePath( Environment.CurrentDirectory );
            builder.Configuration.AddJsonFile( options.Config, optional: true, reloadOnChange: false );
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddOpenFaaSSecrets();

            builder.WebHost.UseKestrel();
            builder.WebHost.UseUrls( $"http://*:{options.Port}" );

            builder.Services.AddCors( options =>
            {
                options.AddPolicy( "AllowAll", p => p
                       .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader() );
            } );

            builder.Services.AddRouting();
            builder.Services.AddControllers();

            // allow function implementation to customize the container
            builderAction?.Invoke( builder );

            // use function startup if it exists
            var startup = StartupWrapper.TryCreate( startupType, builder.Configuration );

            // lookup method Configure( WebApplicationBuilder )
            startup?.InvokeIfExists( "Configure", new object[]
            {
                builder
            } );

            // lookup method ConfigureServices( IServiceCollection )
            startup?.InvokeIfExists( "ConfigureServices", new object[]
            {
                builder.Services
            } );

            var app = builder.Build();

            if ( app.Environment.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // allow function implementation to customize the pipeline
            appAction?.Invoke( app );

            // lookup method Configure( WebApplicationBuilder )
            startup?.InvokeIfExists( "Configure", new object[]
            {
                app
            } );

            // lookup method Configure( IApplicationBuilder, bool )
            startup?.InvokeIfExists( "Configure", new object[]
            {
                (IApplicationBuilder)app,
                app.Environment.IsDevelopment()
            } );                

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors( "AllowAll" );

            //app.Run( $"http://*:{options.Port}" );
            app.Run();
        }
    }
}
