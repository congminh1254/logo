using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public class Position
    {
        int line;
        int column;

        public Position(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        public override string ToString()
        {
            return "Position(line="+(line+1)+",column="+(column+1)+")";
        }

        public int getLine() { return line+1; }

        public int getColumn() { return column+1; }
    }
}
