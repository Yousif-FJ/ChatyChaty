using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ChatyChatyClient.Logic.RepositoryInterfaces;
using Microsoft.Extensions.Logging;

namespace ChatyChatyClient.Logic.Actions.Handler.Authentication
{
    public abstract class AuthHandlerBase
    {
        protected readonly HttpClient httpClient;
        protected readonly IAuthenticationRepository authenticationRepository;
        protected readonly ISelfProfileRepository profileRepository;
        protected readonly ILogger<AuthHandlerBase> logger;

        protected AuthHandlerBase(HttpClient httpClient,
            IAuthenticationRepository authenticationRepository,
            ISelfProfileRepository profileRepository,
            ILogger<AuthHandlerBase> logger)
        {
            this.httpClient = httpClient;
            this.authenticationRepository = authenticationRepository;
            this.profileRepository = profileRepository;
            this.logger = logger;
        }
    }
}
