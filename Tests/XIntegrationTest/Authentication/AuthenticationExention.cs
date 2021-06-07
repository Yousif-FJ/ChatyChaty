using ChatyChaty.HttpShemas.v1.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;

namespace XIntegrationTest
{
    public static class AuthenticationExention
    {
        public static void AddAuthTokenToHeader(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        public async static Task<AuthResponse> Login(this HttpClient httpClient, LoginAccountSchema account)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/v1/Authentication/Account", account);

            var result = await response.CustomRead200Response<AuthResponse>();

            return result;
        }

        public async static Task<AuthResponse> CreateAccount(this HttpClient httpClient, CreateAccountSchema account)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/v1/Authentication/NewAccount", account);

            var result = await response.CustomRead200Response<AuthResponse>();

            return result;
        }
    }
}
