using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v1
{
    /// <summary>
    /// An interface that repressent the list current active sessions 
    /// </summary>
    public interface IHubSessions
    {
        void AddClient(UserId userId);
        bool IsClientConnected(UserId userId);
        bool RemoveClient(UserId userId);
    }
}
