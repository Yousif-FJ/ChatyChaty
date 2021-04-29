using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChatyClient.Entities;
using MediatR;

namespace ChatyChatyClient.Actions.Request.Authentication
{
    public class SignUpRequest : IRequest<AuthenticationResult>
    {
        public SignUpRequest(string username, string password, string displayName)
        {
            Username = username;
            Password = password;
            DisplayName = displayName;
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
