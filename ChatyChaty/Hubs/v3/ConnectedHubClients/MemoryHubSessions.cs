using ChatyChaty.Domain.Model.Entity;
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
            connectedClientIds = new List<UserId>();
        }
        private readonly IList<UserId> connectedClientIds;

        public void AddClient(UserId userId)
        {
            //check if the client already exists
            var IsConnected = IsClientConnected(userId);
            if (IsConnected == false)
            {
                connectedClientIds.Add(userId);
            }
        }

        public bool IsClientConnected(UserId userId)
        {
            return connectedClientIds.Contains(userId);
        }

        public bool RemoveClient(UserId userId)
        {
            return connectedClientIds.Remove(userId);
        }
    }
}
