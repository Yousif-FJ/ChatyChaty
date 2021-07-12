using ChatyChaty.HttpShemas.v1.Error;
using ChatyChatyClient.Logic.AppExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Actions.Handler
{
    public static class ActionExtension
    {
        /// <summary>
        /// Read http response for the applicatoin and handle exptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponse"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ErrorResponseException"></exception>
        /// <exception cref="UnexpectedResponseException"></exception>
        /// <returns></returns>
        public async static Task<T> ReadAppResponseAsync<T>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            T response;
            try
            {
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
                    throw new ErrorResponseException(errorResponse.Errors.First());
                }

                response = await httpResponse.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            }
            catch (Exception e) when (e is NotSupportedException || e is JsonException)
            {
                var responseAsString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new UnexpectedResponseException($"Response doesn't match expected type, response as string : {responseAsString}", e);
            }
            return response;
        }
    }
}
