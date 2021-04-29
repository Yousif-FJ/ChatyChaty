using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v1
{
    public static class MessagesExtension
    {
        public static IEnumerable<MessageInfoReponse> ToMessageInfoResponse(this IEnumerable<Message> messages, UserId userId)
        {
            return messages.Select(message => new MessageInfoReponse
            (
                message.Body,
                message.ConversationId.Value,
                message.Id.Value,
                message.Sender.UserName,
                message.SenderId == userId ? message.Delivered : (bool?)null
            ));
        }
    }
}
