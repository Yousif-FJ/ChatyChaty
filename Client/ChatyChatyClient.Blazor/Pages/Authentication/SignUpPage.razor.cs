using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Blazor.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChatyClient.Logic.Entities;

namespace ChatyChatyClient.Blazor.Pages.Authentication
{
    public partial class SignUpPage: AuthBase
    {
        private readonly SignUpViewModel signUpViewModel = new();

        protected override Task<AuthenticationResult> AuthenticateActionAsync()
        {
            return MediatR.Send(new SignUpRequest(signUpViewModel.Username, signUpViewModel.Password, signUpViewModel.DisplayName));
        }
    }
}
