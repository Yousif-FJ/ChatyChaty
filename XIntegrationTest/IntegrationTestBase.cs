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
            var Factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
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
            client = Factory.CreateClient();
        }



        protected void Authenticate(string token)
        {
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("bearer",token);
        }

        protected async Task<string> CreateAccount(AccountModel accountSchema)
        {

            var response = await client.PostAsJsonAsync("/api/v2/Authentication/CreateAccount", new CreateAccountSchema 
            {
                UserName = accountSchema.UserName,
                Password = accountSchema.Password,
                DisplayName = accountSchema.DisplayName
            });
            return (await response.Content.ReadAsAsync<AuthenticationResponse>()).Token;
        }
    }
}
