using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;
using Xunit;

namespace XIntegrationTest
{
    public class AuthenticationTest : IntegrationTestBase
    {
        [Theory]
        [MemberData(memberName:nameof(DataGenerator.GetAccount),MemberType =typeof(DataGenerator))]
        public async Task<AuthResponse> CreateAccount_Success(CreateAccountSchema account)
        {
            //Arrange

            //Act
            var result = await httpClient.CreateAccount(account);
            //Assert
            Assert.Equal(account.Username, result.Profile.Username);
            Assert.Null(result.Profile.PhotoURL);
            Assert.NotNull(result.Token);

            return result;
        }

        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetAccount), MemberType = typeof(DataGenerator))]
        public async Task CreateAccount_Fail_UsernameInUse(CreateAccountSchema account)
        {
            //Arrange
            var _ = await httpClient.CreateAccount(account);
            //Act
            var exception = await Assert.ThrowsAsync<IntegrationTestException>(
                async () =>
                {
                    await httpClient.CreateAccount(account);
                });


            //Assert
            var result = await exception.ReadHttpError();
            Assert.NotEqual(HttpStatusCode.OK, exception.HttpResponse.StatusCode);
            Assert.NotEmpty(result.Errors);
        }


        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetAccount), MemberType = typeof(DataGenerator))]
        public async Task Authentication_Successful(CreateAccountSchema account)
        {
            //Arrange
            var result = await httpClient.CreateAccount(account);

            httpClient.AddAuthTokenToHeader(result.Token);
            //Act
            var response = await httpClient.GetAsync("/api/v1/message/NewMessages");
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Authentication_Unsuccessful()
        {
            //Arrange
            //Act
            var response = await httpClient.GetAsync("/api/v1/message/NewMessages");
            //Assert
            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
