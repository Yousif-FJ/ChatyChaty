using ChatyChaty.ControllerHubSchema.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// An interface for SignalR hub, that represent responses called by the server to the clients
    /// </summary>
    public interface IChatClient
    {
        Task UpdateMessagesResponse(Response<IEnumerable<MessageInfoBase>> messagesList);
        Task UpdateChatResponse(Response<UserProfileResponseBase> chatInfo);
    }
}
