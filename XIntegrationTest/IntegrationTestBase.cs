using ChatyChaty;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;
using ChatyChaty.ControllerHubSchema.v2;
using System.Net.Http.Headers;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using ChatyChaty.Infrastructure.Database;

namespace XIntegrationTest
{
    public class IntegrationTestBase
    {
        protected readonly HttpClient client;
        public IntegrationTestBase()
        {
            //construct an In-Memory server with chaty startup class
            var Factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                //Remove the real DB refrence 
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ChatyChatyContext>));
                services.Remove(descriptor);

                    //use an In-Memory db  
                    services.AddDbContextPool<ChatyChatyContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });
            //create a special client that work the In-Memory app
            client = Factory.CreateClient();
        }



        protected void Authenticate(string token)
        {
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("bearer",token);
        }

        protected async Task<string> CreateAccount(string userName, string displayName, string password)
        {

            var response = await client.PostAsJsonAsync("/api/v2/Authentication/CreateAccount", new CreateAccountSchema 
            {
                UserName = userName,
                Password = password,
                DisplayName = displayName
            });
            var textResponse = await response.Content.ReadAsStringAsync();
            return (await response.Content.ReadAsAsync<AuthenticationResponse>()).Token;
        }
    }
}
