using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using CloudinaryDotNet;
using ChatyChaty.Hubs.v3;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.AuthenticationManager;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.Domain.Services.NotficationServices.Getter;
using ChatyChaty.Domain.Services.NotficationServices.Handler;
using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.PictureServices;
using ChatyChaty.Infrastructure.Repositories.UserRepository;
using ChatyChaty.Infrastructure.Repositories.ChatRepository;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using ChatyChaty.Infrastructure.Repositories.NotificationRepository;
using MediatR;
using ChatyChaty.StartupConfiguration;

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

            //register classes in DI -----------------------------------------------------

            services.AddIdentity<AppUser, Role>()
               .AddEntityFrameworkStores<ChatyChatyContext>();

            services.AddScoped<IAccountManager, AccountManager>();

            services.AddScoped<IHubHelper,HubHelper>();

            services.AddSingleton<HubConnectedClients>();

            services.AddSingleton<Cloudinary>();

            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IPictureProvider, CloudinaryPictureProvider>();

            services.AddScoped<IAuthenticationManager, AuthenticationManager>();

            services.AddScoped<INotificationGetter, NotificationGetter>();

            services.AddScoped<IMessageService, MessageService>();

            services.AddSignalR();

            //add MediatR ---------------------------------------------------------------
            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(UsersGotChatUpdateAsync).Assembly);

            //configure MVC ---------------------------------------------------------------
            services.AddMvc(option =>
            {
                option.Filters.Add(new ProducesAttribute("application/json"));
                option.Filters.Add(new ConsumesAttribute("application/json"));
            });

            //configure DBcontext ----------------------------------------------------------

            services.AddDbContextPool<ChatyChatyContext>(optionsBuilder =>
            {
                string databaseUrl;
                if (Environment.GetEnvironmentVariable("DATABASE_URL") != null)
                {
                    databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                }
                else
                {
                    throw new Exception("Couldn't get connection string");
                }
                var databaseUri = new Uri(databaseUrl);
                var userInfo = databaseUri.UserInfo.Split(':');

                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = databaseUri.Host,
                    Port = databaseUri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = databaseUri.LocalPath.TrimStart('/'),
                    SslMode = SslMode.Require,
                    TrustServerCertificate = true
                };


                optionsBuilder.UseNpgsql(builder.ToString());
            });


            //configure identity -----------------------------------------------------------

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

            //configure BearerJWT using extension method -----------------------------------------------------------
            services.CustomConfigureJwtAuthentication(Configuration);


            //configure swagger using extension method -------------------------------------
            services.CustomConfigureSwagger();
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
                endpoints.MapHub<MainHub>("/v1/chathub");
                endpoints.MapControllers();
            });
        }
    }
}
