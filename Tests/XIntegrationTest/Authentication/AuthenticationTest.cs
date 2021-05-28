using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace XIntegrationTest
{
    public class AuthenticationTest : IntegrationTestBase
    {
        [Fact]
        public async Task NotAuthenticate_Succesfully()
        {
            //Arrange

            //Act
            var response = await client.GetAsync("/api/v1/message/NewMessages");
            //Assert
            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        }


        [Theory]
        [InlineData("name", "originalPassword123", "something funny")]
        public async Task<AuthResponse> CreateAccount_Success(string username, string password,string displayName)
        {
            //Arrange

            //Act
            var result = await client.CreateAccount(username, displayName, password);
            //Assert
            Assert.Equal(username, result.Profile.Username);
            Assert.Null(result.Profile.PhotoURL);
            Assert.NotNull(result.Token);

            return result;
        }

        [Theory]
        [InlineData("name", "originalPassword123", "something funny")]
        public async Task CreateAccount_Fail_UsernameInUse(string username, string password, string displayName)
        {
            //Arrange

            //Act
            var result1 = await client.CreateAccount(username, displayName, password);

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Authentication/NewAccount", new CreateAccountSchema
            {
                DisplayName = displayName,
                Password = password,
                Username = username
            });

            var result = await CustomReadResponse<ErrorResponse>(response);

            //Assert
            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task Authenticate_Success()
        {
            //Arrange
            var createResult = await CreateAccount_Success("name", "originalPassword123", "something funny");

            client.AddAuthTokenToHeader(createResult.Token);
            //Act
            var response = await client.GetAsync("/api/v1/message/NewMessages");
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
