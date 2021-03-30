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
    public class Login : IRequest<LoginResult>
    {
        public Login(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginHandler : AuthenticationActionHandlerBase ,IRequestHandler<Login, LoginResult>
    {
        private static readonly string LoginURL = "/api/v3/Authentication/Account";

        public LoginHandler(HttpClient httpClient, IAuthenticationRepository authenticationRepository, IProfileRepository profileRepository)
            : base(httpClient, authenticationRepository, profileRepository) { }

        public async Task<LoginResult> Handle(Login request, CancellationToken cancellationToken)
        {
            if (IsInValidInput(request,out string error ))
            {
                return new LoginResult(false, error);
            }

            var loginInfo = new LoginAccountSchema() { Password = request.Password, Username = request.Username };
            var httpResponse = await httpClient.PostAsJsonAsync(LoginURL, loginInfo, cancellationToken);
            var response = await httpResponse.ReadAppResponseDataAs<AuthResponseBase>(cancellationToken);


            if (response.Success == false)
            {
                return new LoginResult(false, response.Errors.FirstOrDefault());
            }

            await authenticationRepository.SetToken(response.Data.Token);
            await profileRepository.Set(
                new UserProfile(
                    response.Data.Profile.Username,
                    response.Data.Profile.DisplayName,
                    response.Data.Profile.PhotoURL
                ));

            return new LoginResult(true, null);
        }

        private static bool IsInValidInput(Login request, out string errors)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                errors = $"{nameof(request.Username)} can't be empty";
                return true;
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors = $"{nameof(request.Password)} can't be empty" ;
                return true;
            }
            errors = default;
            return false;
        }
    }

    public record LoginResult(bool IsSuccessful, string Error);
}
