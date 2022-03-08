﻿using GGPuntoYComa.SSO.BusinesLogic.Identity;
using GGPuntoYComa.SSO.BusinesLogic.IdentityServer;
using GGPuntoYComa.SSO.BusinesLogic.Model.Identity;
using GGPuntoYComa.SSO.DataBaseBusiness;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGPuntoYComa.SSO.BusinesLogic
{
    public static class BussinesLogicCollectionExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            return AddBusinessLogic(services, null);
        }

        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, Action<DataBaseOptions> configureOptions)
        {
            DataBaseOptions options = new();

            configureOptions?.Invoke(options);

            //DataOptions
            services.AddSingleton(options);

            //DataBase
            services.AddDataBaseBusiness();

            //Management
            services.AddScoped<ClientsManagement>();
            services.AddScoped<ClientCorsOriginsManagement>();
            services.AddScoped<PersistedGrantsManagement>();
            services.AddScoped<IdentityResourcesManagement>();
            services.AddScoped<ApiScopesManagement>();
            services.AddScoped<ApiResourcesManagement>();
            services.AddScoped<ClientsBasicManagement>();
            services.AddScoped<UserManagement>();

            //Identity
            services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationAreaRole>, RoleStore>();

            services.AddIdentity<ApplicationUser, ApplicationAreaRole>()
                    .AddDefaultTokenProviders();

            return services;
        }
    }
}
