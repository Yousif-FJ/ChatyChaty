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
        private IMediator MediatR { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }

#pragma warning disable IDE0044 // Add readonly modifier
        private string Username;
        private string Password;
        private string Error;


        private bool ShowLoadingIndicator;
        private bool DisableLoginButton;

        public async Task Login()
        {
            DisableButton();
            var result = await MediatR.Send(new Login(Username, Password));
            if (result.IsSuccessful == false)
            {
                Error = result.Error;
                EnableButton();
                return;
            }

            NavigationManager.NavigateTo("/client");
        }

        private void DisableButton()
        {
            ShowLoadingIndicator = true;
            DisableLoginButton = true;
        }

        private void EnableButton()
        {
            ShowLoadingIndicator = false;
            DisableLoginButton = false;
        }
    }
}
