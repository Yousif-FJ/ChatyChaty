using ChatyChatyClient.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.RepositoryInterfaces
{
    public interface ISelfProfileRepository
    {
        public ValueTask Set(UserProfile profile);
        public ValueTask<UserProfile> Get();
        public ValueTask Remove();
        public ValueTask Update(UserProfile profile);
    }
}
