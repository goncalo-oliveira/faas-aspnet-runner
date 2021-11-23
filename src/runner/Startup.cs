using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OpenFaaS.Hosting
{
    internal class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices( IServiceCollection services )
        {
            services.AddCors( options =>
            {
                options.AddPolicy( "AllowAll", p => p
                       .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader() );
            } );

            // initializes function
            services.AddFunction( Configuration )
                .AddRouting()
                .AddControllers()
                    .AddFunctionControllers();

            services.ConfigureFunctionServices();
        }

        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            if ( !Configuration.GetValue<bool>( "Args:SkipAuth" ) )
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            // allow function implementation to customize the pipeline
            app.ConfigureFunction( env );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
