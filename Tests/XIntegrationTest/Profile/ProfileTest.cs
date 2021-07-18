using ChatyChaty;
using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Error;
using ChatyChaty.HttpShemas.v1.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XIntegrationTest.BaseConfiguration;
using Xunit;

namespace XIntegrationTest.Profile
{
    public class ProfileTest : IntegrationTestBase
    {
        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetSenderAndReceiver), MemberType = typeof(DataGenerator))]
        public async Task<UserProfileResponse> CreateChat_Success(CreateAccountSchema senderSchem, CreateAccountSchema receiverSchem)
        {
            //Arrange
            var sender = await httpClient.CreateAccount(senderSchem);
            var receiver = await httpClient.CreateAccount(receiverSchem);


            //Act
            UserProfileResponse result = await httpClient.CreateChat(sender, receiver);
            //Assert

            Assert.NotNull(result.ChatId);
            Assert.Equal(receiver.Profile.Username, result.Profile.Username);
            return result;
        }

        [Theory]
        [MemberData(memberName: nameof(DataGenerator.GetSenderAndReceiver), MemberType = typeof(DataGenerator))]
        public async Task<ErrorResponse> CreateChat_NotFound(CreateAccountSchema senderSchema, CreateAccountSchema receiver)
        {
            //Arrange
            var sender = await httpClient.CreateAccount(senderSchema);


            //Act
            var exception = await Assert.ThrowsAsync<IntegrationTestException>( 
                async () =>
                {
                    await httpClient.CreateChat(sender, new AuthResponse(null, new ProfileResponse(receiver.Username, receiver.DisplayName, null)));
                }
            );

            //Assert
            var error = await exception.ReadHttpError();

            Assert.NotEqual(HttpStatusCode.OK, exception.HttpResponse.StatusCode);
            Assert.NotEmpty(error.Errors);

            return error;
        }
    }
}
