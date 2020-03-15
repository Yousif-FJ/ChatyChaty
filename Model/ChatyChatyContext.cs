using ChatyChaty.Model.MessageModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class ChatyChatyContext : IdentityUserContext<AppUser,long>
    {
        private readonly ILogger<ChatyChatyContext> logger;

        public ChatyChatyContext(DbContextOptions dbContextOptions, ILogger<ChatyChatyContext> logger) : base(dbContextOptions)
        {
            this.logger = logger;
        }

        public DbSet<Message> MessagesSet { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=ChatyChatyDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().Ignore(f => f.Email);
            builder.Entity<AppUser>().Ignore(f => f.PhoneNumber);
            builder.Entity<AppUser>(f => f.HasKey(b => b.Id));
            builder.Entity<AppUser>().Ignore(f => f.NormalizedEmail);
        }
    }
}

