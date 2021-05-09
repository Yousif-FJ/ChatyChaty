using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.HttpShemas.v1.Error
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            Errors = new List<string>();
        }

        public ErrorResponse(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException($"'{nameof(error)}' cannot be null or whitespace", nameof(error));
            }

            Errors = new List<string> { error };
        }
        public ErrorResponse(IEnumerable<string> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public IEnumerable<string> Errors { get; init; }


        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

    }
}
