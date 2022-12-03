using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core
{
    public class TextReader: SourceCode
    {
        FileReader reader;
        char currentChar;
        string input = "";
        string newline = Utils.Utils.newline;
        char currChar = Utils.Utils.nullChar, lastChar = Utils.Utils.nullChar, nextChar = Utils.Utils.nullChar;
        char eof = Utils.Utils.eof;
        int index = -1, line = 0, column = -1;

        public TextReader(string input)
        {
            this.input = input.Trim();
            index = -1;
            currChar = Utils.Utils.nullChar;
            if (input.Length > 0)
            {
                nextChar = input[index+1];
            } else
            {
                nextChar = eof;
            }
        }

        public char getCurrChar()
        {
            return currChar;
        }

        public char peekChar()
        {
            return nextChar;
        }

        public char getNextChar()
        {
            if ((newline.Length > 1 && newline[1] == nextChar && newline[0] == lastChar) || (newline.Length == 0 && newline[0] == nextChar))
            {
                line++;
                column = -1;
            }
            column++;
            index++;
            char c = nextChar;
            currChar = nextChar;
            if (index >= input.Length)
            {
                nextChar = eof;
            } else
            {
                nextChar = input[index];
            }
            return c;
        }

        public Position getPosition()
        {
            return new Position(line, column);
        }
    }
}
