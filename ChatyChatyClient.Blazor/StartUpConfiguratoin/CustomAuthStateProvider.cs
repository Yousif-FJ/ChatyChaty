using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ChatyChatyClient.Logic.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatyChatyClient.Blazor.StartUpConfiguratoin
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IProfileRepository profileRepository;
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly ILogger logger;

        public CustomAuthStateProvider(IProfileRepository profileRepository, IAuthenticationRepository authenticationRepository, ILogger<CustomAuthStateProvider> logger)
        {
            this.profileRepository = profileRepository;
            this.authenticationRepository = authenticationRepository;
            this.logger = logger;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if ((await authenticationRepository.IsAuthenticated()) == false)
            {
                logger.LogInformation("user not authenticated");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var profile = await profileRepository.Get();

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, profile.DisplayName),
                new Claim(ClaimTypes.NameIdentifier, profile.Username),
            }, "bearer");

            var user = new ClaimsPrincipal(identity);

            logger.LogInformation("user authenticated");
            return new AuthenticationState(user);
        }

        

        public void UpdateAuthState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
