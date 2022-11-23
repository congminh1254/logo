﻿using Logo.Core.Utils;
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

        public SourceCode(string fileName)
        {
            currPos = new Position(0, 0);
            reader = new FileReader(fileName);
        }

        public char getNextChar()
        {
            currentChar = reader.getNextChar();
            currPos = reader.getPostion();
            return currentChar;
        }

        public char getChar()
        {
            return currentChar;
        }

        public Position getPosition()
        {
            return currPos;
        }
    }
}