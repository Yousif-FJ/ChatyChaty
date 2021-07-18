using ChatyChaty;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;
using ChatyChaty.Infrastructure.Database;

namespace XIntegrationTest
{
    public abstract class IntegrationTestBase
    {
        protected readonly HttpClient httpClient;
        protected readonly WebApplicationFactory<Startup> Factory;
        public IntegrationTestBase()
        {
            Factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    RemoveRealDBContext(services);

                    var sqliteMemory = new SqliteInMemory();
                    services.AddDbContextFactory<ChatyChatyContext>(optionsBuilder =>
                    {
                        optionsBuilder.UseSqlite(sqliteMemory.Connection);
                    });
                });
            });
            
            //create a special client that work the In-Memory app

            httpClient = Factory.CreateClient();
        }



        private static void RemoveRealDBContext(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChatyChatyContext>));
            services.Remove(descriptor);
        }
    }
}
