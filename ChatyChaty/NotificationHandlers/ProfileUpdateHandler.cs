using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.NotficationRequests;
using ChatyChaty.Hubs.v1;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChaty.NotificationHandlers
{
    public class ProfileUpdateHandler : NotificationHandlerCommon<UserUpdatedTheirProfileAsync>
    {
        private readonly IAccountManager accountManager;

        public ProfileUpdateHandler(IHubHelper hubHelper, IAccountManager accountManager) : base(hubHelper)
        {
            this.accountManager = accountManager;
        }

        protected async override Task Handle(UserUpdatedTheirProfileAsync request, CancellationToken cancellationToken)
        {
            IEnumerable<ProfileAccountModel> chats = await accountManager.GetConversations(request.UserId);
            foreach (var chat in chats)
            {
                var successful = await hubHelper.TrySendChatUpdate(chat.UserId, chat);
                if (successful == false)
                {
                }
            }
        }
    }
}
