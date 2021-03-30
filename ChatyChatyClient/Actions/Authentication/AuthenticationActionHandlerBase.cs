using ChatyChatyClient.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChatyChatyClient.Actions.Authentication
{
    public abstract class AuthenticationActionHandlerBase
    {
        protected readonly HttpClient httpClient;
        protected readonly IAuthenticationRepository authenticationRepository;
        protected readonly IProfileRepository profileRepository;
        protected AuthenticationActionHandlerBase(HttpClient httpClient, IAuthenticationRepository authenticationRepository, IProfileRepository profileRepository)
        {
            this.httpClient = httpClient;
            this.authenticationRepository = authenticationRepository;
            this.profileRepository = profileRepository;
        }
    }
}
