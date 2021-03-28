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
    public record LoginResult(bool IsSuccessful, string Error);

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

    public class LoginHandler : IRequestHandler<Login, LoginResult>
    {
        private readonly HttpClient httpClient;
        private readonly IAuthenticationRepository authenticationRepository;
        private static readonly string LoginURL = "/api/v3/Authentication/Account";

        public LoginHandler(HttpClient httpClient, IAuthenticationRepository authenticationRepository)
        {
            this.httpClient = httpClient;
            this.authenticationRepository = authenticationRepository;
        }

        public async Task<LoginResult> Handle(Login request, CancellationToken cancellationToken)
        {
            if (IsInputNotValid(request,out string error ))
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

            authenticationRepository.SetToken(response.Data.Token);
            return new LoginResult(true, null);
        }

        private static bool IsInputNotValid(Login request, out string errors)
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
}
