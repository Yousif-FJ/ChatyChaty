using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v1
{
    /// <summary>
    /// This class store the InMemory state of the connected clients.
    /// It maps each SingalR Id to a userId and track the last message
    /// </summary>
    public class HubClientsStateManager
    {
        public HubClientsStateManager()
        {
            ConnectedClients = new List<ClientState>();
        }
        public IList<ClientState> ConnectedClients { get; }

        public void AddUpdateClient(long userId, long lastMessageId)
        {
            //check if the client already exists
            var client = GetClient(userId);
            if (client != null)
            {
                client.LastMessageId = lastMessageId;
            }
            //client is new
            else
            {
                ConnectedClients.Add(new ClientState {UserId = userId ,LastMessageId = lastMessageId });
            }
        }

        public ClientState GetClient(long userId)
        {
            return ConnectedClients.FirstOrDefault(client => client.UserId == userId);
        }

        public ClientState RemoveClient(long userId)
        {
            var clinetState = ConnectedClients.FirstOrDefault(client => client.UserId == userId);
            ConnectedClients.Remove(clinetState);
            return clinetState;
        }
    }

    public class ClientState
    {
        public long UserId { get; set; }
        public long LastMessageId { get; set; }
    }
}
