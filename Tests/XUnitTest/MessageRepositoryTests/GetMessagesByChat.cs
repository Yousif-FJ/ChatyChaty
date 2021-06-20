using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.MessageRepositoryTests
{
    public class GetMessagesByChat : MessageRepositoryTestBase
    {
        [Fact]
        public async Task OneMessage_OneChat_ShouldReturn_OneMessage()
        {
            //Arrange
            var message = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user1.Id)).Entity;
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetForChatAsync(chatUser1AndUser2.Id);
            //Assert
            Assert.Single(result);
            Assert.Equal(message.Body, result.FirstOrDefault().Body);
            Assert.Equal(message.Id, result.FirstOrDefault().Id);
            Assert.Equal(message.ConversationId, result.FirstOrDefault().ConversationId);
        }

        [Fact]
        public async Task TwoMessages_TwoChats_ShouldReturn_OneMessage()
        {
            //Arrange
            var message1 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user1.Id)).Entity;
            _ = dbContext.Messages.Add(new Message("some message", chatUser1AndUser3.Id, user1.Id)).Entity;
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetForChatAsync(chatUser1AndUser2.Id);
            //Assert
            Assert.Single(result);
            Assert.Equal(message1.Id, result.FirstOrDefault().Id);
        }

        [Fact]
        public async Task ThreeMessages_TwoChats_ShouldReturn_TwoMessage()
        {
            //Arrange
            var message1 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user1.Id)).Entity;
            var message2 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user1.Id)).Entity;
            _ = dbContext.Messages.Add(new Message("some message", chatUser1AndUser3.Id, user1.Id)).Entity;
            dbContext.SaveChanges();
            //Act
            var result = await repository.GetForChatAsync(chatUser1AndUser2.Id);
            //Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.Id == message1.Id);
            Assert.Contains(result, m => m.Id == message2.Id);
        }
    }
}
