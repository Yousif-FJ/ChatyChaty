using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.NotficationServices.Handler
{
    /// <summary>
    /// Interface that handles notification events
    /// </summary>

    public class UsersGotChatUpdateAsync : IRequest
    {
        public readonly (long InvokerId, long ReceiverId)[] invokerAndReceiverIds;

        public UsersGotChatUpdateAsync(params (long InvokerId, long ReceiverId)[] invokerAndReceiverIds)
        {
            this.invokerAndReceiverIds = invokerAndReceiverIds;
        }
    }

    public class UserGotNewMessageAsync: IRequest
    {
        public readonly (long userId, long messageId)[] userAndMessageId;

        public UserGotNewMessageAsync(params (long userId, long messageId)[] userAndMessageId)
        {
            this.userAndMessageId = userAndMessageId;
        }
    }

    public class UsersGotMessageDeliveredAsync : IRequest
    {
        public readonly (long userId, long messageId)[] userAndMessageIds;

        public UsersGotMessageDeliveredAsync(params (long userId, long messageId)[] userAndMessageIds)
        {
            this.userAndMessageIds = userAndMessageIds;
        }
    }

}
