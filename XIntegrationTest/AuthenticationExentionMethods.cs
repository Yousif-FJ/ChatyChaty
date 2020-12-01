using ChatyChaty.ControllerHubSchema.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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
        public static void Authenticate(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        public async static Task<Response<AuthenticationResponseBase>> CreateAccount(this HttpClient httpClient,string userName, string displayName, string password)
        {

            var response = await httpClient.PostAsJsonAsync("/api/v3/Authentication/NewAccount", new CreateAccountSchema
            {
                DisplayName = displayName, Password = password, Username = userName
            });
            Response<AuthenticationResponseBase> responseAsClass;
            responseAsClass = await response.Content.ReadAsAsync<Response<AuthenticationResponseBase>>();

            if (responseAsClass.Success == false)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error, The response as string is : {responseAsString}");
            }
            return responseAsClass;
        }
    }
}
