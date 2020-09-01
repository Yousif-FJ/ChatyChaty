using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public static class MessagesExtensionMethods
    {
        public static IList<MessageInfoBase> ToMessageInfoResponse(this IEnumerable<Message> messages, long userId)
        {
            var returnMessages = new List<MessageInfoBase>();
            foreach (var message in messages)
            {
                returnMessages.Add(new MessageInfoBase
                {
                    Body = message.Body,
                    ChatId = message.ConversationId,
                    MessageId = message.Id,
                    Sender = message.Sender.UserName,
                    Delivered = message.SenderId == userId ? message.Delivered : (bool?)null
                });
            }
            return returnMessages;
        }
    }
}
