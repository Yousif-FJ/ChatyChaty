using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.OldModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public class ChatyChatyContext : IdentityUserContext<AppUser, long>
    {
        private readonly ILogger<ChatyChatyContext> logger;

        public ChatyChatyContext(DbContextOptions dbContextOptions, ILogger<ChatyChatyContext> logger) : base(dbContextOptions)
        {
            this.logger = logger;
        }

        public DbSet<Message1> MessagesSet { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }


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
            builder.Entity<AppUser>().Ignore(f => f.NormalizedEmail);
            builder.Entity<AppUser>().Ignore(f => f.EmailConfirmed);
            builder.Entity<AppUser>().Ignore(f => f.PhoneNumberConfirmed);
            builder.Entity<AppUser>().Ignore(f => f.TwoFactorEnabled);


            builder.Entity<Message>()
                .HasOne<Conversation>(x => x.Conversation)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ConversationId);

            builder.Entity<Message>()
                .HasOne<AppUser>(x => x.Sender)
                .WithMany(x => x.MessageSender)
                .HasForeignKey(x => x.SenderId);

            builder.Entity<Conversation>()
                .HasOne<AppUser>(x => x.FirstUser)
                .WithMany(x => x.Conversations1)
                .HasForeignKey(x => x.FirstUserId);

            builder.Entity<Conversation>()
                .HasOne<AppUser>(x => x.SecondUser)
                .WithMany(x => x.Conversations2)
                .HasForeignKey(x => x.SecondUserId);

        }
    }
}
