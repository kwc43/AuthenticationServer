using AuthenticationServer.Core.Entities;
using AuthenticationServer.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer
{
    public class Startup
    {
        private const string ConnectionStringName = "Default";
        private const string Address = "AppSettings:Address";

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        protected virtual void AddDbContext(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(ConfigureDatabase);
        }

        protected virtual void ConfigureDatabase(DbContextOptionsBuilder contextOptionsBuilder)
        {
            contextOptionsBuilder.UseSqlServer(Configuration.GetConnectionString(ConnectionStringName));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AddDbContext(services);

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            var builder = services.AddIdentityServer().AddOperationalStore(options =>
            {
                options.ConfigureDbContext = ConfigureDatabase;
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            })
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryClients(Config.GetClients(Configuration.GetValue(Address, "")))
            .AddAspNetIdentity<AppUser>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("Need to configure key material");
            }

            services.ConfigureApplicationCookie((obj) =>
            {
                obj.LoginPath = "/Accounts/Login";
                obj.LogoutPath = "/Accounts/Logout";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                    }
                });
            });

            app.UseRouting();
            app.UseStaticFiles();

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseIdentityServer();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
    }
}
