using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v1
{
    public static class MessagesExtension
    {
        public static IEnumerable<MessageInfoReponseBase> ToMessageInfoResponse(this IEnumerable<Message> messages, UserId userId)
        {
            return messages.Select(message => new MessageInfoReponseBase
            {
                Body = message.Body,
                ChatId = message.ConversationId.Value,
                MessageId = message.Id.Value,
                Sender = message.Sender.UserName,
                Delivered = message.SenderId == userId ? message.Delivered : (bool?)null
            });
        }
    }
}
