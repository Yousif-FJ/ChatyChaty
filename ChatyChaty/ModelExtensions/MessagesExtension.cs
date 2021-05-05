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
            List<MessageInfoReponse> result = new();
            foreach (var message in messages)
            {
                if (message.Sender is null)
                {
                    throw new ArgumentNullException(nameof(messages), "a message in messages must have a sender");
                }
                result.Add(new MessageInfoReponse
                            (
                                message.Body,
                                message.ConversationId.Value,
                                message.Id.Value,
                                message.Sender.UserName,
                                message.SenderId == userId ? message.Delivered : null
                            ));
            }
            return result;
        }
    }
}
