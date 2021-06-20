using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.MessageRepositoryTests
{
    public class AddMessageTest : MessageRepositoryTestBase
    {
        [Fact]
        public async Task Success()
        {
            //Arrange
            var messageBody = "Some message";
            dbContext.SaveChanges();
            var message = new Message(messageBody, chatUser1AndUser2.Id, user1.Id);
            //Act
            var returnMessage = await repository.AddAsync(message);
            //Assert
            var dbMessage = dbContext.Messages.Find(returnMessage.Id);
            Assert.True(dbMessage.SenderId == user1.Id);
            Assert.True(dbMessage.ConversationId == chatUser1AndUser2.Id);
            Assert.True(dbMessage.Body == messageBody);
        }

    }
}
