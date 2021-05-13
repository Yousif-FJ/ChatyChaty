using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChaty.HttpShemas.v1.Error;
using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Logic.AppExceptions;
using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatyChatyClient.Logic.Actions.Handler.Authentication
{
    public class LoginHandler : AuthenticationActionHandlerBase, IRequestHandler<LoginRequest, AuthenticationResult>
    {
        private const string LoginURL = "/api/v1/Authentication/Account";

        public LoginHandler(HttpClient httpClient,
            IAuthenticationRepository authenticationRepository,
            IProfileRepository profileRepository,
            ILogger<LoginHandler> logger)
            : base(httpClient, authenticationRepository, profileRepository, logger) { }

        public async Task<AuthenticationResult> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var loginInfo = new LoginAccountSchema() { Password = request.Password, Username = request.Username };
            var httpResponse = await httpClient.PostAsJsonAsync(LoginURL, loginInfo, cancellationToken);


            AuthResponse response;
            try
            {
                response = await httpResponse.ReadApplicatoinResponse<AuthResponse>(cancellationToken);
            }
            catch (ErrorResponseException e)
            {
                return new AuthenticationResult(false, e.Message);
            }


            await authenticationRepository.SetToken(response.Token);
            await profileRepository.Set(
                new UserProfile(
                    response.Profile.Username,
                    response.Profile.DisplayName,
                    response.Profile.PhotoURL
                ));

            return new AuthenticationResult(true, null);
        }
    }
}
