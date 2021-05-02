using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.NotficationRequests;
using ChatyChaty.Hubs.v1;

namespace ChatyChaty.NotificationHandlers
{
    public class NewMessageHandler : NotificationHandlerCommon<UserGotNewMessageAsync>
    {
        private readonly IMessageRepository messageRepository;

        public NewMessageHandler(IHubHelper hubHelper, IMessageRepository messageRepository) : base(hubHelper)
        {
            this.messageRepository = messageRepository;
        }

        protected override async Task Handle(UserGotNewMessageAsync request, CancellationToken cancellationToken)
        {
            foreach (var (userId, messageId) in request.UserAndMessageId)
            {

                var message = await messageRepository.GetAsync(messageId);

                bool successful = hubHelper.TrySendMessageUpdate(userId, new List<Message> { message });
                if (successful == false)
                {
                }
            }
        }
    }
}
