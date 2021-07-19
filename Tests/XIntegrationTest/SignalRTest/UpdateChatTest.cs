using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;
using XIntegrationTest.Profile;
using System.Threading;

namespace XIntegrationTest.SignalRTest
{
    public class UpdateChatTest : SignalRTestBase
    {
        private const string UpdateMethodName = "UpdateChat";

        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetSenderAndReceiver), MemberType = typeof(DataGenerator))]
        public async Task ChatCreate_UpdateChat_ShouldReturn_1Chat(CreateAccountSchema sender, CreateAccountSchema receiver)
        {
            //Arrange
            var senderResponse = await httpClient.CreateAccount(sender);
            var receiverResponse = await httpClient.CreateAccount(receiver);

            var hubConnection = CreateHubConnection(receiverResponse.Token);
            await hubConnection.StartAsync();

            UserProfileResponse updateResponse = default;
            CancellationTokenSource cancellationSource = new();
            var delayTask = Task.Delay(1000, cancellationSource.Token);


            Action<UserProfileResponse> UpdateChatHanlder = (UserProfileResponse u) =>
            {
                updateResponse = u;
                cancellationSource.Cancel();
            };

            //Act
            hubConnection.On<UserProfileResponse>(UpdateMethodName, UpdateChatHanlder);
            var chat = await httpClient.CreateChat(senderResponse.Token, receiverResponse.Profile.Username);

            //Assert
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await delayTask);
            Assert.NotNull(updateResponse);
            Assert.Equal(chat.ChatId, updateResponse.ChatId);
        }
    }
}
