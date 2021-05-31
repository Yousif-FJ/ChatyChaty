using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.AppExceptions
{
    public class UnexpectedResponseException : Exception
    {
        public UnexpectedResponseException(string message) : base(message)
        {
        }

        public UnexpectedResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
