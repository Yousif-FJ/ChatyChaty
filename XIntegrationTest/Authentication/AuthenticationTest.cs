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
            Assert.Equal(username, result.Profile.Username);
            Assert.Null(result.Profile.PhotoURL);
            Assert.NotNull(result.Token);
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
            client.Authenticate(createResponse.Token);
            //Act
            var response = await client.GetAsync("/api/v1/message/NewMessages");
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
