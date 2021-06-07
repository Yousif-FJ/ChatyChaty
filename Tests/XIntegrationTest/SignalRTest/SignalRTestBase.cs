using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
