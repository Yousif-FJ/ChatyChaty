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
using ChatyChatyClient.Actions.Request.Authentication;
using ChatyChatyClient.Entities;
using ChatyChatyClient.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatyChatyClient.Actions.Handler.Authentication
{
    public class SignUpHandler : AuthenticationActionHandlerBase, IRequestHandler<SignUpRequest, AuthenticationResult>
    {
        private static readonly string SignupURL = "/api/v1/Authentication/NewAccount";

        public SignUpHandler(HttpClient httpClient,
            IAuthenticationRepository authenticationRepository,
            IProfileRepository profileRepository,
            ILogger<SignUpHandler> logger)
            : base(httpClient, authenticationRepository, profileRepository, logger) { }

        public async Task<AuthenticationResult> Handle(SignUpRequest request, CancellationToken cancellationToken)
        {
            var signUpInfo = new CreateAccountSchema() { Password = request.Password, Username = request.Username, DisplayName = request.DisplayName };
            var httpResponse = await httpClient.PostAsJsonAsync(SignupURL, signUpInfo, cancellationToken);


            AuthResponse response;
            try
            {
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
                    return new AuthenticationResult(false, errorResponse.Errors.FirstOrDefault());
                }

                response = await httpResponse.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
            }
            catch (Exception e) when (e is NotSupportedException || e is JsonException)
            {
                logger.LogError(e, "Error while reading Response");
                return new AuthenticationResult(false, "Error at the server");
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
