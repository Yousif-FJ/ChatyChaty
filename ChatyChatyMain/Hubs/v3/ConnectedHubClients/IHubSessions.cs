using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// An interface that repressent the list current active sessions 
    /// </summary>
    public interface IHubSessions
    {
        void AddClient(long userId);
        bool IsClientConnected(long userId);
        bool RemoveClient(long userId);
    }
}
