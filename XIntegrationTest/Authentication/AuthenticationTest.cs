using ChatyChaty.ControllerHubSchema.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace XIntegrationTest
{
    public class AuthenticationTest : IntegrationTestBase
    {

        [Fact]
        public async Task CreateAccount_Successfully()
        {
            //Arrange
            var username = "name";
            var password = "originalPassword123";
            var displayName = "something funny";
            //Act
            var result = await client.CreateAccount(username, displayName, password);
            //Assert
            Assert.True(result.Success);
            Assert.Equal(username, result.Data.Profile.Username);
            Assert.Null(result.Data.Profile.PhotoURL);
            Assert.NotNull(result.Data.Token);
        }

        [Fact]
        public async Task Authenticate_Succesfully()
        {
            //Arrange
            var username = "name1";
            var password = "originalPassword123";
            var displayName = "something funny";
            var createResponse = await client.CreateAccount(username, displayName, password);
            //add token to header
            client.Authenticate(createResponse.Data.Token);
            //Act
            var response = await client.GetAsync("/api/v3/message/NewMessages");
            var textResponse = await response.Content.ReadAsStringAsync();
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
