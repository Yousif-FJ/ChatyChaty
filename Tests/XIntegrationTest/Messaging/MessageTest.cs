using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using ChatyChaty.HttpShemas.v1.Message;
using ChatyChaty.HttpShemas.v1.Profile;
using Microsoft.AspNetCore.Mvc.Testing;
using ChatyChaty;
using ChatyChaty.HttpShemas.v1.Authentication;
using XIntegrationTest.Profile;
using XIntegrationTest.BaseConfiguration;
using XIntegrationTest.Messaging;

namespace XIntegrationTest
{
    public class MessageTest : IntegrationTestBase
    {
        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetSenderReceiverAndMessageBody), MemberType = typeof(DataGenerator))]
        public async Task<MessageResponse> SendMessage_Success(CreateAccountSchema senderSchem, CreateAccountSchema receiverSchem, string messageBody)
        {
            //Arrange
            var sender = await httpClient.CreateAccount(senderSchem);
            var receiver = await httpClient.CreateAccount(receiverSchem);

            var chat = await httpClient.CreateChat(sender, receiver);

            //Act
            var result = await httpClient.SendMessage(sender, chat, messageBody);

            //Assert

            Assert.False(result.Delivered);
            Assert.Equal(messageBody, result.Body);
            Assert.Equal(chat.ChatId, result.ChatId);

            return result;
        }
    }
}
