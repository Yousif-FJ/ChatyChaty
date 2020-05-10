using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using ChatyChaty.ControllerSchema.v2;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.ControllerSchema.v1;

namespace XIntegrationTest
{
    public class MessageController : IntegrationTestBase
    {
        [Fact]
        public async Task CreateUser_Startconversation_SendMessage_Check_Receive()
        {
            //Creating Users

            var User1Token = await CreateAccount(userName: "user1", displayName: "Test user", password: "a12345678");
            var User2Token = await CreateAccount(userName: "user2", displayName: "Test user", password: "a12345678");

            //Sign in as user1
            Authenticate(User1Token);

            //Check For updates
            var UpdateCheckResponse = await client.GetAsync("api/v2/Notification/CheckForUpdates");
            var UpdateCheck = await UpdateCheckResponse.Content.ReadAsAsync<CheckForUpdatesResponse>();

            Assert.False(UpdateCheck.MessageUpdate && UpdateCheck.ChatUpdate);


            //Create a Chat 
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v1/Profile/GetUser");
            requestMessage.Headers.Add("UserName", "user2");
            var GetUserResult = await client.SendAsync(requestMessage);


            //Create a message 
            var MessageSend = new SendMessageSchema
            {
                Body = "Test Message",
                ChatId = (await GetUserResult.Content.ReadAsAsync<GetUserProfileResponse>()).ChatId.Value
            };

            //Send a message
            var PostMessageResponse = await client.PostAsJsonAsync("api/v2/Message/SendMessage",MessageSend );
            var PostMessageResponseText = await PostMessageResponse.Content.ReadAsStringAsync();

            //Login in as user2
            Authenticate(User2Token);

            UpdateCheckResponse = await client.GetAsync("api/v2/Notification/CheckForUpdates");
            UpdateCheck = await UpdateCheckResponse.Content.ReadAsAsync<CheckForUpdatesResponse>();
            Assert.True(UpdateCheck.MessageUpdate && UpdateCheck.ChatUpdate);

            using HttpRequestMessage requestMessage2 = new HttpRequestMessage(HttpMethod.Get, "api/v2/Message/GetNewMessages");
            requestMessage2.Headers.Add("LastMessageId", "0");
            var GetNewMessageResponse = await client.SendAsync(requestMessage2);

            var GetNewMessage = await GetNewMessageResponse.Content.ReadAsAsync<IEnumerable<NewMessagesResponse>>();
            Assert.True(GetNewMessage.First().Body == MessageSend.Body);
        }

        [Fact]
        public async Task IsWorking()
        {
            var result = await client.GetAsync("");
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
