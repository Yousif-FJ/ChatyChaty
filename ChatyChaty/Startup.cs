using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using ChatyChaty.Hubs.v1;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.AuthenticationManager;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.Infrastructure.StartupConfiguration;
using ChatyChaty.StartupConfiguration;
using ChatyChaty.StartupConfiguration.ControllersCustomAttributes;
using ChatyChaty.Domain.Services.NotficationRequests;
using MediatR;

namespace ChatyChaty
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            //register classes in DI 

            services.AddScoped<IAccountManager, AccountManager>();

            services.AddScoped<IHubHelper,HubHelper>();

            services.AddSingleton<IHubSessions,MemoryHubSessions>();

            services.AddScoped<IAuthenticationManager, AuthenticationManager>();

            services.AddScoped<IMessageService, MessageService>();

            services.AddSignalR();

            services.AddInfrastructureClasses(Configuration);

            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(UsersGotChatUpdateAsync).Assembly);

            //configure MVC 
            //TODO- remove views
            services.AddControllersWithViews(option =>
            {
                option.Filters.Add(new ProducesAttribute("application/json"));
                option.Filters.Add(new ConsumesAttribute("application/json"));
                option.Conventions.Add(new SuppressAutoModelStateResponseAttribute());
                option.Filters.Add(typeof(CustomModelValidationResponseAttribute));
            });


            services.CustomConfigureDbContext(Configuration);

            services.CustomConfigureIdentity(Configuration);

            services.CustomConfigureJwtAuthentication(Configuration);

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.CustomConfigureSwagger();

            services.CustomConfigureHealthCheck();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseSwaggerCustomConfiguration();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions {
                    ResponseWriter = HealthCheckConfigurationExtension.CustomHealthCheckResponseWriter, AllowCachingResponses = true });
                endpoints.MapHub<MainHub>("/v1/chathub");
                endpoints.MapControllers();
            });
        }
    }
}
