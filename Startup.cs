using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChatyChaty.Model;
using ChatyChaty.Model.MessageModels;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

            //register services ----------------------------------------------------------
            services.AddMvc();

            services.AddDbContext<ChatyChatyContext>();

            services.AddScoped<IAccountManager, AccountManager>();

            services.AddScoped<IMessageRepository, MessagesRepository>();

            services.AddIdentity<IdentityUser, IdentityRole>()
               .AddEntityFrameworkStores<ChatyChatyContext>();


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
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            });


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
                          ValidateLifetime = true,
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
                c.SwaggerDoc("ChatyChatyAPI", new OpenApiInfo { Title = "ChatyChatyAPI" });
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

            //configure identity to disable redirect ---------------------------------
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

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/ChatyChatyAPI/swagger.json", "ChatyChatyAPI");

            });

            app.UseStaticFiles();


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
