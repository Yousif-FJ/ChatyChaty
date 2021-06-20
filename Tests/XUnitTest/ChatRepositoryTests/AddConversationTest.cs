using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.ChatRepositoryTests
{
    public class AddConversationTest : ChatRepositoryTestBase
    {
        public AddConversationTest()
        {
        }

        [Fact]
        public async Task AddConeversation_successfully()
        {
            //Arrange
            var conversation = new Conversation(users[0].Id, users[1].Id);
            //Act
            await ChatRepository.AddAsync(conversation);
            //Assert
            var dbConversation = Context.Conversations.Find(conversation.Id);
            Assert.Equal(conversation.Id, dbConversation.Id);
        }
    }
}
