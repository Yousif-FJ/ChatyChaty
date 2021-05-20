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
    }
}
