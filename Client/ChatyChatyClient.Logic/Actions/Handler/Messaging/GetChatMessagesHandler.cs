using ChatyChatyClient.Logic.Actions.Request.Messaging;
using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.RepositoryInterfaces;
using ChatyChatyClient.Logic.Actions.Handler;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChatyChaty.HttpShemas.v1.Message;

namespace ChatyChatyClient.Logic.Actions.Handler.Messaging
{
    public class GetChatMessagesHandler : IRequestHandler<GetChatMessagesRequest, Chat>
    {
        private const string GetMessagesForChatURL = "/api/v1/Message/MessagesForChat";
        private readonly HttpClient httpClient;
        private readonly IChatStateContainer stateContainer;
        private readonly IMediator mediator;

        public GetChatMessagesHandler(HttpClient httpClient, IChatStateContainer stateContainer, IMediator mediator)
        {
            this.httpClient = httpClient;
            this.stateContainer = stateContainer;
            this.mediator = mediator;
        }
        public async Task<Chat> Handle(GetChatMessagesRequest request, CancellationToken cancellationToken)
        {
            var chat = stateContainer.GetChat(request.ChatId);
            if (chat is null)
            {
                await mediator.Send(new GetChatsRequest(), cancellationToken);
                chat = stateContainer.GetChat(request.ChatId);

                if (chat is null)
                {
                    throw new Exception("chat can't be null");
                }
            }
            if (chat.Messages is not null)
            {
                return chat;
            }

            var httpResponse = await httpClient.GetAsync($"{GetMessagesForChatURL}?chatId={request.ChatId}", cancellationToken);

            var messagesResponse = await httpResponse.ReadAppResponseAsync<List<MessageResponse>>(cancellationToken);

            chat.Messages = messagesResponse.ToEntityList();
            chat.Messages.Sort();

            return chat;
        }
    }
}
