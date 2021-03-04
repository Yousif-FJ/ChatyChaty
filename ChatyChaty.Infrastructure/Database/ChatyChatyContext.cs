using ChatyChaty.Domain.Model.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ChatyChaty.Infrastructure.Database
{
    public class ChatyChatyContext : IdentityUserContext<AppUser,UserId>
    {
        public ChatyChatyContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
//TODO-Factor out configuration
            builder.Entity<AppUser>().Ignore(f => f.Email);
            builder.Entity<AppUser>().Ignore(f => f.PhoneNumber);
            builder.Entity<AppUser>().Ignore(f => f.NormalizedEmail);
            builder.Entity<AppUser>().Ignore(f => f.EmailConfirmed);
            builder.Entity<AppUser>().Ignore(f => f.PhoneNumberConfirmed);
            builder.Entity<AppUser>().Ignore(f => f.TwoFactorEnabled);


            builder.Entity<Message>()
                .HasKey(m => m.Id);

            builder.Entity<Message>()
                .HasOne<Conversation>(x => x.Conversation)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ConversationId);

            builder.Entity<Message>()
                .HasOne<AppUser>(x => x.Sender)
                .WithMany(x => x.MessageSender)
                .HasForeignKey(x => x.SenderId);

            builder.Entity<Message>()
                .Property(x => x.Id)
                .HasConversion(x => x.Value,
                               x => new MessageId(x));

            builder.Entity<Conversation>()
                .HasKey(c => c.Id);

            builder.Entity<Conversation>()
                .HasOne<AppUser>(x => x.FirstUser)
                .WithMany(x => x.Conversations1)
                .HasForeignKey(x => x.FirstUserId);

            builder.Entity<Conversation>()
                .HasOne<AppUser>(x => x.SecondUser)
                .WithMany(x => x.Conversations2)
                .HasForeignKey(x => x.SecondUserId);

            builder.Entity<Conversation>()
                .Property(x => x.Id)
                .HasConversion(x => x.Value,
                               x => new ConversationId(x));

            builder.Entity<AppUser>()
                .Property(x => x.UserName)
                .HasMaxLength(32);

            builder.Entity<AppUser>()
                .Property(x => x.DisplayName)
                .HasMaxLength(32);

            builder.Entity<AppUser>()
                .Property(x => x.Id)
                .HasConversion(x => x.Value,
                               x => new UserId(x));
        }
    }
}

