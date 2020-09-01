using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.Hubs.v3
{
    /// <summary>
    /// Main sigalR hub that handles WebSocket method calles
    /// </summary>
    [Authorize]
    public class MainHub : Hub<IChatClient>
    {
        public MainHub(IMessageService messageService,
            HubClientsStateManager hubClients)
        {
            this.messageService = messageService;
            this.hubClients = hubClients;
            jsonSerializerOption = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        
        private readonly IMessageService messageService;
        private readonly HubClientsStateManager hubClients;
        private readonly JsonSerializerOptions jsonSerializerOption;

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        /// <summary>
        /// A method called by the client everytime it connects to track the last and message and keep things in sync
        /// </summary>
        /// <param name="lastMessageIdJson"></param>
        /// <returns></returns>
        public async Task RegisterSession(string lastMessageIdJson)
        {
            //check if we have valid json
            if (TryDeserialize<long>(lastMessageIdJson, out long lastMessageId) == false)
            {
                _ = Clients.Caller.InvalidJsonResponse(GetResponseJsonError().ToJson());
            }
            else
            {
                var userId = long.Parse(Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

                var result = await messageService.GetNewMessages(userId, lastMessageId);

                //if there are new messages
                if (result.Messages.Count() > 0)
                {
                    var messages = result.Messages.ToMessageInfoResponse(userId);


                    _ = Clients.Caller.UpdateMessagesResponses(
                             new ResponseBase<IEnumerable<MessageInfoBase>>
                             {
                                 Success = true,
                                 Data = messages
                             }.ToJson()
                        );
                    hubClients.AddUpdateClient(userId, messages.Max(m => m.MessageId));
                }
                else
                {
                    hubClients.AddUpdateClient(userId, lastMessageId);
                }

            }
        }
        /// <summary>
        /// test method that echo back the recieved message to the caller
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendTest(string message)
        {
            return Clients.Caller.TestResponse(message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            hubClients.RemoveClient(long.Parse(userId));
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Try Deserialize the input json and gives output, return true when success
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inupt"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryDeserialize<T>(string inupt , out T output)
        {
            try
            {
                var lastMessageId = JsonSerializer.Deserialize<T>(inupt, jsonSerializerOption);
                output = lastMessageId;
                return true;
            }
            catch (Exception)
            {
                output = default;
                return false;
            }
        }

        static private ResponseBase<object> GetResponseJsonError()
        {
            return new ResponseBase<object>
            {
                Data = null,
                Success = false,
                Errors = new List<string> { "Invalid Json resquest" }
            };
        }
    }
}
