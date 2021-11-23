using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenFaaS
{
    internal class FunctionBuilder : IFunctionBuilder
    {
        public FunctionBuilder( IServiceCollection services )
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
