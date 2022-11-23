﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public class Position
    {
        public int line;
        public int column;

        public Position(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        public string toString()
        {
            return "Position(line="+line+",column="+column+")";
        }

        public int getLine() { return line; }

        public int getColumn() { return column; }
    }
}