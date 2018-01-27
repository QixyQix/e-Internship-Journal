using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using E_Internship_Journal.Data;
using E_Internship_Journal.Models;
using E_Internship_Journal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace E_Internship_Journal
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                //Prepare for HTTPS | Reference : https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl
                //options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddApplicationInsightsTelemetry(Configuration);
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSession();
            services.AddMemoryCache();
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                //Prepare for only "ConfirmEmail" Log in | Reference : https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl
                // config.SignIn.RequireConfirmedEmail = true;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            var defaultPolicy = new AuthorizationPolicyBuilder()
       .RequireAuthenticatedUser()
       .Build();
            services.AddAuthorization(options =>
            {
                // inline policies
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("ADMIN"));
                options.AddPolicy("RequireSeniorOfficerRole", policy => policy.RequireRole("SLO"));
                options.AddPolicy("RequireOfficerRole", policy => policy.RequireRole("LO"));
                options.AddPolicy("RequireSupervisorRole", policy => policy.RequireRole("SUPERVISOR"));
                options.AddPolicy("RequireStudentRole", policy => policy.RequireRole("STUDENT"));
            });
            services.AddMvc(setup =>
            {
                setup.Filters.Add(new AuthorizeFilter(defaultPolicy));
            });
            services.AddMvc()
                  .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());


            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //var sslPort = 0;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();

                // Redirect https port to correct port
                //Reference : https://long2know.com/2016/07/asp-net-core-enforcing-https/
                //            var builder = new ConfigurationBuilder()
                //.SetBasePath(env.ContentRootPath)
                //.AddJsonFile(@"Properties/launchSettings.json", optional: false, reloadOnChange: true);
                //            var launchConfig = builder.Build();
                //            sslPort = launchConfig.GetValue<int>("iisSettings:iisExpress:sslPort");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            // Redirect https port to correct port
            //Reference : https://long2know.com/2016/07/asp-net-core-enforcing-https/
            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.IsHttps)
            //    {
            //        await next();
            //    }
            //    else
            //    {
            //        var sslPortStr = sslPort == 0 || sslPort == 443 ? string.Empty : $":{sslPort}";
            //        var httpsUrl = $"https://{context.Request.Host.Host}{sslPortStr}{context.Request.Path}";
            //        context.Response.Redirect(httpsUrl);
            //    }
            //});
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DataSeeder.SeedData(new ApplicationDbContext());
        }
    }
}
