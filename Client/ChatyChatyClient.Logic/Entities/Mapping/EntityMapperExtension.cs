using ChatyChaty.HttpShemas.v1.Message;
using ChatyChaty.HttpShemas.v1.Profile;
using ChatyChatyClient.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Entities
{
    public static class EntityMapperExtension
    {
        public static IList<Chat> ToEntityList(this IList<UserProfileResponse> userProfiles)
        {
            var chats = new List<Chat>();
            foreach (var item in userProfiles)
            {
                chats.Add(new Chat(item.ChatId,
                    new UserProfile(item.Profile.Username, item.Profile.DisplayName, item.Profile.PhotoURL)));
            }
            return chats;
        }

        public static IList<Message> ToEntityList(this IList<MessageResponse> messageResponses)
        {
            var messages = new List<Message>();
            foreach (var item in messageResponses)
            {
                messages.Add(new Message(item.Body, item.Sender, item.MessageId, item.Delivered,
                    item.SentTime, item.DeliveryTime));
            }
            return messages;
        }
    }
}
