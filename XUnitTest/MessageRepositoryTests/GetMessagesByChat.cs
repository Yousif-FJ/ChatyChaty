using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.MessageRepositoryTests
{
    public class GetMessagesByChat : BaseMessageRepositoryTest
    {
        [Fact]
        public async Task OneMessage_OneChat_ShouldReturn_OneMessage()
        {
            //Arrange
            var chat = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var message = dbContext.Messages.Add(new Message("some message", chat.Id, user1.Id)).Entity;
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetForChatAsync(chat.Id);
            //Assert
            Assert.Equal(1,result.Count);
            Assert.Equal(message.Body, result.FirstOrDefault().Body);
            Assert.Equal(message.Id, result.FirstOrDefault().Id);
            Assert.Equal(message.ConversationId, result.FirstOrDefault().ConversationId);
        }

        [Fact]
        public async Task TwoMessages_TwoChats_ShouldReturn_OneMessage()
        {
            //Arrange
            var chat1 = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var chat2 = dbContext.Conversations.Add(new Conversation(user1.Id, user3.Id)).Entity;
            var message1 = dbContext.Messages.Add(new Message("some message", chat1.Id, user1.Id)).Entity;
            _ = dbContext.Messages.Add(new Message("some message", chat2.Id, user1.Id)).Entity;
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetForChatAsync(chat1.Id);
            //Assert
            Assert.Equal(1, result.Count);
            Assert.Equal(message1.Id, result.FirstOrDefault().Id);
        }

        [Fact]
        public async Task ThreeMessages_TwoChats_ShouldReturn_TwoMessage()
        {
            //Arrange
            var chat1 = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            var chat2 = dbContext.Conversations.Add(new Conversation(user1.Id, user3.Id)).Entity;
            var message1 = dbContext.Messages.Add(new Message("some message", chat1.Id, user1.Id)).Entity;
            var message2 = dbContext.Messages.Add(new Message("some message", chat1.Id, user1.Id)).Entity;
            _ = dbContext.Messages.Add(new Message("some message", chat2.Id, user1.Id)).Entity;
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetForChatAsync(chat1.Id);
            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(message1.Id, result.FirstOrDefault().Id);
            Assert.Equal(message2.Id, result.LastOrDefault().Id);
        }
    }
}
