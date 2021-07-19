using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;

namespace XIntegrationTest.Profile
{
    public static class ProfileExtention
    {
        public static async Task<UserProfileResponse> CreateChat(this HttpClient httpClient, string token, string receiverUsername)
        {
            httpClient.AddAuthTokenToHeader(token);

            var response = await httpClient.GetAsync($"api/v1/Profile/User?UserName={receiverUsername}");

            return await response.CustomRead200Response<UserProfileResponse>();
        }
    }
}
