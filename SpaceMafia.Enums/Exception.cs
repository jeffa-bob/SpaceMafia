using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceMafia.Exceptions
{
    public class AUException : Exception
    {
        public AUException(string message) : base(message)
        {
        }

        public AUException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
