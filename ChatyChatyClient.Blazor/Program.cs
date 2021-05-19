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
using ChatyChatyClient.Logic.Repository.Interfaces;
using ChatyChatyClient.Logic.Repository.Implementation;
using ChatyChatyClient.Logic.Actions.Handler.Authentication;
using ChatyChatyClient.Logic.StartupConfig;
using Microsoft.AspNetCore.Components.Authorization;
using ChatyChatyClient.Blazor.StartUpConfiguratoin;

namespace ChatyChatyClient.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");


            //http client stuff
            builder.Services.AddHttpClient("ServerAPI",
                    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<AddAuthTokenToHttpClient>();

            builder.Services.AddTransient<AddAuthTokenToHttpClient>();
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ServerAPI"));


            builder.Services.AddMediatR(typeof(LoginHandler));

            //repository stuff
            builder.Services.AddScoped<IAuthenticationRepository, LocalStorageAuthRepository>();
            builder.Services.AddScoped<IProfileRepository, LocalStorageProfileRepository>();
            builder.Services.AddBlazoredLocalStorage();

            //authentication stuff
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            //state containers
            builder.Services.AddSingleton<IChatStateContainer, ChatMemoryStateContainer>();

            await builder.Build().RunAsync();
        }
    }
}
