using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Repository.Interfaces
{
    public interface IAuthenticationRepository
    {
        public ValueTask<string> GetToken();
        public ValueTask SetToken(string value);
        public ValueTask RemoveToken();
    }
}
