using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Logo.Core.Utils;

namespace Logo.Core
{
    public class FileReader: SourceCode
    {
        int column = -1;
        int line = 0;
        string newline = Utils.Utils.newline;
        StreamReader reader;
        char nextChar = Utils.Utils.nullChar, lastChar = Utils.Utils.nullChar, currChar = Utils.Utils.nullChar;
        char eof = Utils.Utils.eof;

        public FileReader(StreamReader reader)
        {
            this.reader = reader;
            currChar = Utils.Utils.nullChar;
            nextChar = (char)reader.Read();
        }

        public char getCurrChar()
        {
            return currChar;
        }

        public char getNextChar()
        {
            if ((newline.Length > 1 && newline[1] == nextChar && newline[0] == currChar) || (newline.Length == 0 && newline[0] == nextChar)) {
                line++;
                column = -1;
            }
            column++;
            char c = nextChar;
            currChar = c;
            if (!reader.EndOfStream)
            {
                nextChar = (char)reader.Read();
            } else
            {
                nextChar = eof;
            }
            return c;
        }

        public char peekChar()
        {
            return nextChar;
        }

        public Position getPosition()
        {
            return new Position(line, column);
        }
    }
}
