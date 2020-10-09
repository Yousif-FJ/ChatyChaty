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
        Task TestResponse(string message);
        Task SyncSessionErrorResponse(string jsonResponse);
        Task SendMessageErrorResponse(string jsonResponse);
        Task UpdateMessagesResponses(string jsonMessagesList);
        Task UpdateChatsResponses(string jsonChatList);
        Task UpdateDeliveredResponses(string jsonChatLastDeliveredMessage);
    }
}
