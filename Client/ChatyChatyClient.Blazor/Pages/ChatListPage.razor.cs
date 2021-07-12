using ChatyChatyClient.Blazor.ViewModel;
using ChatyChatyClient.Logic.Actions.Request.Authentication;
using ChatyChatyClient.Logic.Actions.Request.Messaging;
using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChatyClient.Logic.Entities;

namespace ChatyChatyClient.Blazor.Pages
{
    public partial class ChatListPage
    {
        [Inject]
        private IMediator MediatR { get; init; }
        [Inject]
        private NavigationManager Navigation { get; init; }

        [CascadingParameter]
        protected LoadingIndicator LoadingIndicator { get; init; }

        private ChatListViewModel ViewModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            LoadingIndicator.Show();
            var chats = await MediatR.Send(new GetChatsRequest());
            ViewModel = new ChatListViewModel(chats);
            LoadingIndicator.Hide();
        }

        private async Task Logout()
        {
            await MediatR.Send(new LogoutRequest());
            Navigation.NavigateTo("/client/login");
        }

        private void OpenChat(string chatId)
        {
            Navigation.NavigateTo($"/client/{chatId}");
        }
    }
}
