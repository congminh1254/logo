using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core
{
    public class SourceCode
    {
        Position currPos;

        FileReader reader;
        char currentChar;

        public SourceCode(string input, bool file = true)
        {
            currPos = new Position(0, 0);
            reader = new FileReader(input, file);
        }

        public char getNextChar()
        {
            currentChar = reader.getNextChar();
            currPos = reader.getPostion();
            return currentChar;
        }

        public char previewChar()
        {
            return reader.previewChar();
        }

        public char previewChar2()
        {
            return reader.previewChar2();
        }

        public char getChar()
        {
            return currentChar;
        }

        public Position getPosition()
        {
            return currPos;
        }

        public string getAllText()
        {
            return reader.getAllText();
        }
    }
}
