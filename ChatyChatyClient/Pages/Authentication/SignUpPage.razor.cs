using ChatyChatyClient.Actions.Request.Authentication;
using ChatyChatyClient.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


        private readonly SignUpViewModel signUpViewModel = new();
        private string Error;

        [CascadingParameter]
        public LoadingIndicator LoadingIndicator { get; set; }
        private bool DisableLoginButton;

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
