using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using ChatyChaty.HttpShemas.v1.Message;
using ChatyChaty.HttpShemas.v1.Profile;

namespace XIntegrationTest
{
    public class MessageTest : IntegrationTestBase
    {
        [Fact]
        public async Task SendMessage_Success()
        {
            //Arrange
            var user1CreationResponse = await client.CreateAccount("user1", "A name", "veryGoodPassword123");
            var user2CreationResponse = await client.CreateAccount("user2", "A name", "veryGoodPassword123");
            client.AddAuthTokenToHeader(user1CreationResponse.Token);

            var chatHttpResposne = await client.GetAsync($"api/v1/Profile/User?UserName={user2CreationResponse.Profile.Username}");
            var chatResposne = await chatHttpResposne.Content.ReadAsAsync<UserProfileResponse>();

            var chatId = chatResposne.ChatId;
            var messageBody = "hi";
            //Act
            var response = await client.PostAsJsonAsync("api/v1/Message/Message",
                new SendMessageSchema { Body = messageBody, ChatId = chatId });
            //Assert

            var result = await CustomReadResponse<MessageInfoReponse>(response);

            Assert.False(result.Delivered);
            Assert.Equal(messageBody, result.Body);
            Assert.Equal(chatId, result.ChatId);
        }
    }
}
