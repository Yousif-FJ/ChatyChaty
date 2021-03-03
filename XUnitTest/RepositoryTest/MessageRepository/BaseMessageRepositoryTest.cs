using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace XUnitTest.RepositoryTest.MessageRepositoryTest
{
    public abstract class BaseMessageRepositoryTest
    {
        protected readonly ChatyChatyContext dbContext;
        protected readonly MessageRepository repository;
        protected readonly AppUser user1;
        protected readonly AppUser user2;
        protected readonly AppUser user3;
        public BaseMessageRepositoryTest()
        {
            var SqliteInMemory = new ChatyChatySqliteInMemoryBuilder();

            dbContext = SqliteInMemory.CreateChatyChatyContext();

            repository = new MessageRepository(dbContext);

            //create users to test
            user1 = dbContext.Users.Add(new AppUser("FirstUser")).Entity;
            user2 = dbContext.Users.Add(new AppUser("SecondUser")).Entity;
            user3 = dbContext.Users.Add(new AppUser("SecondUser")).Entity;
            dbContext.SaveChanges();
        }
    }
}
