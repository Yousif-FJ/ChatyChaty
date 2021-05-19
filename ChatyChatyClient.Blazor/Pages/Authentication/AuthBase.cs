using ChatyChatyClient.Blazor.StartUpConfiguratoin;
using ChatyChatyClient.Blazor.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.Pages.Authentication
{
    public abstract class AuthBase : ComponentBase
    {
        [Inject]
        protected IMediator MediatR { get; init; }
        [Inject]
        protected NavigationManager NavigationManager { get; init; }

        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; init; }
        protected CustomAuthStateProvider CustomAuthStateProvider => (CustomAuthStateProvider)AuthStateProvider;
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; init; }


        [CascadingParameter]
        protected LoadingIndicator LoadingIndicator { get; init; }


        protected string Error;
        protected bool DisableLoginButton;

        protected override Task OnInitializedAsync()
        {
            return IfLoggedInRedirectToHome();
        }

        private async Task IfLoggedInRedirectToHome()
        {
            var authState = await AuthenticationStateTask;

            if (authState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/client");
            }
        }

        protected void DisableButton()
        {
            LoadingIndicator.Show();
            DisableLoginButton = true;
        }

        protected void EnableButton()
        {
            DisableLoginButton = false;
            LoadingIndicator.Hide();
        }
    }
}
