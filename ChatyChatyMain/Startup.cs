using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChatyChaty.Model;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
using ChatyChaty.Model.NotficationHandler;
using ChatyChaty.Services;
using ChatyChaty.Services.GoogleFirebase;
using ChatyChaty.ValidationAttribute;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
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

            services.AddSingleton<Cloudinary>();

            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IPictureProvider, CloudinaryPictureProvider>();

            services.AddScoped<IAuthenticationManager, AuthenticationManager>();

            services.AddScoped<INotificationHandler, NotificationHandler>();

            services.AddScoped<IMessageService, MessageService>();

            //Test google authentication
            services.AddSingleton<FirstTest>();

            //configure MVC ---------------------------------------------------------------
            services.AddMvc(option =>
            {
                option.Filters.Add(new ProducesAttribute("application/json"));
                option.Filters.Add(new ConsumesAttribute("application/json"));
            });

            //configure DBcontext ----------------------------------------------------------

            services.AddDbContext<ChatyChatyContext>(optionsBuilder =>
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
                c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "ChatyChatyAPI" });
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

            //configure cookie to disable redirect -----------------------------------------
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
