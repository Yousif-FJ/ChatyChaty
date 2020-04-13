using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.NotficationHandler;
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
            var NotificationHandler = new NotificationHandler(dbContext);
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
            await notificationHandler.IntializeNofificationHandler(user2.Id);
            //Assert
            var notification = await notificationHandler.CheckForUpdates(user2.Id);
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
            await notificationHandler.IntializeNofificationHandler(user1.Id);
            await notificationHandler.IntializeNofificationHandler(user1.Id);
            //Assert
            var notification = await notificationHandler.CheckForUpdates(user1.Id);
            Assert.True(notification.Id == user1.Id);
            Assert.True(notification.MessageUpdate == false);
        }


        [Fact]
        public async Task UserUpdatedProfile_No_update()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            await notificationHandler.IntializeNofificationHandler(user1.Id);
            await notificationHandler.IntializeNofificationHandler(user2.Id);
            //Act
            await notificationHandler.UserUpdatedProfile(user1.Id);
            //Assert
            var notification = await notificationHandler.CheckForUpdates(user2.Id);
            Assert.True(notification.Id == user2.Id);
            Assert.False(notification.MessageUpdate);
            Assert.False(notification.ChatUpdate);
        }

        [Fact]
        public async Task UserUpdatedProfile_Profile_Change()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            var user3 = (await dbContext.Users.AddAsync(new AppUser("Test3"))).Entity;
            dbContext.SaveChanges();
            await notificationHandler.IntializeNofificationHandler(user1.Id);
            await notificationHandler.IntializeNofificationHandler(user2.Id);
            await notificationHandler.IntializeNofificationHandler(user3.Id);
            await dbContext.Conversations.AddAsync(
                new Conversation
                {
                    FirstUserId = user1.Id,
                    SecondUserId = user2.Id
                }
                );
            dbContext.SaveChanges();
            //Act
            await notificationHandler.UserUpdatedProfile(user1.Id);
            //Assert
            //Check if the user1 got update
            var notification = await notificationHandler.CheckForUpdates(user2.Id);
            Assert.True(notification.Id == user2.Id);
            Assert.False(notification.MessageUpdate);
            Assert.True(notification.ChatUpdate);

            //Check if user3 got no update
            var notification2 = await notificationHandler.CheckForUpdates(user3.Id);
            Assert.True(notification2.Id == user3.Id);
            Assert.False(notification2.MessageUpdate);
            Assert.False(notification2.ChatUpdate);
        }

        [Fact]
        public async Task UserUpdatedProfile_update()
        {
            //Arrange
            var user1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var user2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            dbContext.SaveChanges();
            await notificationHandler.IntializeNofificationHandler(user1.Id);
            await notificationHandler.IntializeNofificationHandler(user2.Id);
            //Act
            await notificationHandler.UserGotChatUpdate(user1.Id);
            await notificationHandler.UserGotNewMessage(user1.Id);
            //Assert
            var notification = await notificationHandler.CheckForUpdates(user1.Id);
            Assert.True(notification.Id == user1.Id);
            Assert.True(notification.ChatUpdate);
            Assert.True(notification.MessageUpdate);
            Assert.False(notification.DeliveredUpdate);
        }
    }
}
