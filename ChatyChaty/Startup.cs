using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatyChaty.Hubs.v3;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.AuthenticationManager;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.Domain.Services.NotficationServices.Getter;
using ChatyChaty.Domain.Services.NotficationServices.Handler;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.StartupConfiguration;
using ChatyChaty.StartupConfiguration;
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

            services.AddIdentity<AppUser, Role>()
               .AddEntityFrameworkStores<ChatyChatyContext>();

            services.AddScoped<IAccountManager, AccountManager>();

            services.AddScoped<IHubHelper,HubHelper>();

            services.AddSingleton<IHubSessions,MemoryHubSessions>();

            services.AddScoped<IAuthenticationManager, AuthenticationManager>();

            services.AddScoped<INotificationGetter, NotificationGetter>();

            services.AddScoped<IMessageService, MessageService>();

            services.AddSignalR();

            services.AddInfrastructureClasses(Configuration);

            //add MediatR 
            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(UsersGotChatUpdateAsync).Assembly);

            //configure MVC 
            services.AddMvc(option =>
            {
                option.Filters.Add(new ProducesAttribute("application/json"));
                option.Filters.Add(new ConsumesAttribute("application/json"));
            });

            //configure DBcontext 

            services.CustomConfigureDbContext(Configuration);

            //configure identity 

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyz0123456789_";
            });

            //configure BearerJWT using extension method 
            services.CustomConfigureJwtAuthentication(Configuration);


            //configure swagger using extension method 
            services.CustomConfigureSwagger();

            //configure custom health check
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
