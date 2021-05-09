using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ChatyChatyClient.Repository;
using Microsoft.Extensions.Logging;

namespace ChatyChatyClient.Actions.Handler.Authentication
{
    public abstract class AuthenticationActionHandlerBase
    {
        protected readonly HttpClient httpClient;
        protected readonly IAuthenticationRepository authenticationRepository;
        protected readonly IProfileRepository profileRepository;
        protected readonly ILogger<AuthenticationActionHandlerBase> logger;

        protected AuthenticationActionHandlerBase(HttpClient httpClient,
            IAuthenticationRepository authenticationRepository,
            IProfileRepository profileRepository,
            ILogger<AuthenticationActionHandlerBase> logger)
        {
            this.httpClient = httpClient;
            this.authenticationRepository = authenticationRepository;
            this.profileRepository = profileRepository;
            this.logger = logger;
        }
    }
}
