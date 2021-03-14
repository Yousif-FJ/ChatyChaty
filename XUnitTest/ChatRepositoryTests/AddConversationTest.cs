using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.ChatRepositoryTests
{
    public class AddConversationTest
    {
        private readonly ChatRepositoryTestKit testKit;
        public AddConversationTest()
        {
            testKit = new ChatRepositoryTestKit();
        }

        [Fact]
        public async Task AddConeversation_successfully()
        {
            //Arrange
            var conversation = new Conversation(testKit.users[0].Id, testKit.users[1].Id);
            //Act
            await testKit.ChatRepository.AddConversationAsync(conversation);
            //Assert
            var dbConversation = testKit.Context.Conversations.Find(conversation.Id);
            Assert.Equal(conversation.Id, dbConversation.Id);
        }
    }
}
