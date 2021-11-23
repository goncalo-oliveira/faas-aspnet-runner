using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo( "function" )]

namespace OpenFaaS.Hosting
{
    internal static class Runner
    {
        internal static void Run( string[] args )
        {
            var parsed = Parser.Default.ParseArguments<Options>( args );

            var version = typeof( Hosting.Startup ).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToString();

            Console.WriteLine( $"OpenFaaS ASPNET Function Runner v{version}" );
            Console.WriteLine();

            parsed.WithParsed( options =>
            {
                if ( options.Version )
                {
                    return;
                }

                CreateHostBuilder( args, options )
                    .Build()
                    .Run();
            } );
        }

        private static IHostBuilder CreateHostBuilder( string[] args, Options options ) =>
            Host.CreateDefaultBuilder( args )
                .ConfigureLogging( logging =>
                {
                    logging.AddFilter( "Microsoft.AspNetCore.DataProtection", LogLevel.Warning )
                        .AddFilter( "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogLevel.Warning );
                } )
                .ConfigureWebHostDefaults( webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration( ( context, configBuilder ) =>
                    {
                        if ( System.IO.File.Exists( options.Config ) )
                        {
                            Console.WriteLine( $"Using '{options.Config}' configuration file." );
                        }

                        configBuilder.SetBasePath( Environment.CurrentDirectory );
                        configBuilder.AddJsonFile( options.Config, optional: true, reloadOnChange: false );
                        configBuilder.AddEnvironmentVariables();
                        configBuilder.AddOpenFaaSSecrets();
                        configBuilder.AddInMemoryCollection( new Dictionary<string, string>
                        {
                            { "Args:SkipAuth", options.NoAuth.ToString() }
                        } );
                    } );

                    webBuilder.UseKestrel();
                    webBuilder.UseStartup<Hosting.Startup>();
                    webBuilder.UseUrls( $"http://*:{options.Port}" );
                } );
    }
}
