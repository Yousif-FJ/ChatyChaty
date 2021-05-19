using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Blazor.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.Pages.Authentication
{
    public partial class SignUpPage: AuthBase
    {
        private readonly SignUpViewModel signUpViewModel = new();
        public async Task SignUp()
        {
            DisableButton();
            var result = await MediatR.Send(new SignUpRequest(signUpViewModel.Username, signUpViewModel.Password, signUpViewModel.DisplayName));
            if (result.IsSuccessful == false)
            {
                Error = result.Error;
                EnableButton();
                return;
            }
            EnableButton();
            CustomAuthStateProvider.UpdateAuthState();
            NavigationManager.NavigateTo("/client");
        }
    }
}
