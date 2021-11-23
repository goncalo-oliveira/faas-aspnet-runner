using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace OpenFaaS.NewtonsoftJson
{
    internal class NewtonsoftJsonMvcBuilder : IMvcBuilder
    {
        public NewtonsoftJsonMvcBuilder( IFunctionBuilder builder )
        {
            Services = builder.Services;
        }

        public IServiceCollection Services { get; }

        public ApplicationPartManager PartManager => throw new NotImplementedException();
    }
}