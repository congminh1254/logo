using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public class ErrorHandling
    {
        public class LogoException
        {
            public string message;
            public Position position;
            public string stackTrace;
            public LogoException(string message) {
                this.message = message;
                stackTrace = Environment.StackTrace;
            }

            public LogoException(string message, Position position)
            {
                this.message = message;
                this.position = position;
                stackTrace = Environment.StackTrace;
            }

            public string toString()
            {
                return "Error: " + message + (position != null ? ", position: " + position.toString() : "")+"\n"+stackTrace;
            }
        }

        public static List<LogoException> exceptions = new List<LogoException>();

        public static void pushError(LogoException e) { 
            exceptions.Add(e);
        }

        public static List<LogoException> getAllError()
        {
            return exceptions;
        }
    }
}
