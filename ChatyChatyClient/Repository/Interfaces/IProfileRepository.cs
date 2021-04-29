using ChatyChatyClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Repository
{
    public interface IProfileRepository
    {
        public Task Set(UserProfile profile);
        public Task<UserProfile> Get();
        public Task Update(UserProfile profile);
    }
}
