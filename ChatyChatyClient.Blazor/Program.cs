using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using ChatyChatyClient.Logic.Repository.Interfaces;
using ChatyChatyClient.Logic.Repository.Implementation;
using ChatyChatyClient.Logic.Actions.Handler.Authentication;

namespace ChatyChatyClient.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddMediatR(typeof(LoginHandler));

            builder.Services.AddScoped<IAuthenticationRepository, LocalStorageAuthRepository>();
            builder.Services.AddScoped<IProfileRepository, LocalStorageProfileRepository>();

            builder.Services.AddSingleton<IChatStateContainer, ChatMemoryStateContainer>();

            await builder.Build().RunAsync();
        }
    }
}
