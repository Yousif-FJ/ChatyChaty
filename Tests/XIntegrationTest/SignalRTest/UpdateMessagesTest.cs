using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;
using Microsoft.AspNetCore.SignalR.Client;
using XIntegrationTest.Profile;
using Xunit;
using ChatyChaty.HttpShemas.v1.Message;
using XIntegrationTest.Messaging;

namespace XIntegrationTest.SignalRTest
{
    public class UpdateMessagesTest : SignalRTestBase
    {
        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetAccount), MemberType = typeof(DataGenerator))]
        public async Task Connect_WithoutException(CreateAccountSchema account)
        {
            var user1 = await httpClient.CreateAccount(account);

            var connection = CreateHubConnection(user1.Token);

            await connection.StartAsync();
        }


        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetSenderAndReceiver), MemberType = typeof(DataGenerator))]
        public async Task SendMessage_MessageUpdate_ShouldReturn_1Message(CreateAccountSchema sender, CreateAccountSchema receiver)
        {
            //Arrange
            var senderResponse = await httpClient.CreateAccount(sender);
            var receiverResponse = await httpClient.CreateAccount(receiver);
            var chat = await httpClient.CreateChat(senderResponse.Token, receiverResponse.Profile.Username);

            var hubConnection = CreateHubConnection(receiverResponse.Token);
            await hubConnection.StartAsync();

            IEnumerable<MessageResponse> updateMessage = default;
            CancellationTokenSource cancellationSource = new();
            var delayTask = Task.Delay(1000, cancellationSource.Token);

            //Act
            hubConnection.On<IEnumerable<MessageResponse>> ("UpdateMessages",
                u => {
                    updateMessage = u;
                    cancellationSource.Cancel();
                });
            var message = httpClient.SendMessage(senderResponse, chat.ChatId, "some message");
            //Assert
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await delayTask);
            Assert.NotNull(updateMessage);
        }
    }
}
