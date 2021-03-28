using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChatyClient.HttpSchemas
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Data { get; set; }

        public Response() { }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
