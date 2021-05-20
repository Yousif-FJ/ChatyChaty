using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatyChaty.HttpShemas.v1.Error;
using ChatyChaty.HttpShemas.v1.Profile;
using ChatyChatyClient.Logic.Actions.Request.Messaging;
using ChatyChatyClient.Logic.AppExceptions;
using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.RepositoryInterfaces;
using MediatR;

namespace ChatyChatyClient.Logic.Actions.Handler.Messaging
{
    public class GetChatsHandler : IRequestHandler<GetChatsRequest, IList<Chat>>
    {
        private const string GetChatsURL = "/api/v1/Profile/Chats";

        private readonly HttpClient httpClient;
        private readonly IChatStateContainer stateContainer;

        public GetChatsHandler(HttpClient httpClient, IChatStateContainer stateContainer)
        {
            this.httpClient = httpClient;
            this.stateContainer = stateContainer;
        }
        public async Task<IList<Chat>> Handle(GetChatsRequest request, CancellationToken cancellationToken)
        {
            var chats = stateContainer.GetChats();
            if (chats is not null)
            {
                return chats;
            }

            var httpResponse = await httpClient.GetAsync(GetChatsURL,cancellationToken);

            List<UserProfileResponse> response;
            response = await httpResponse.ReadApplicatoinResponse<List<UserProfileResponse>>(cancellationToken);

            chats = response.ToEntityList();

            stateContainer.SetChats(chats);

            return chats;
        }
    }

}
