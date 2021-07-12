using ChatyChatyClient.Blazor.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChatyClient.Logic.Entities;
using ChatyChatyClient.Logic.Actions.Request.Messaging;
using ChatyChatyClient.Logic.RepositoryInterfaces;
using Microsoft.JSInterop;

namespace ChatyChatyClient.Blazor.Pages
{
    public partial class ChatPage
    {
        [Inject]
        private IMediator MediatR { get; init; }
        [Inject]
        private ISelfProfileRepository SelfProfileRepository { get; init; }
        [Inject]
        private IJSRuntime JS { get; init; }

        [CascadingParameter]
        protected LoadingIndicator LoadingIndicator { get; init; }

        [Parameter]
        public string ChatId { get; set; }
        private ChatViewModel ViewModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            LoadingIndicator.Show();

            var chatInfo = await MediatR.Send(new GetChatMessagesRequest(ChatId));
            var selfProfile = await SelfProfileRepository.Get();
            ViewModel = new ChatViewModel(selfProfile, chatInfo);

            LoadingIndicator.Hide();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("ScrollBottomOfElement", "messages-container");
        }
    }
}
