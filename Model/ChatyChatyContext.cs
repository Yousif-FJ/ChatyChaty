using ChatyChaty.Model.MessageModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class ChatyChatyContext : IdentityDbContext<AppUser>
    {
        private readonly ILogger<ChatyChatyContext> logger;

        public ChatyChatyContext(DbContextOptions dbContextOptions, ILogger<ChatyChatyContext> logger) : base(dbContextOptions)
        {
            this.logger = logger;
        }

        public DbSet<Message> MessagesSet { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseUrl;
                if (Environment.GetEnvironmentVariable("DATABASE_URL") != null)
                {
                    databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                }
                else
                {
                    logger.LogWarning("DATABASE_URL is null, falling back to inline Database connection");
                    databaseUrl = "postgres://ariehsrkswnkuy:f71fcbb30ddc875d91836b4ca0c0ca4af1da51b3885d73674381e0888ac757d5@ec2-54-75-231-215.eu-west-1.compute.amazonaws.com:5432/dkmm2e5rvrrn1";
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
        }
    }
}

