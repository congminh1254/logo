using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class Board
    {
        public Variable width;
        public Variable height;
        public Bitmap bitmap;
        public Board(int width, int height) {
            this.width = new Variable("VW", VariableType.INT, width);
            this.height = new Variable("VH", VariableType.INT, height);
            bitmap = new Bitmap(width, height);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {
                Rectangle ImageSize = new Rectangle(0, 0, width, height);
                graph.FillRectangle(Brushes.White, ImageSize);
            }
        }
    }
}
