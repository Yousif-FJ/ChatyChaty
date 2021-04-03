using ChatyChaty.ControllerHubSchema.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v1
{
    /// <summary>
    /// An interface for SignalR hub, that represent responses called by the server to the clients
    /// </summary>
    public interface IChatClient
    {
        Task UpdateMessages(IEnumerable<MessageInfoReponse> messagesList);
        Task UpdateChat(UserProfileResponse chatInfo);
        Task UpdateMessageStatus(MessageStatusResponse messageStatusInfo);
    }
}
