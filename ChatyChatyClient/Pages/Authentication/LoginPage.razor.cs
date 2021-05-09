using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using ChatyChatyClient.Actions.Request.Authentication;
using ChatyChatyClient.ViewModel;

namespace ChatyChatyClient.Pages.Authentication
{
    public partial class LoginPage
    {
        [Inject]
        private IMediator MediatR { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private readonly LoginViewModel loginModel = new();
        private string Error;

        [CascadingParameter]
        protected LoadingIndicator LoadingIndicator { get; set; }
        private bool DisableLoginButton;

        public async Task Login()
        {
            DisableButton();
            var result = await MediatR.Send(new LoginRequest(loginModel.Username, loginModel.Password));
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
