using Castle.Core.Internal;
using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.RepositoryTest.MessageRepositoryTest
{
    public class GetLastDeliveredMessageForEachChat: BaseMessageRepositoryTest
    {
        [Fact]
        public async Task NoChat_Should_Return_0_Messages()
        {
            //Arrange

            //Act
            var result = await repository.GetLastDeliveredMessageForEachChat(user1.Id);
            //Assert
            Assert.True(result.IsNullOrEmpty());
        }

        [Fact]
        public async Task NotDelivered_Should_Return_0_Messages()
        {
            //Arrage
            var messageBody = "Some message";
            var chat = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var message1 = new Message(messageBody, chat.Id, user1.Id);
            var message2 = new Message(messageBody, chat.Id, user1.Id);
            dbContext.Conversations.Add(chat);
            dbContext.Messages.AddRange(message1, message2);
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetLastDeliveredMessageForEachChat(user1.Id);
            //Assert
            Assert.True(result.IsNullOrEmpty());
        }

        [Fact]
        public async Task OneChat_OneDeliveredMessage_Should_Return_1_Message()
        {
            //Arrage
            var messageBody = "Some message";
            var chat = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var message1 = new Message(messageBody, chat.Id, user1.Id).MarkAsDelivered();
            var message2 = new Message(messageBody, chat.Id, user1.Id);
            dbContext.Add(chat);
            dbContext.Messages.AddRange(message1, message2);
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetLastDeliveredMessageForEachChat(user1.Id);
            //Assert
            Assert.True(result.Count() == 1);
            Assert.True(result.FirstOrDefault(m => m.Id == message1.Id).Body == message1.Body);
        }

        [Fact]
        public async Task OneChat_TwoDeliveredMessages_Should_Return_1_Message()
        {
            //Arrage
            var messageBody = "Some message";
            var chat = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var message1 = new Message(messageBody, chat.Id, user1.Id).MarkAsDelivered();
            var message2 = new Message(messageBody, chat.Id, user1.Id).MarkAsDelivered();
            dbContext.Add(chat);
            dbContext.Messages.AddRange(message1, message2);
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetLastDeliveredMessageForEachChat(user1.Id);
            //Assert
            Assert.True(result.Count() == 1);
            Assert.True(result.FirstOrDefault(m => m.Id == message2.Id).Body == message2.Body);
        }

        [Fact]
        public async Task TwoChat_OneReadMessage_Should_Return_1_Message()
        {
            //Arrage
            var messageBody = "Some message";
            var chat1 = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var chat2 = dbContext.Conversations.Add(new Conversation(user1.Id, user3.Id)).Entity;
            var message1 = new Message(messageBody, chat1.Id, user1.Id).MarkAsDelivered();
            var message2 = new Message(messageBody, chat2.Id, user1.Id);
            dbContext.Conversations.AddRange(chat1, chat2);
            dbContext.Messages.AddRange(message1, message2);
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetLastDeliveredMessageForEachChat(user1.Id);
            //Assert
            Assert.True(result.Count() == 1);
            Assert.True(result.FirstOrDefault(m => m.Id == message1.Id).Body == message1.Body);
        }

        [Fact]
        public async Task TwoChat_TwoReadMessage_Should_Return_2_Message()
        {
            //Arrage
            var messageBody = "Some message";
            var chat1 = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var chat2 = dbContext.Conversations.Add(new Conversation(user1.Id, user3.Id)).Entity;
            var message1 = new Message(messageBody, chat1.Id, user1.Id).MarkAsDelivered();
            var message2 = new Message(messageBody, chat2.Id, user1.Id).MarkAsDelivered();
            dbContext.Conversations.AddRange(chat1, chat2);
            dbContext.Messages.AddRange(message1, message2);
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetLastDeliveredMessageForEachChat(user1.Id);
            //Assert
            Assert.True(result.Count() == 2);
            Assert.True(result.FirstOrDefault(m => m.Id == message1.Id).Body == message1.Body);
            Assert.True(result.FirstOrDefault(m => m.Id == message2.Id).Body == message2.Body);
        }
    }
}
