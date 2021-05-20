using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Logic.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Actions.Handler.Authentication
{
    public class LogoutHandler : IRequestHandler<LogoutRequest>
    {
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly ISelfProfileRepository profileRepository;
        private readonly ILogger<LogoutHandler> logger;

        public LogoutHandler(IAuthenticationRepository authenticationRepository, ISelfProfileRepository profileRepository, ILogger<LogoutHandler> logger)
        {
            this.authenticationRepository = authenticationRepository;
            this.profileRepository = profileRepository;
            this.logger = logger;
        }

        public async Task<Unit> Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            await authenticationRepository.RemoveToken();
            await profileRepository.Remove();
            logger.LogInformation("Logout completed");
            return Unit.Value;
        }
    }
}
