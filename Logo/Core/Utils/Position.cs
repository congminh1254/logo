using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public class Position: ICloneable
    {
        public int line { get; private set; } = 0;
        public int column { get; private set; } = -1;

        public Position(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        public void nextLine()
        {
            line++;
            column = -1;
        }

        public void nextColumn() { column++; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
