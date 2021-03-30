using ChatyChatyClient.Entities;
using ChatyChatyClient.HttpSchemas;
using ChatyChatyClient.HttpSchemas.Authentication;
using ChatyChatyClient.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChatyClient.Actions.Authentication
{
    public class SignUp : IRequest<SignUpResult>
    {
        public SignUp(string username, string password, string displayName)
        {
            Username = username;
            Password = password;
            DisplayName = displayName;
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }

    public class SignUpHandler : AuthenticationActionHandlerBase, IRequestHandler<SignUp, SignUpResult>
    {
        private static readonly string SignupURL = "/api/v3/Authentication/NewAccount";

        public SignUpHandler(HttpClient httpClient, IAuthenticationRepository authenticationRepository, IProfileRepository profileRepository)
            : base(httpClient, authenticationRepository, profileRepository){}

        public async Task<SignUpResult> Handle(SignUp request, CancellationToken cancellationToken)
        {
            if (IsInvalidInput(request, out string error))
            {
                return new SignUpResult(false, error);
            }

            var signUpInfo = new CreateAccountSchema() { Password = request.Password, Username = request.Username, DisplayName = request.DisplayName };
            var httpResponse = await httpClient.PostAsJsonAsync(SignupURL, signUpInfo, cancellationToken);
            var response = await httpResponse.ReadAppResponseDataAs<AuthResponseBase>(cancellationToken);


            if (response.Success == false)
            {
                return new SignUpResult(false, response.Errors.FirstOrDefault());
            }

            await authenticationRepository.SetToken(response.Data.Token);
            await profileRepository.Set(
                new UserProfile(
                    response.Data.Profile.Username,
                    response.Data.Profile.DisplayName,
                    response.Data.Profile.PhotoURL
             ));

            return new SignUpResult(true, null);
        }

        private static bool IsInvalidInput(SignUp request, out string errors)
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
            if (string.IsNullOrWhiteSpace(request.DisplayName))
            {
                errors = $"{nameof(request.DisplayName)} can't be empty";
                return true;
            }
            errors = default;
            return false;
        }
    }

    public record SignUpResult(bool IsSuccessful, string Error);
}
