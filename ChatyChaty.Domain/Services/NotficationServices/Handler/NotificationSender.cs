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
        public readonly (long receiverId, long chatId)[] InvokerAndReceiverIds;

        public UsersGotChatUpdateAsync(params (long receiverId, long chatId)[] invokerAndReceiverIds)
        {
            this.InvokerAndReceiverIds = invokerAndReceiverIds;
        }
    }

    public class UserGotNewMessageAsync: IRequest
    {
        public readonly (long userId, long messageId)[] UserAndMessageId;

        public UserGotNewMessageAsync(params (long userId, long messageId)[] userAndMessageId)
        {
            this.UserAndMessageId = userAndMessageId;
        }
    }

    public class UsersGotMessageStatusUpdateAsync : IRequest
    {
        public readonly (long receieverId, long chatId, long messageId)[] MessageInfo;

        public UsersGotMessageStatusUpdateAsync(params (long receieverId, long chatId, long messageId)[] messageInfo)
        {
            this.MessageInfo = messageInfo;
        }
    }

    public class UserUpdatedTheirProfileAsync : IRequest
    {
        public readonly long UserId;

        public UserUpdatedTheirProfileAsync(long usersId)
        {
            this.UserId = usersId;
        }
    }
}
