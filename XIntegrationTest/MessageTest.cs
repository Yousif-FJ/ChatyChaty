using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using ChatyChaty.ControllerHubSchema.v3;

namespace XIntegrationTest
{
    public class MessageTest : IntegrationTestBase
    {
        [Fact]
        public async Task CreateChat()
        {
            //Arrange
            var user1CreationResponse = await client.CreateAccount("user1", "A name", "veryGoodPassword123");
            var user2CreationResponse = await client.CreateAccount("user2", "A name", "veryGoodPassword123");
            client.Authenticate(user1CreationResponse.Data.Token);
            //Act
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v3/Profile/User");
            requestMessage.Headers.Add("UserName", user2CreationResponse.Data.Profile.Username);
            var response = await client.SendAsync(requestMessage);
            //Assert
            var result = await response.Content.ReadAsAsync<Response<UserProfileResponseBase>>();
            Assert.True(result.Success, $"response as string : { await response.Content.ReadAsStringAsync()}");
            Assert.NotNull(result.Data.ChatId);
            Assert.False(result.Data.ChatId == default(long));
            Assert.Equal(user2CreationResponse.Data.Profile.Username, result.Data.Profile.Username);
        }

        [Fact]
        public async Task SendMessage()
        {
            //Arrange
            var user1CreationResponse = await client.CreateAccount("user3", "A name", "veryGoodPassword123");
            var user2CreationResponse = await client.CreateAccount("user4", "A name", "veryGoodPassword123");
            client.Authenticate(user1CreationResponse.Data.Token);
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v3/Profile/User");
            requestMessage.Headers.Add("UserName", user2CreationResponse.Data.Profile.Username);
            var chatResposne = await (await client.SendAsync(requestMessage)).Content.ReadAsAsync<Response<UserProfileResponseBase>>();
            var chatId = chatResposne.Data.ChatId.Value;
            var messageBody = "hi";
            //Act
            var response = await client.PostAsJsonAsync("api/v3/Message/Message",
                new SendMessageSchema { Body = messageBody, ChatId = chatId });
            //Assert
            var result = await response.Content.ReadAsAsync<Response<MessageInfoReponseBase>>();
            Assert.True(result.Success, $"response as string : { await response.Content.ReadAsStringAsync()}");
            Assert.False(result.Data.Delivered);
            Assert.Equal(messageBody, result.Data.Body);
            Assert.Equal(chatId, result.Data.ChatId);
        }
    }
}
