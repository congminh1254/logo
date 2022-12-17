using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core
{
    public class SourceCode
    {
        Position pos = new Position(0, -1);
        string newline = Utils.Utils.newline;
        StreamReader reader;
        char currChar = Utils.Utils.nullChar;
        char eof = Utils.Utils.eof;

        public SourceCode(StreamReader reader)
        {
            this.reader = reader;
            currChar = Utils.Utils.nullChar;
        }

        public char getCurrChar()
        {
            return currChar;
        }

        public char getNextChar()
        {
            pos.nextColumn();
            if (!reader.EndOfStream)
            {
                currChar = (char)reader.Read();
            }
            else
            {
                currChar = eof;
            }
            if (newline[0] == currChar)
            {
                pos.nextLine();
                reader.Read();
                currChar = '\n';
            }
            return currChar;
        }

        public char peekChar()
        {
            if (newline[0] == reader.Peek())
                return '\n';
            if (reader.Peek() == -1)
                return eof;
            return (char)reader.Peek();
        }

        public Position getPosition()
        {
            return (Position)pos.Clone();
        }
    }
}
