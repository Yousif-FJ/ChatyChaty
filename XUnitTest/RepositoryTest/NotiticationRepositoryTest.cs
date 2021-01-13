using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.Repositories.NotificationRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.RepositoryTest
{
    public class NotiticationRepositoryTest
    {
        private readonly ChatyChatyContext dbContext;
        private readonly NotificationRepository notificationRepository;
        public NotiticationRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ChatyChatyContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            var context = new ChatyChatyContext(options);
            dbContext = context;

            var NotificationHandler = new NotificationRepository(dbContext);
            notificationRepository = NotificationHandler;
        }

        [Fact]
        public async Task IntializeNofification_CheckForUpdates_Intial_Value_check()
        {
            //Arrange
            await dbContext.Users.AddAsync(new AppUser("Test1"));
            var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            //Act
            var notification = await notificationRepository.GetNotificationAsync(user2.Id);
            //Assert
            Assert.True(notification.MessageUpdate == false);
        }

        [Fact]
        public async Task IntializeNofification_For_Existing_User()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            dbContext.SaveChanges();
            //Act
            var notification = await notificationRepository.GetNotificationAsync(user1.Id);
            //Assert
            Assert.True(notification.MessageUpdate == false);
        }


        [Fact]
        public async Task UsersGotMessageUpdateAsync_No_update()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            //var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            //Act
            var notification = await notificationRepository.GetNotificationAsync(user1.Id);
            //Assert
            Assert.False(notification.MessageUpdate);
            Assert.False(notification.ChatUpdate);
        }

        [Fact]
        public async Task UsersGotMessageUpdateAsync_Got_update()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            //var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            //Act
            await notificationRepository.StoreUserNewMessageStatusAsync(user1.Id);
            var notification = await notificationRepository.GetNotificationAsync(user1.Id);
            //Assert
            Assert.True(notification.MessageUpdate);
            Assert.False(notification.ChatUpdate);
        }
    }
}
