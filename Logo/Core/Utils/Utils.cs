using System;
using System.Collections.Generic;
using System.IO;
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
        public static char lexerNewline = '\n';

        static string getNewline()
        {
            return System.Environment.NewLine;
        }
        public static int charToNumber(char c)
        {
            return c - '0';
        }

        public static bool IsNumber(object value)
        {
            return value is int || value is float;
        }
        public static StreamReader stringToStreamReader(string s)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(s);
            MemoryStream stream = new MemoryStream(byteArray);
            return new StreamReader(stream);
        }
    }
}
