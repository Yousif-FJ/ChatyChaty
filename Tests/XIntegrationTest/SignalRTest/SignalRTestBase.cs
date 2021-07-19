using ChatyChaty.HttpShemas.v1.Authentication;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XIntegrationTest.SignalRTest
{
    public abstract class SignalRTestBase : IntegrationTestBase
    {
        protected HubConnection CreateHubConnection(string token)
        {
            return new HubConnectionBuilder()
                .WithUrl($"{Factory.Server.BaseAddress.AbsoluteUri}v1/chathub",
                o =>
                {
                    o.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();
                    o.AccessTokenProvider = () => Task<string>.Factory.StartNew(() => token);
                })
                .Build();
        }

        protected async Task SetupHubTest<T>(CreateAccountSchema sender, CreateAccountSchema receiver, string UpdateMethodName, Action<T> UpdateHanlder)
        {
            var senderResponse = await httpClient.CreateAccount(sender);
            var receiverResponse = await httpClient.CreateAccount(receiver);

            var hubConnection = CreateHubConnection(receiverResponse.Token);
            await hubConnection.StartAsync();

            CancellationTokenSource cancellationSource = new();
            var delayTask = Task.Delay(1000, cancellationSource.Token);

            hubConnection.On<T>(UpdateMethodName, UpdateHanlder);
        }
    }
}
