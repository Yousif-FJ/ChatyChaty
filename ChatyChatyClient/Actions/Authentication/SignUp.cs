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
    public record SignUpResult(bool IsSuccessful, string Error);
    public class SignUp : IRequest<SignUpResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }

    public class SignUpHandler : IRequestHandler<SignUp, SignUpResult>
    {
        private readonly HttpClient httpClient;
        private readonly IAuthenticationRepository authenticationRepository;
        private static readonly string LoginURL = "/api/v3/Authentication/NewAccount";

        public SignUpHandler(HttpClient httpClient, IAuthenticationRepository authenticationRepository)
        {
            this.httpClient = httpClient;
            this.authenticationRepository = authenticationRepository;
        }

        public async Task<SignUpResult> Handle(SignUp request, CancellationToken cancellationToken)
        {
            if (IsInputNotValid(request, out string error))
            {
                return new SignUpResult(false, error);
            }

            var signUpInfo = new CreateAccountSchema() { Password = request.Password, Username = request.Username, DisplayName = request.DisplayName };
            var httpResponse = await httpClient.PostAsJsonAsync(LoginURL, signUpInfo, cancellationToken);

            var response = await httpResponse.ReadAppResponseDataAs<AuthResponseBase>(cancellationToken);


            if (response.Success == false)
            {
                return new SignUpResult(false, response.Errors.FirstOrDefault());
            }

            authenticationRepository.SetToken(response.Data.Token);
            return new SignUpResult(true, null);
        }

        private static bool IsInputNotValid(SignUp request, out string errors)
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
}
