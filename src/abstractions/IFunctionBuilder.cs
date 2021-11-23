using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenFaaS
{
    public interface IFunctionBuilder
    {
        IServiceCollection Services { get; }
    }
}
