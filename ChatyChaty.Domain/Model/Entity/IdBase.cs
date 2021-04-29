using ChatyChaty.Domain.ApplicationExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.Entity
{
    public abstract record IdBase
    {
        public string Value { get; }
        public IdBase()
        {
            Value = Guid.NewGuid().ToString();
        }

        public IdBase(string value)
        {
            if (Guid.TryParse(value, out _))
            {
                Value = value;
            }
            else
            {
                throw new InvalidIdFormatException($"Invalid {GetType().Name}");
            }
        }

        public override string ToString()
        {
            return Value;
        }
    };
}
