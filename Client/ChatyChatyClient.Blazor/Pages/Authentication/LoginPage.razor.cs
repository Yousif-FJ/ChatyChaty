using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Blazor.ViewModel;
using ChatyChatyClient.Logic.Entities;

namespace ChatyChatyClient.Blazor.Pages.Authentication
{
    public partial class LoginPage : AuthBase
    {
        private readonly LoginViewModel loginModel = new();

        protected override Task<AuthenticationResult> AuthenticateActionAsync()
        {
            return MediatR.Send(new LoginRequest(loginModel.Username, loginModel.Password));
        }
    }
}
