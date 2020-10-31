using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTest.RepositoryTest.MessageRepositoryTest
{
    public class BaseMessageRepositoryTest
    {
        protected readonly ChatyChatyContext dbContext;
        protected readonly MessageRepository repository;
        protected readonly AppUser user1;
        protected readonly AppUser user2;
        protected readonly AppUser user3;
        public BaseMessageRepositoryTest()
        {
            //construct an In-Memory Database
            var options = new DbContextOptionsBuilder<ChatyChatyContext>()
                .UseInMemoryDatabase(databaseName: "database")
                .Options;
            var context = new ChatyChatyContext(options);
            dbContext = context;

            //construct repository
            repository = new MessageRepository(dbContext);

            //create users to test
            user1 = dbContext.Users.Add(new AppUser("FirstUser")).Entity;
            user2 = dbContext.Users.Add(new AppUser("SecondUser")).Entity;
            user3 = dbContext.Users.Add(new AppUser("SecondUser")).Entity;
            dbContext.SaveChanges();
        }
    }
}
