using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.HttpShemas.v1.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ModelExtensions
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
                message.SenderId == userId ? message.Delivered : null
            ));
        }
    }
}
