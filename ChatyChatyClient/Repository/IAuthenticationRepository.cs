using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Repository
{
    public interface IAuthenticationRepository
    {
        public Task<string> GetToken();
        public void SetToken(string value);
        public void RemoveToken();
    }
}
