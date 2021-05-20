using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.RepositoryInterfaces
{
    public interface IAuthenticationRepository
    {
        public ValueTask<bool> IsAuthenticated();
        public ValueTask<string> GetToken();
        public ValueTask SetToken(string value);
        public ValueTask RemoveToken();
    }
}
