using ChatyChatyClient.Blazor.StartUpConfiguratoin;
using ChatyChatyClient.Blazor.ViewModel;
using ChatyChatyClient.Logic.Entities;
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
        private CustomAuthStateProvider CustomAuthStateProvider => (CustomAuthStateProvider)AuthStateProvider;
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; init; }


        [CascadingParameter]
        protected LoadingIndicator LoadingIndicator { get; init; }

        protected string Error;
        protected bool DisableLoginButton;

        protected abstract Task<AuthenticationResult> AuthenticateActionAsync();
        protected override Task OnInitializedAsync()
        {
            return IfLoggedInRedirectToHome();
        }

        protected async Task AuthenticateButtonClickAsync()
        {
            StartLoadingState();
            var result = await AuthenticateActionAsync();
            if (result.IsSuccessful == false)
            {
                Error = result.Error;
                EndLoadingState();
                return;
            }
            EndLoadingState();
            CustomAuthStateProvider.UpdateAuthState();
            NavigationManager.NavigateTo("/client");
        }

        private async Task IfLoggedInRedirectToHome()
        {
            var authState = await AuthenticationStateTask;

            if (authState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/client");
            }
        }

        protected void StartLoadingState()
        {
            LoadingIndicator.Show();
            DisableLoginButton = true;
        }

        protected void EndLoadingState()
        {
            DisableLoginButton = false;
            LoadingIndicator.Hide();
        }
    }
}
