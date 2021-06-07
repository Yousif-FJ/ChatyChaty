using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace XIntegrationTest.BaseConfiguration
{
    public static class IntegrationTestExtension
    {
        public static async Task<T> CustomRead200Response<T>(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    var result = await response.Content.ReadAsAsync<T>();
                    return result;
                }
                catch (UnsupportedMediaTypeException)
                {
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Unexpected type response, The response as string : {responseAsString}");
                }
            }
            else
            {
                throw new IntegrationTestException(response);
            }
        }
    }
}
