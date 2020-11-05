using ChatyChaty.ControllerHubSchema.v3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.StartupConfiguration
{
    public static class AuthenticationDIExtension
    {
        public static void CustomConfigureJwtAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        //Replace the default not authorized response
                        OnChallenge = OnChallengeCustomHandler,
                        //Receive token in query for the hub
                        OnMessageReceived = OnMessageReceivedCustomHandler

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
        }

        /// <summary>
        /// Replace the default not authorized response
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task OnChallengeCustomHandler(JwtBearerChallengeContext context)
        {
            context.Response.ContentType = "application/json";
            context.HandleResponse();
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(
                new ResponseBase<object>
                {
                    Success = false,
                    Errors = new List<string> { "The user is not authenticated" }
                }.ToJson());
        }

        /// <summary>
        /// Read auth tokne from the URL for the Hub
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Task OnMessageReceivedCustomHandler(MessageReceivedContext context)
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
    }
}
