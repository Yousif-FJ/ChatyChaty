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

namespace XUnitTest
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
            await notificationRepository.IntializeNotificationHandlerAsync(user2.Id);
            //Assert
            var notification = await notificationRepository.CheckForUpdatesAsync(user2.Id);
            Assert.True(notification.MessageUpdate == false);
        }

        [Fact]
        public async Task IntializeNofification_For_Existing_User()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            dbContext.SaveChanges();
            //Act
            await notificationRepository.IntializeNotificationHandlerAsync(user1.Id);
            await notificationRepository.IntializeNotificationHandlerAsync(user1.Id);
            //Assert
            var notification = await notificationRepository.CheckForUpdatesAsync(user1.Id);
            Assert.True(notification.MessageUpdate == false);
        }


        [Fact]
        public async Task UsersGotChatUpdateAsync_No_update()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            await notificationRepository.IntializeNotificationHandlerAsync(user1.Id);
            await notificationRepository.IntializeNotificationHandlerAsync(user2.Id);
            //Act
            await notificationRepository.UsersGotChatUpdateAsync(user1.Id);
            //Assert
            var notification = await notificationRepository.CheckForUpdatesAsync(user1.Id);
            Assert.False(notification.MessageUpdate);
            Assert.False(notification.ChatUpdate);
        }
    }
}
