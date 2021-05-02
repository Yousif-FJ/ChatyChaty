using ChatyChaty.Domain.Model.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.NotficationRequests
{
    /// <summary>
    /// Interface that handles notification events
    /// </summary>

    public class UsersGotChatUpdateAsync : IRequest
    {
        public readonly (UserId receiverId, ConversationId chatId)[] InvokerAndReceiverIds;

        public UsersGotChatUpdateAsync(params (UserId receiverId, ConversationId chatId)[] invokerAndReceiverIds)
        {
            InvokerAndReceiverIds = invokerAndReceiverIds;
        }
    }

    public class UserGotNewMessageAsync : IRequest
    {
        public readonly (UserId userId, MessageId messageId)[] UserAndMessageId;

        public UserGotNewMessageAsync(params (UserId userId, MessageId messageId)[] userAndMessageId)
        {
            UserAndMessageId = userAndMessageId;
        }
    }

    public class UsersGotMessageStatusUpdateAsync : IRequest
    {
        public readonly (UserId receieverId, ConversationId chatId, MessageId messageId)[] MessageInfo;

        public UsersGotMessageStatusUpdateAsync(params (UserId receieverId, ConversationId chatId, MessageId messageId)[] messageInfo)
        {
            MessageInfo = messageInfo;
        }
    }

    public class UserUpdatedTheirProfileAsync : IRequest
    {
        public readonly UserId UserId;

        public UserUpdatedTheirProfileAsync(UserId usersId)
        {
            UserId = usersId;
        }
    }
}
