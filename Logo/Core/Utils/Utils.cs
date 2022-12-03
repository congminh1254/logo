using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public class Utils
    {
        public static char nullChar = '\0';
        public static char eof = (char)3;
        public static string newline = getNewline();

        static string getNewline()
        {
            return System.Environment.NewLine;
        }
        public static byte charToNumber(char c)
        {
            switch (c)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                default:
                    throw new Exception("Invalid number character!");
            }
        }
    }
}
