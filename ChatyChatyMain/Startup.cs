using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using CloudinaryDotNet;
using ChatyChaty.Hubs.v3;
using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.AuthenticationManager;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.Domain.Services.NotficationServices.Getter;
using ChatyChaty.Domain.Services.NotficationServices.Handler;
using ChatyChaty.Infrastructure.PictureServices;
using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Repositories.UserRepository;
using ChatyChaty.Infrastructure.Repositories.ChatRepository;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using ChatyChaty.Infrastructure.Repositories.NotificationRepository;
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


            //configure BearerJWT -----------------------------------------------------------
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    //passing a delegate to replace the default not authorized response
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async delegate (JwtBearerChallengeContext context)
                        {
                            context.Response.ContentType = "application/json";
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync(
                                new ResponseBase<object>
                                {
                                    Success = false,
                                    Errors = new Collection<string> { "The user is not authenticated" }
                                }.ToJson()
                            );
                        },
                        //Receive token in query for the hub
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/v1/chathub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }

                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))
                             )
                    };
                });


            //configure swagger -------------------------------------------------------------
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "ChatyChatyAPI - V2", Version = "v2" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                   "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                  {
                    new OpenApiSecurityScheme
                      {
                      Reference = new OpenApiReference
                        {
                          Type = ReferenceType.SecurityScheme,
                          Id = "Bearer"
                        },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,

                      },
                    new List<string>()
                  }
              });
            });
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

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
            });

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
