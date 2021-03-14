using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.MessageRepositoryTests
{
    public class AddMessageTest : BaseMessageRepositoryTest
    {
        [Fact]
        public async Task Success()
        {
            //Arrange
            var messageBody = "Some message";
            var chat = dbContext.Conversations.Add(new Conversation(user1.Id, user2.Id)).Entity;
            dbContext.SaveChanges();
            var message = new Message(messageBody, chat.Id, user1.Id);
            //Act
            var returnMessage = await repository.AddMessageAsync(message);
            //Assert
            var dbMessage = dbContext.Messages.Find(returnMessage.Id);
            Assert.True(dbMessage.SenderId == user1.Id);
            Assert.True(dbMessage.ConversationId == chat.Id);
            Assert.True(dbMessage.Body == messageBody);
        }

    }
}
