using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChatyClient.Entities;
using MediatR;

namespace ChatyChatyClient.Actions.Request.Authentication
{
    public class LoginRequest : IRequest<AuthenticationResult>
    {
        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
