using ChatyChaty.HttpShemas.v1.Error;
using ChatyChaty.HttpShemas.v1.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace XIntegrationTest.Profile
{
    public class ProfileTest : IntegrationTestBase
    {
        [Theory]
        [InlineData("user2")]
        public async Task<UserProfileResponse> CreateChat_Success(string username)
        {
            //Arrange
            var user1CreationResponse = await client.CreateAccount("user3", "A name", "veryGoodPassword123");
            var user2CreationResponse = await client.CreateAccount(username, "A name", "veryGoodPassword123");

            client.AddAuthTokenToHeader(user1CreationResponse.Token);
            //Act
            var response = await client.GetAsync($"api/v1/Profile/User?UserName={username}");
            //Assert

            var result = await CustomReadResponse<UserProfileResponse>(response);

            Assert.NotNull(result.ChatId);
            Assert.Equal(user2CreationResponse.Profile.Username, result.Profile.Username);
            return result;
        }

        [Theory]
        [InlineData("user2")]
        public async Task<ErrorResponse> CreateChat_NotFound(string username)
        {
            //Arrange
            var user1CreationResponse = await client.CreateAccount("user1", "A name", "veryGoodPassword123");

            client.AddAuthTokenToHeader(user1CreationResponse.Token);
            //Act
            var response = await client.GetAsync($"api/v1/Profile/User?UserName={username}");


            //Assert
            var result = await CustomReadResponse<ErrorResponse>(response);

            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(result.Errors);

            return result;
        }
    }
}
