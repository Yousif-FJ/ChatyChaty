using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using ChatyChaty.ControllerHubSchema.v1;

namespace XIntegrationTest
{
    public class MessageTest : IntegrationTestBase
    {
        [Fact]
        public async Task CreateChat()
        {
            //Arrange
            var user2CreationResponse = await client.CreateAccount("user2", "A name", "veryGoodPassword123");
            //var user1CreationResponse = await client.CreateAccount("user1", "A name", "veryGoodPassword123");
            var user3CreationResponse = await client.CreateAccount("user3", "A name", "veryGoodPassword123");
            client.Authenticate(user3CreationResponse.Token);
            //Act
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v1/Profile/User");
            requestMessage.Headers.Add("UserName", user2CreationResponse.Profile.Username);
            var response = await client.SendAsync(requestMessage);
            //Assert
            UserProfileResponse result;
            try
            {
                 result = await response.Content.ReadAsAsync<UserProfileResponse>();
            }
            catch (UnsupportedMediaTypeException)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Unsupported type reponse, The response as string : {responseAsString}");
            }

            Assert.NotNull(result.ChatId);
            Assert.Equal(user2CreationResponse.Profile.Username, result.Profile.Username);
        }

        [Fact]
        public async Task SendMessage()
        {
            //Arrange
            var user1CreationResponse = await client.CreateAccount("user3", "A name", "veryGoodPassword123");
            var user2CreationResponse = await client.CreateAccount("user4", "A name", "veryGoodPassword123");
            client.Authenticate(user1CreationResponse.Token);
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v1/Profile/User");
            requestMessage.Headers.Add("UserName", user2CreationResponse.Profile.Username);
            var chatResposne = await (await client.SendAsync(requestMessage)).Content.ReadAsAsync<UserProfileResponse>();
            var chatId = chatResposne.ChatId;
            var messageBody = "hi";
            //Act
            var response = await client.PostAsJsonAsync("api/v1/Message/Message",
                new SendMessageSchema { Body = messageBody, ChatId = chatId });

            //Assert
            MessageInfoReponse result;
            try
            {
                result = await response.Content.ReadAsAsync<MessageInfoReponse>();
            }
            catch (UnsupportedMediaTypeException)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Unsupported type response, The response as string : {responseAsString}");
            }

            Assert.False(result.Delivered);
            Assert.Equal(messageBody, result.Body);
            Assert.Equal(chatId, result.ChatId);
        }
    }
}
