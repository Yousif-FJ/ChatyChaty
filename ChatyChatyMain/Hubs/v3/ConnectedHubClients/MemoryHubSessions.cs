using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// Store an InMemory list of the current active sessions
    /// </summary>
    public class MemoryHubSessions : IHubSessions
    {
        public MemoryHubSessions()
        {
            connectedClientIds = new List<long>();
        }
        private readonly IList<long> connectedClientIds;

        public void AddClient(long userId)
        {
            //check if the client already exists
            var IsConnected = IsClientConnected(userId);
            if (IsConnected == false)
            {
                connectedClientIds.Add(userId);
            }
        }

        public bool IsClientConnected(long userId)
        {
            return connectedClientIds.Contains(userId);
        }

        public bool RemoveClient(long userId)
        {
            return connectedClientIds.Remove(userId);
        }
    }
}
