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
using ChatyChatyClient.Logic.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatyChatyClient.Logic.Actions.Handler.Authentication
{
    public class SignUpHandler : AuthHandlerBase, IRequestHandler<SignUpRequest, AuthenticationResult>
    {
        private const string SignupURL = "/api/v1/Authentication/NewAccount";

        public SignUpHandler(HttpClient httpClient,
            IAuthenticationRepository authenticationRepository,
            ISelfProfileRepository profileRepository,
            ILogger<SignUpHandler> logger)
            : base(httpClient, authenticationRepository, profileRepository, logger) { }

        public async Task<AuthenticationResult> Handle(SignUpRequest request, CancellationToken cancellationToken)
        {
            var signUpInfo = new CreateAccountSchema(request.Username, request.Password, request.DisplayName);
            var httpResponse = await httpClient.PostAsJsonAsync(SignupURL, signUpInfo, cancellationToken);


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
            logger.LogInformation("SignUp completed");
            return new AuthenticationResult(true, null);
        }
    }
}
