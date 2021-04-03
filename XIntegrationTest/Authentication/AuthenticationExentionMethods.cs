using ChatyChaty.ControllerHubSchema.v1;
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

        public async static Task<Response<AuthResponseBase>> CreateAccount(this HttpClient httpClient,string userName, string displayName, string password)
        {

            var response = await httpClient.PostAsJsonAsync("/api/v3/Authentication/NewAccount", new CreateAccountSchema
            {
                DisplayName = displayName, Password = password, Username = userName
            });
            Response<AuthResponseBase> responseAsClass;
            try
            {
                responseAsClass = await response.Content.ReadAsAsync<Response<AuthResponseBase>>();
            }
            catch (UnsupportedMediaTypeException)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                throw new Exception($"Unsupported type response, The response as string : {responseAsString}");
            }

            return responseAsClass;
        }
    }
}
