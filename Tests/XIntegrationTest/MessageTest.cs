using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using ChatyChaty.HttpShemas.v1.Message;
using ChatyChaty.HttpShemas.v1.Profile;

namespace XIntegrationTest
{
    public class MessageTest : IntegrationTestBase
    {
        [Fact]
        public async Task<UserProfileResponse> CreateChat()
        {
            //Arrange
            var user2CreationResponse = await client.CreateAccount("user2", "A name", "veryGoodPassword123");
            //var user1CreationResponse = await client.CreateAccount("user1", "A name", "veryGoodPassword123");
            var user3CreationResponse = await client.CreateAccount("user3", "A name", "veryGoodPassword123");
            client.Authenticate(user3CreationResponse.Token);
            //Act
            var response = await client.GetAsync($"api/v1/Profile/User?UserName={user2CreationResponse.Profile.Username}");
            //Assert

            var result = await response.Content.ReadAsAsync<UserProfileResponse>();


            if (HttpStatusCode.OK != response.StatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Not Ok 200, The response as string : {responseAsString}");
            }

            Assert.NotNull(result.ChatId);
            Assert.Equal(user2CreationResponse.Profile.Username, result.Profile.Username);
            return result;
        }

        [Fact]
        public async Task SendMessage()
        {
            //Arrange
            var user1CreationResponse = await client.CreateAccount("user3", "A name", "veryGoodPassword123");
            var user2CreationResponse = await client.CreateAccount("user4", "A name", "veryGoodPassword123");
            client.Authenticate(user1CreationResponse.Token);

            var chatHttpResposne = await client.GetAsync($"api/v1/Profile/User?UserName={user2CreationResponse.Profile.Username}");
            var chatResposne = await chatHttpResposne.Content.ReadAsAsync<UserProfileResponse>();

            var chatId = chatResposne.ChatId;
            var messageBody = "hi";
            //Act
            var response = await client.PostAsJsonAsync("api/v1/Message/Message",
                new SendMessageSchema { Body = messageBody, ChatId = chatId });
            //Assert

            var result = await response.Content.ReadAsAsync<MessageInfoReponse>();


            if (HttpStatusCode.OK != response.StatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Not Ok 200, The response as string : {responseAsString}");
            }

            Assert.False(result.Delivered);
            Assert.Equal(messageBody, result.Body);
            Assert.Equal(chatId, result.ChatId);
        }
    }
}
