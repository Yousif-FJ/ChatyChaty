using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.EntityTests
{
    public class ConversationTests
    {
        [Fact]
        public void Conversation_Construtor()
        {
            //Arrange
            var user1 = new AppUser("user1");
            var user2 = new AppUser("user2");
            //Act
            var conversation = new Conversation(user1.Id,user2.Id);
            //Assert
            Assert.Equal(conversation.FirstUserId, user1.Id);
            Assert.Equal(conversation.SecondUserId, user2.Id);
            Assert.NotNull(conversation.Id);
            Assert.NotEqual(conversation.Id.Value, new Guid().ToString());
            Assert.Null(conversation.Messages);
            Assert.Null(conversation.SecondUser);
            Assert.Null(conversation.FirstUser);
        }

    }
}
