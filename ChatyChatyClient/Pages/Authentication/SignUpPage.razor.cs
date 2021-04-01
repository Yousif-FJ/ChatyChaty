using ChatyChatyClient.Actions.Authentication;
using ChatyChatyClient.Entities;
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
        private IMediator MediatR { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }

#pragma warning disable IDE0044 // Add readonly modifier
        private string Username;
        private string Password;
        private string DisplayName;
        private string Error;

        [CascadingParameter]
        public LoadingIndicator LoadingIndicator { get; set; }
        private bool DisableLoginButton;

        public async Task SignUp()
        {
            DisableButton();
            var result = await MediatR.Send(new SignUp(Username, Password, DisplayName));
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
            LoadingIndicator.Show();
            DisableLoginButton = true;
        }

        private void EnableButton()
        {
            DisableLoginButton = false;
            LoadingIndicator.Hide();
        }
    }
}
