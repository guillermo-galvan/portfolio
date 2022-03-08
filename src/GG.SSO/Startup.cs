using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using GGPuntoYComa.SSO.BusinesLogic;
using GGPuntoYComa.SSO.BusinesLogic.Identity;
using GGPuntoYComa.SSO.BusinesLogic.Model.Identity;
using GGPuntoYComa.SSO.Models;
using GGPuntoYComa.SSO.Store.IdentityServer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data.CRUD;
using System.Data.CRUD.MySql;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GGPuntoYComa.SSO
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-MX");
            ConnectionDataBaseCollection.Connections.Add("MainConnection",new MySqlDataBaseConnection(Configuration.GetConnectionString("MainConnection")));

            IConfigurationSection configurationMyKeys = Configuration.GetSection(nameof(ExternalAuthentication));
            ExternalAuthentication[] externalAuthentication = configurationMyKeys.Get<ExternalAuthentication[]>();

            services.AddBusinessLogic();
            
            services.AddScoped<IResourceStore, ResourceStore>();
            services.AddScoped<IClientStore, ClientStore>();
            services.AddScoped<ICorsPolicyService, CorsPolicyService>();
            services.AddScoped<IPersistedGrantStore, PersistedGrantStore>();

            //duende identity
            var builder = services.AddIdentityServer()
                                 .AddResourceStore<ResourceStore>()
                                 .AddClientStore<ClientStore>()
                                 .AddCorsPolicyService<CorsPolicyService>()
                                 .AddAspNetIdentity<ApplicationUser>();

            builder.AddProfileService<ProfileService>();

#if DEBUG
            //This is for dev only scenarios when you don�t have a certificate to use.
            builder.AddDeveloperSigningCredential();
#endif

            if (externalAuthentication.Any())
            {
                var authenticationBuilder = services.AddAuthentication();
                ExternalAuthentication google = externalAuthentication.FirstOrDefault(x => x.External == "Google");

                if (google != null)
                {   
                    authenticationBuilder.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        options.ClientId = google.ClientId;
                        options.ClientSecret = google.ClientSecret;
                    });
                }

                ExternalAuthentication facebook = externalAuthentication.FirstOrDefault(x => x.External == "Facebook");
                if (facebook != null)
                {
                    authenticationBuilder.AddFacebook(options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        options.AppId = facebook.ClientId;
                        options.AppSecret = facebook.ClientSecret;
                    });
                }

                ExternalAuthentication microsof = externalAuthentication.FirstOrDefault(x => x.External == "Microsof");
                if (microsof != null)
                {
                    authenticationBuilder.AddMicrosoftAccount(options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        options.ClientId = microsof.ClientId;
                        options.ClientSecret = microsof.ClientSecret;
                    });
                }
            }

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    // Podr�amos especificar �nicamente los or�genes permitidos.
                    policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
                });
            });

            services.AddControllersWithViews();           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("default");
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
