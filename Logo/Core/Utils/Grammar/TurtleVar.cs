using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class TurtlePen
    {
        public Color color = Color.Black;
        public bool enable = true;
        public int width = 1;
        public int textSize = 14;
        public TurtlePen()
        {

        }
    }

    internal class TurtleVar
    {
        public int direction = 0;
        public bool hidden = false;
        public TurtlePen pen = new TurtlePen();
        public int x = 0;
        public int y = 0;

        public TurtleVar()
        {

        }
    }
}
