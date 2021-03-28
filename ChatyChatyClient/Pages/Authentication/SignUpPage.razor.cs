using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Pages.Authentication
{
    public partial class SignUpPage
    {
        [Inject]
        protected IMediator MediatR { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Error { get; set; }
        public void SignUp()
        {

        }
    }
}
