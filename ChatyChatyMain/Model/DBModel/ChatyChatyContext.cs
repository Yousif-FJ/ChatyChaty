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

        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Notification> Notifications { get; set; }


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

            builder.Entity<AppUser>()
                .HasOne<Notification>(x => x.Notification)
                .WithOne(x => x.AppUser)
                .HasForeignKey<Notification>(x => x.Id);

            builder.Entity<AppUser>()
                .Property(x => x.UserName)
                .HasMaxLength(32);

            builder.Entity<AppUser>()
                .Property(x => x.DisplayName)
                .HasMaxLength(32);
        }
    }
}

