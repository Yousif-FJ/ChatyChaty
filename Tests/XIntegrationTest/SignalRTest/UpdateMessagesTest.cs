using ChatyChaty.HttpShemas.v1.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;
using Xunit;

namespace XIntegrationTest.SignalRTest
{
    public class UpdateMessagesTest : SignalRTestBase
    {
        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetAccount), MemberType = typeof(DataGenerator))]
        public async Task Connect_WithoutException(CreateAccountSchema account)
        {
            var user1 = await client.CreateAccount(account);

            var connection = CreateHubConnection(user1.Token);

            await connection.StartAsync();
        }


/*        public async Task UpdateMessage_OneMessage()
        {
            throw new NotImplementedException();
        }*/
    }
}
