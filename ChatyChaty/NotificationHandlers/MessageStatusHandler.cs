using ChatyChaty.Domain.Services.NotficationRequests;
using ChatyChaty.Hubs.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChaty.NotificationHandlers
{
    public class MessageStatusHandler : NotificationHandlerCommon<UsersGotMessageStatusUpdateAsync>
    {
        public MessageStatusHandler(IHubHelper hubHelper) : base(hubHelper)
        {
        }

        protected override Task Handle(UsersGotMessageStatusUpdateAsync request, CancellationToken cancellationToken)
        {
            foreach (var (receieverId, chatId, messageId) in request.MessageInfo)
            {
                var sentSuccessfully = hubHelper.TrySendMessageStatusUpdate(receieverId, chatId, messageId);
                if (sentSuccessfully == false)
                {
                }
            }

            return Task.CompletedTask;
        }
    }
}
