using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Message;
using ChatyChaty.HttpShemas.v1.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;

namespace XIntegrationTest.Messaging
{
    public static class MessageExtension
    {
        public static async Task<MessageResponse> SendMessage(this HttpClient httpClient, AuthResponse senderSchem, string chatId, string messageBody)
        {
            httpClient.AddAuthTokenToHeader(senderSchem.Token);

            var response = await httpClient.PostAsJsonAsync("api/v1/Message/Message",
                new SendMessageSchema(chatId, messageBody));

            return await response.CustomRead200Response<MessageResponse>();
        }
    }
}
