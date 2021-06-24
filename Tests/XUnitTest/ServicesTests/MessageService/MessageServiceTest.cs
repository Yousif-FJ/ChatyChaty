using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.Domain.Services.NotficationRequests;
using ChatyChaty.Domain.Services.ScopeServices;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.Repositories.ChatRepository;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using ChatyChaty.Infrastructure.Repositories.UserRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
//TODO: Service test should test The service logic not the repository logic
namespace XUnitTest.Services
{
    public class MessageServiceTest
    {
        private readonly MessageService messageService;
        private readonly ChatyChatyContext dbContext;
        public MessageServiceTest()
        {
            dbContext = new ChatyChatySqliteInMemoryBuilder()
                .CreateChatyChatyContext() ;

            var messageRepository = new MessageRepository(dbContext);
            var chatRepository = new ChatRepository(dbContext);

            var loggerMock = new Mock<ILogger<FireAndForgetService>>();
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();

            var fireAndForegetMock = new Mock<FireAndForgetService>(loggerMock.Object, scopeFactoryMock.Object);
            messageService = new MessageService(messageRepository, chatRepository, fireAndForegetMock.Object);
        }


        [Fact]
        public async Task SendMessage_success()
        {
            //Arrange
            var sender = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var reciver = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            var conversation = (await dbContext.Conversations.AddAsync(new Conversation(sender.Id,reciver.Id))).Entity;

            await dbContext.SaveChangesAsync();

            string message = "Some test message";
            //Act
            await messageService.SendMessage(conversation.Id, sender.Id, message);
            //Assert
            var m = await dbContext.Messages.FirstAsync();
            Assert.True(m.Body == message);
        }

        [Fact]
        public async Task GetNewMessages_AllMessages()
        {
            //Arrange
            var u1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var u2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            var conversation1 = (await dbContext.Conversations.AddAsync(new Conversation(u1.Id, u2.Id))).Entity;

            var Messages = new List<Message>
            {
                new Message("test1",  conversation1.Id ,u1.Id),
                new Message("test2", conversation1.Id, u2.Id)
            };

            await dbContext.AddRangeAsync(Messages);
            await dbContext.SaveChangesAsync();


            //Act
            var MessagesResult = await messageService.GetMessages(u1.Id);
            //Assert
            Assert.True(MessagesResult[0].Body == "test1");
            Assert.True(MessagesResult[1].Body == "test2");
            Assert.True(MessagesResult[0].ConversationId == conversation1.Id);
        }

        [Fact]
        public async Task GetNewMessages_CorrectChat()
        {
            //Arrange
            var u1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var u2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            var u3 = (await dbContext.Users.AddAsync(new AppUser("Test3"))).Entity;
            var conversation1 = (await dbContext.Conversations.AddAsync(new Conversation(u1.Id,u2.Id))).Entity;

            var conversation2 = (await dbContext.Conversations.AddAsync(new Conversation(u1.Id, u3.Id))).Entity;

            var Messages = new List<Message>
            {
                new Message( "test1", conversation1.Id, u1.Id),
                new Message( "test3", conversation2.Id, u1.Id)
            };

            await dbContext.AddRangeAsync(Messages);
            await dbContext.SaveChangesAsync();

            //Act
            var MessagesResult = await messageService.GetMessages(u2.Id);
            //Assert
            Assert.True(MessagesResult[0].Body == "test1");
            Assert.True(MessagesResult[0].ConversationId == conversation1.Id);
            Assert.True(MessagesResult.Count == 1);
        }

        [Fact]
        public async Task IsDelivered_False()
        {
            //Arrange
            var u1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var u2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            var conversation2 = (await dbContext.Conversations.AddAsync(new Conversation( u1.Id, u2.Id))).Entity;
            await dbContext.SaveChangesAsync();
            var messageResult = await messageService.SendMessage(conversation2.Id, u1.Id, "Test Message");
            //Act
            var result = await messageService.IsDelivered(u1.Id, messageResult.Id);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsDelivered_True()
        {
            //Arrange
            var u1 = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var u2 = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            var conversation2 = (await dbContext.Conversations.AddAsync(new Conversation( u1.Id,u2.Id))).Entity;
            await dbContext.SaveChangesAsync();
            var messageResult = await messageService.SendMessage(conversation2.Id, u1.Id, "Test Message");
            await messageService.GetMessages(u2.Id);
            //Act
            var result = await messageService.IsDelivered(u1.Id, messageResult.Id);
            //Assert
            Assert.True(result);
        }
    }
}
