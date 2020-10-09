using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// This class store the InMemory list of the connected clients
    /// </summary>
    public class HubConnectedClients
    {
        public HubConnectedClients()
        {
            ConnectedClientIds = new List<long>();
        }
        public IList<long> ConnectedClientIds { get; }

        public void AddClient(long userId)
        {
            //check if the client already exists
            var IsConnected = IsClientConnected(userId);
            if (IsConnected == false)
            {
                ConnectedClientIds.Add(userId);
            }
        }

        public bool IsClientConnected(long userId)
        {
            return ConnectedClientIds.Contains(userId);
        }

        public bool RemoveClient(long userId)
        {
            return ConnectedClientIds.Remove(userId);
        }
    }
}
