using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Domain.Services.MessageServices;
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
            HubConnectedClients hubClients)
        {
            this.messageService = messageService;
            this.hubClients = hubClients;
            jsonSerializerOption = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        
        private readonly IMessageService messageService;
        private readonly HubConnectedClients hubClients;
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
        public async Task SyncSession(string lastMessageIdJson)
        {
            //check if we have valid json
            if (TryDeserialize<long>(lastMessageIdJson, out long lastMessageId) == false)
            {
                _ = Clients.Caller.SyncSessionErrorResponse(GetResponseJsonError().ToJson());
            }
            else
            {
                var userId = long.Parse(Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

                //get new messages form message service
                var result = await messageService.GetNewMessages(userId, lastMessageId);
                
                //send update to client about new messages (using this extension method)
                Clients.SendMessageUpdates(result, userId);

                //update client list
                hubClients.AddClient(userId);
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


        /// <summary>
        /// Send New message with the chatId
        /// </summary>
        /// <param name="messageSchemaJson">string representing the message info to be deserialized</param>
        /// <returns></returns>
        public async Task SendMessage(string messageSchemaJson)
        {
            if (TryDeserialize<SendMessageSchema>(messageSchemaJson, out SendMessageSchema messageSchema) == false)
            {
                _ = Clients.Caller.SendMessageErrorResponse(GetResponseJsonError().ToJson());
            }
            else
            {
                var userId = long.Parse(Context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
                //send message to message service
                var result = await messageService.SendMessage(messageSchema.ChatId, userId, messageSchema.Body);
                
                if (result.Error != null)
                {
                    _ = Clients.Caller.SendMessageErrorResponse(
                        new ResponseBase<object>
                        {
                            Data = null,
                            Success = false,
                            Errors = new List<string> { result.Error }
                        }.ToJson()
                        );
                }
                //response to the client 
                else
                {
                    Clients.SendMessageUpdates(result, userId);
                }
            }
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
