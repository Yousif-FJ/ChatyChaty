using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.AppExceptions
{
    public class ErrorResponseException : ApplicationException
    {
        public ErrorResponseException(string message) : base(message)
        {
        }

        public ErrorResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
