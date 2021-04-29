using ChatyChatyClient.Actions.Authentication;
using ChatyChatyClient.HttpSchemas;
using ChatyChatyClient.HttpSchemas.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChatyClient.Actions
{
    public static class ActionExtension
    {
        public async static Task<Response<TResult>> ReadAppResponseDataAs<TResult>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            Response<TResult> response ;
            try
            {
                response = await httpResponse.Content.ReadFromJsonAsync<Response<TResult>>(cancellationToken: cancellationToken);
            }
            catch (NotSupportedException) {
                response = new Response<TResult>() { Success = false, Errors = new List<string> { "An error occured at the server" } };
            }

            return response;
        }
    }
}
