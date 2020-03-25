using ChatyChaty.Model.DBModel;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class MessageServiceTest
    {
        private readonly MessageService messageService;
        private readonly ChatyChatyContext dbContext;
        public MessageServiceTest()
        {
            var options = new DbContextOptionsBuilder<ChatyChatyContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            var context = new ChatyChatyContext(options);
            dbContext = context;
            messageService = new MessageService(context);
        }


        [Fact]
        public async Task GetConversation()
        {
            //Arrange
            var sender = new AppUser("Test1") 
            { 
                Id = 1
            };

            var reciver = new AppUser("Test2")
            {
                Id = 2
            };

            await dbContext.Users.AddRangeAsync(sender,reciver);
            dbContext.SaveChanges();
            //Act
            var ConversationId = await messageService.NewConversation(sender.Id, reciver.Id);
            //Assert
            var conversation = await dbContext.Conversations.FindAsync(ConversationId);
            Assert.True(conversation != null && 
                conversation.FirstUserId == sender.Id && 
                conversation.SecondUserId == reciver.Id);
        }

        [Fact]
        public async Task SendMessage_Test()
        {
            //Arrange
            var sender = new AppUser("Test1")
            {
                Id = 1
            };

            var reciver = new AppUser("Test2")
            {
                Id = 2
            };

            var conversation = await dbContext.Conversations.AddAsync(new Conversation
            {
                FirstUserId = sender.Id,
                SecondUserId = reciver.Id
            });


            await dbContext.Users.AddRangeAsync(sender, reciver);
            dbContext.SaveChanges();

            string message = "Some test message";
            //Act
            await messageService.SendMessage(conversation.Entity.Id, sender.Id, message);
            //Assert
            var m = await dbContext.Messages.LastAsync();
            Assert.True(m.Body == message);
        }
    }

}
