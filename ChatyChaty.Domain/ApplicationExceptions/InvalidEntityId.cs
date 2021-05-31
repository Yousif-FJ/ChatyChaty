using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.ApplicationExceptions
{
    public class InvalidEntityIdException : Exception
    {
        public InvalidEntityIdException(IdBase id) : base($"Invalid entity Id of type {id.GetType().Name}")
        {
        }

        public InvalidEntityIdException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
