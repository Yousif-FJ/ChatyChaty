using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Blazor.ViewModel;
using Microsoft.AspNetCore.Components.Authorization;
using ChatyChatyClient.Blazor.StartUpConfiguratoin;

namespace ChatyChatyClient.Blazor.Pages.Authentication
{
    public partial class LoginPage : AuthBase
    {
        private readonly LoginViewModel loginModel = new();

        private async Task Login()
        {
            DisableButton();
            var result = await MediatR.Send(new LoginRequest(loginModel.Username, loginModel.Password));
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
