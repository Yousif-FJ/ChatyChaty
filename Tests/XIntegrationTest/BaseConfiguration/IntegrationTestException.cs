using ChatyChaty.HttpShemas.v1.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace XIntegrationTest.BaseConfiguration
{
    public class IntegrationTestException : Exception
    {
        public readonly HttpResponseMessage HttpResponse;
        public IntegrationTestException(HttpResponseMessage httpResponse)
        {
            this.HttpResponse = httpResponse;
        }
        public Task<ErrorResponse> ReadHttpError() => HttpResponse.Content.ReadAsAsync<ErrorResponse>();
    }
}
