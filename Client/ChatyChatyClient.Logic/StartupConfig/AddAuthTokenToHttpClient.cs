using ChatyChatyClient.Logic.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.StartupConfig
{
    public class AddAuthTokenToHttpClient : DelegatingHandler
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public AddAuthTokenToHttpClient(IAuthenticationRepository authenticationRepository)
        {
            this.authenticationRepository = authenticationRepository;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await authenticationRepository.GetToken();
            if (token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new Exception("use httpClient.SendAsync instead of httpClient.Send");
        }
    }
}
