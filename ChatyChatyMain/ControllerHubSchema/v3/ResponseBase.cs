using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public class ResponseBase<T>
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Data { get; set; }

        public ResponseBase(){}

        public ResponseBase(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                throw new InvalidOperationException("ModelState is valid, expected invalid modelstate");
            }
            Success = false;
            Errors = modelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
