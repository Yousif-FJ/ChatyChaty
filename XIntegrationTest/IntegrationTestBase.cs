using ChatyChaty;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ChatyChaty.Model.DBModel;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v2;
using System.Net.Http.Headers;
using ChatyChaty.Model.AccountModel;
using System.Linq;

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
                    //Remove the real DB refrence and use an In-Memory one
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<ChatyChatyContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddDbContext<ChatyChatyContext>(options =>
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
            return (await response.Content.ReadAsAsync<AuthenticationResponse>()).Token;
        }
    }
}
