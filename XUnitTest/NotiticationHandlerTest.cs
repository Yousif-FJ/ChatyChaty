using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.NotificationRepository;
using ChatyChaty.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class NotiticationHandlerTest
    {
        private readonly ChatyChatyContext dbContext;
        private readonly NotificationHandler notificationHandler;
        public NotiticationHandlerTest()
        {
            var options = new DbContextOptionsBuilder<ChatyChatyContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            var context = new ChatyChatyContext(options);
            dbContext = context;
            var NotificationHandler = new NotificationHandler(new NotificationRepository(dbContext));
            this.notificationHandler = NotificationHandler;
        }

        [Fact]
        public async Task IntializeNofification_CheckForUpdates_Intial_Value_check()
        {
            //Arrange
            await dbContext.Users.AddAsync(new AppUser("Test1"));
            var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            //Act
            await notificationHandler.IntializeNotificationHandlerAsync(user2.Id);
            //Assert
            var notification = await notificationHandler.CheckForUpdatesAsync(user2.Id);
            Assert.True(notification.Id == user2.Id);
            Assert.True(notification.MessageUpdate == false);
        }

        [Fact]
        public async Task IntializeNofification_For_Existing_User()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            dbContext.SaveChanges();
            //Act
            await notificationHandler.IntializeNotificationHandlerAsync(user1.Id);
            await notificationHandler.IntializeNotificationHandlerAsync(user1.Id);
            //Assert
            var notification = await notificationHandler.CheckForUpdatesAsync(user1.Id);
            Assert.True(notification.Id == user1.Id);
            Assert.True(notification.MessageUpdate == false);
        }


        [Fact]
        public async Task UsersGotChatUpdateAsync_No_update()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            await notificationHandler.IntializeNotificationHandlerAsync(user1.Id);
            await notificationHandler.IntializeNotificationHandlerAsync(user2.Id);
            //Act
            await notificationHandler.UsersGotChatUpdateAsync(user1.Id);
            //Assert
            var notification = await notificationHandler.CheckForUpdatesAsync(user1.Id);
            Assert.True(notification.UserId == user1.Id);
            Assert.False(notification.MessageUpdate);
            Assert.False(notification.ChatUpdate);
        }
    }
}
