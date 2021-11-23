using System;
using Microsoft.AspNetCore.Mvc;
using OpenFaaS;
using OpenFaaS.NewtonsoftJson;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NewtonsoftJsonFunctionBuilderExtensions
    {
        /// <summary>
        /// Configures Newtonsoft.Json specific features such as input and output formatters
        /// </summary>
        /// <returns>The OpenFaaS.IFunctionBuilder instance</returns>
        public static IFunctionBuilder AddNewtonsoftJson( this IFunctionBuilder builder )
        {
            IMvcBuilder mvcBuilder = new NewtonsoftJsonMvcBuilder( builder );

            mvcBuilder.AddNewtonsoftJson();

            return ( builder );
        }

        /// <summary>
        /// Configures Newtonsoft.Json specific features such as input and output formatters
        /// </summary>
        /// <param name="configure">Callback to configure Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions</param>
        /// <returns>The OpenFaaS.IFunctionBuilder instance</returns>
        public static IFunctionBuilder AddNewtonsoftJson( this IFunctionBuilder builder, Action<MvcNewtonsoftJsonOptions> configure )
        {
            IMvcBuilder mvcBuilder = new NewtonsoftJsonMvcBuilder( builder );

            mvcBuilder.AddNewtonsoftJson( configure );

            return ( builder );
        }
    }
}