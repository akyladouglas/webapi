using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtendTeleMedicina.DistributedInterfaces.Services.Extensions;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, UserInfo>();

            return services;
        }
    }
}
