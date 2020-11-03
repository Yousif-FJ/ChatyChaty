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
        public readonly (long receiverId, long chatId)[] invokerAndReceiverIds;

        public UsersGotChatUpdateAsync(params (long receiverId, long chatId)[] invokerAndReceiverIds)
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

    public class UsersGotMessageStatusUpdateAsync : IRequest
    {
        public readonly (long userId, long messageId)[] userAndMessageIds;

        public UsersGotMessageStatusUpdateAsync(params (long userId, long messageId)[] userAndMessageIds)
        {
            this.userAndMessageIds = userAndMessageIds;
        }
    }

}
