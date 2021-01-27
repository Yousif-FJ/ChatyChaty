﻿using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.Domain.Services.NotficationServices.Handler;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.Repositories.ChatRepository;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using ChatyChaty.Infrastructure.Repositories.UserRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.MockClasses;

namespace XUnitTest
{
    public class MessageServiceTest
    {
        private readonly MessageService messageService;
        private readonly ChatyChatyContext dbContext;
        public MessageServiceTest()
        {
            //construct an In-Memory Database
            var options = new DbContextOptionsBuilder<ChatyChatyContext>()
                .UseInMemoryDatabase(databaseName: "database")
                .Options;
            var context = new ChatyChatyContext(options);
            dbContext = context;

            //construct a message repositor and notfication handler then a message service
            var messageRepository = new MessageRepository(context);
            var chatRepository = new ChatRepository(context);

            var notificationHandlerMock = new Mock<IMediator>() ;
            messageService = new MessageService(messageRepository, chatRepository, notificationHandlerMock.Object);
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
            var m = await dbContext.Messages.LastAsync();
            Assert.True(m.Body == message);
        }

        [Fact]
        public async Task GetNewMessages_ZeroAsMessageId()
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
            var MessagesResult = await messageService.GetNewMessages(u1.Id, 0);
            //Assert
            var Messagelist = new List<Message>(MessagesResult.Messages);
            Assert.True(Messagelist[0].Body == "test1");
            Assert.True(Messagelist[1].Body == "test2");
            Assert.True(Messagelist[0].ConversationId == conversation1.Id);
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
            var MessagesResult = await messageService.GetNewMessages(u2.Id, 0);
            //Assert
            var Messagelist = new List<Message>(MessagesResult.Messages);
            Assert.True(Messagelist[0].Body == "test1");
            Assert.True(Messagelist[0].ConversationId == conversation1.Id);
            Assert.True(Messagelist.Count == 1);
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
            var result = await messageService.IsDelivered(u1.Id, messageResult.Message.Id);
            //Assert
            Assert.False(result.IsDelivered);
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
            await messageService.GetNewMessages(u2.Id, 0);
            //Act
            var result = await messageService.IsDelivered(u1.Id, messageResult.Message.Id);
            //Assert
            Assert.True(result.IsDelivered);
        }
    }
}