using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using ChatyChatyClient.Actions.Authentication;

namespace ChatyChatyClient.Pages.Authentication
{
    public partial class LoginPage
    {
        [Inject]
        protected IMediator MediatR { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Error { get; set; }

        public async Task Login()
        {
            var result = await MediatR.Send(new Login(Username, Password));
            if (result.IsSuccessful == false)
            {
                Error = result.Error;
            }
        }
    }
}
