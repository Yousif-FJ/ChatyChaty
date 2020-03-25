using ChatyChaty.Model.OldModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.DBModel
{
    public class ChatyChatyContext : IdentityUserContext<AppUser,long>
    {
        public ChatyChatyContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Message1> MessagesSet { get; set; }
        public DbSet<Message> Messages{ get; set; }
        public DbSet<Chat> Chats { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
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
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().Ignore(f => f.Email);
            builder.Entity<AppUser>().Ignore(f => f.PhoneNumber);
            builder.Entity<AppUser>().Ignore(f => f.NormalizedEmail);
            builder.Entity<AppUser>().Ignore(f => f.EmailConfirmed);
            builder.Entity<AppUser>().Ignore(f => f.PhoneNumberConfirmed);
            builder.Entity<AppUser>().Ignore(f => f.TwoFactorEnabled);


            builder.Entity<Message>()
                .HasOne<Chat>(x => x.Chat)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ChatId);

            builder.Entity<Message>()
                .HasOne<AppUser>(x => x.Sender)
                .WithMany(x => x.MessageSender)
                .HasForeignKey(x => x.SenderId);

            builder.Entity<Chat>()
                .HasOne<AppUser>(x => x.FirstUser)
                .WithMany(x => x.Chat1)
                .HasForeignKey(x => x.FirstUserId);

            builder.Entity<Chat>()
                .HasOne<AppUser>(x => x.SecondUser)
                .WithMany(x => x.Chat2)
                .HasForeignKey(x => x.SecondUserId);

        }
    }
}

