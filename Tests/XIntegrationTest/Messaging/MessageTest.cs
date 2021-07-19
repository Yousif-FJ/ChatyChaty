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
        [InlineData("a message")]
        public async Task<MessageResponse> SendMessage_Success(string messageBody)
        {
            //Arrange
            var accounts = DataGenerator.Get2AccountTuple();
            var sender = await httpClient.CreateAccount(accounts.Item1);
            var receiver = await httpClient.CreateAccount(accounts.Item2);

            var chat = await httpClient.CreateChat(sender.Token, receiver.Profile.Username);

            //Act
            var result = await httpClient.SendMessage(sender, chat.ChatId, messageBody);

            //Assert

            Assert.False(result.Delivered);
            Assert.Equal(messageBody, result.Body);
            Assert.Equal(chat.ChatId, result.ChatId);

            return result;
        }
    }
}
