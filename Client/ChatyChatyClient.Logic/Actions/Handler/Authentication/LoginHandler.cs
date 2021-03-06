﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatyChaty.HttpShemas.v1.Authentication;
using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Logic.AppExceptions;
using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatyChatyClient.Logic.Actions.Handler.Authentication
{
    public class LoginHandler : AuthHandlerBase, IRequestHandler<LoginRequest, AuthenticationResult>
    {
        private const string LoginURL = "/api/v1/Authentication/Account";

        public LoginHandler(HttpClient httpClient,
            IAuthenticationRepository authenticationRepository,
            ISelfProfileRepository profileRepository,
            ILogger<LoginHandler> logger)
            : base(httpClient, authenticationRepository, profileRepository, logger) { }

        public async Task<AuthenticationResult> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var loginInfo = new LoginAccountSchema(request.Username, request.Password);
            var httpResponse = await httpClient.PostAsJsonAsync(LoginURL, loginInfo, cancellationToken);


            AuthResponse response;
            try
            {
                response = await httpResponse.ReadAppResponseAsync<AuthResponse>(cancellationToken);
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
            logger.LogInformation("Login completed");
            return new AuthenticationResult(true, null);
        }
    }
}
