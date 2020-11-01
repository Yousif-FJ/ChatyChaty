using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Services
{
    public interface IAuthenticationService
    {
        public bool IsAuthenticated();
        public string GetAuthenticationToken();
        public string Login(string username, string password);
        public void Logout();
        public string Signup(string username, string password);
    }
}
