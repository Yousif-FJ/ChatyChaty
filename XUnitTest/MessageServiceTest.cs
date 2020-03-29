using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
using ChatyChaty.Model.NotficationHandler;
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
            var MessageRepositor = new MessageRepository(context);
            NotificationHandler notificationHandler = new NotificationHandler(dbContext);
            messageService = new MessageService(MessageRepositor,notificationHandler);
        }


        [Fact]
        public async Task NewConversation_success()
        {
            //Arrange
            var sender = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var reciver = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
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
        public async Task SendMessage_success()
        {
            //Arrange
            var sender = (await dbContext.Users.AddAsync(new AppUser("Test1"))).Entity;
            var reciver = (await dbContext.Users.AddAsync(new AppUser("Test2"))).Entity;
            var conversation = (await dbContext.Conversations.AddAsync(new Conversation
            {
                FirstUserId = sender.Id,
                SecondUserId = reciver.Id
            })).Entity;

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
            var conversation1 = (await dbContext.Conversations.AddAsync(new Conversation
            {
                FirstUserId = u1.Id,
                SecondUserId = u2.Id
            })).Entity;

            var Messages = new List<Message>
            {
                new Message
                {
                    Body = "test1",
                    ConversationId = conversation1.Id,
                    SenderId = u1.Id
                },
                new Message
                {
                    Body = "test2",
                    ConversationId = conversation1.Id,
                    SenderId = u2.Id
                }
            };

            await dbContext.AddRangeAsync(Messages);
            await dbContext.SaveChangesAsync();


            //Act
            var MessagesResult = await messageService.GetNewMessages(u1.Id, 0);
            //Assert
            var Messagelist = new List<Message>(MessagesResult);
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
            var conversation1 = (await dbContext.Conversations.AddAsync(new Conversation
            {
                FirstUserId = u1.Id,
                SecondUserId = u2.Id
            })).Entity;

            var conversation2 = (await dbContext.Conversations.AddAsync(new Conversation
            {
                FirstUserId = u1.Id,
                SecondUserId = u3.Id
            })).Entity;

            var Messages = new List<Message>
            {
                new Message
                {
                    Body = "test1",
                    ConversationId = conversation1.Id,
                    SenderId = u1.Id
                },
                new Message
                {
                    Body = "test3",
                    ConversationId = conversation2.Id,
                    SenderId = u1.Id
                }
            };

            await dbContext.AddRangeAsync(Messages);
            await dbContext.SaveChangesAsync();


            //Act
            var MessagesResult = await messageService.GetNewMessages(u2.Id, 0);
            //Assert
            var Messagelist = new List<Message>(MessagesResult);
            Assert.True(Messagelist[0].Body == "test1");
            Assert.True(Messagelist[0].ConversationId == conversation1.Id);
            Assert.True(Messagelist.Count == 1);
        }
    }
}
