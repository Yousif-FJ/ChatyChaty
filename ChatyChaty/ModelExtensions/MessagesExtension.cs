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
        public static IEnumerable<MessageResponse> ToMessageInfoResponse(this IEnumerable<Message> messages, UserId userId)
        {
            List<MessageResponse> result = new();
            foreach (var message in messages)
            {
                result.Add(new MessageResponse
                            (
                                message.ConversationId.Value,
                                message.Id.Value,
                                message.SenderUsername,
                                message.Body,
                                message.SentTime,
                                message.DeliveryTime,
                                message.SenderId == userId ? message.Delivered : null
                            ));
            }
            return result;
        }
    }
}
