using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatyChatyClient.Actions.Request.Authentication;
using ChatyChatyClient.Entities;
using ChatyChatyClient.HttpSchemas;
using ChatyChatyClient.HttpSchemas.Authentication;
using ChatyChatyClient.Repository;
using MediatR;

namespace ChatyChatyClient.Actions.Handler.Authentication
{
    public class LoginHandler : AuthenticationActionHandlerBase, IRequestHandler<LoginRequest, AuthenticationResult>
    {
        private static readonly string LoginURL = "/api/v3/Authentication/Account";

        public LoginHandler(HttpClient httpClient, IAuthenticationRepository authenticationRepository, IProfileRepository profileRepository)
            : base(httpClient, authenticationRepository, profileRepository) { }

        public async Task<AuthenticationResult> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            if (IsInValidInput(request, out string error))
            {
                return new AuthenticationResult(false, error);
            }

            var loginInfo = new LoginAccountSchema() { Password = request.Password, Username = request.Username };
            var httpResponse = await httpClient.PostAsJsonAsync(LoginURL, loginInfo, cancellationToken);

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
            catch (NotSupportedException)
            {
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

        private static bool IsInValidInput(LoginRequest request, out string errors)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                errors = $"{nameof(request.Username)} can't be empty";
                return true;
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors = $"{nameof(request.Password)} can't be empty";
                return true;
            }
            errors = default;
            return false;
        }
    }
}
