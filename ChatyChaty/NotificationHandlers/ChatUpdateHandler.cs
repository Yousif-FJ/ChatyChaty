using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.NotficationRequests;
using ChatyChaty.Hubs.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChaty.NotificationHandlers
{
    public class ChatUpdateHandler : NotificationHandlerCommon<UsersGotChatUpdateAsync>
    {
        private readonly IAccountManager accountManager;

        public ChatUpdateHandler(IHubHelper hubHelper, IAccountManager accountManager) : base(hubHelper)
        {
            this.accountManager = accountManager;
        }

        protected override async Task Handle(UsersGotChatUpdateAsync request, CancellationToken cancellationToken)
        {
            foreach (var (receiverId, chatId) in request.InvokerAndReceiverIds)
            {
                var chat = await accountManager.GetConversation(chatId, receiverId);
                bool successful = await hubHelper.TrySendChatUpdate(receiverId, chat);
                if (successful == false)
                {
                }
            }
        }
    }
}
