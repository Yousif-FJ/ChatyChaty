using ChatyChaty.HttpShemas.v1.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace XIntegrationTest
{
    public static class AuthenticationExentionMethods
    {
        /// <summary>
        /// Add token to the header of the client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="token"></param>
        public static void AddAuthTokenToHeader(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        public async static Task<AuthResponse> CreateAccount(this HttpClient httpClient,string userName, string displayName, string password)
        {

            HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/v1/Authentication/NewAccount", new CreateAccountSchema
            {
                DisplayName = displayName, Password = password, Username = userName
            });

            var result = await IntegrationTestBase.CustomReadResponse<AuthResponse>(response);

            return result;
        }
    }
}
