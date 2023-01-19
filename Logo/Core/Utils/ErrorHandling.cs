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
            public LogoException(string message)
            {
                this.message = message;
            }

            public LogoException(string message, Position position)
            {
                this.message = message;
                this.position = position;
            }

            public string ToString()
            {
                return "message="+message+", position="+position.ToString();
            }
        }
        
        public static List<LogoException> exceptions { get; private set; } = new List<LogoException>();

        public static void pushError(LogoException e, bool throwError = false)
        {
            exceptions.Add(e);
            if (throwError )
            {
                throw new Exception(e.ToString());
            }
        }

        public static void clear()
        {
            exceptions.Clear();
        }
    }
}
