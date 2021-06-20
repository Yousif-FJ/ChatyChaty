using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace XUnitTest.MessageRepositoryTests
{
    public abstract class MessageRepositoryTestBase
    {
        protected readonly ChatyChatyContext dbContext;
        protected readonly MessageRepository repository;
        protected readonly AppUser user1;
        protected readonly AppUser user2;
        protected readonly AppUser user3;
        protected readonly Conversation chatUser1AndUser2;
        protected readonly Conversation chatUser1AndUser3;
        public MessageRepositoryTestBase()
        {
            var SqliteInMemory = new ChatyChatySqliteInMemoryBuilder();

            dbContext = SqliteInMemory.CreateChatyChatyContext();

            repository = new MessageRepository(dbContext);

            //create users and chat to test
            user1 = dbContext.Users.Add(new AppUser("FirstUser")).Entity;
            user2 = dbContext.Users.Add(new AppUser("SecondUser")).Entity;
            user3 = dbContext.Users.Add(new AppUser("SecondUser")).Entity;
            chatUser1AndUser2 = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            chatUser1AndUser3 = dbContext.Conversations.Add(new Conversation(user1.Id, user3.Id)).Entity;
            dbContext.SaveChanges();

        }
    }
}
