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
        public Variable VW { get; set; }
        public Variable VH { get; set; }
        public Bitmap bitmap;
        public Board(int width, int height) {
            this.VW = new Variable(width);
            this.VH = new Variable(height);
            bitmap = new Bitmap(width, height);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {
                Rectangle ImageSize = new Rectangle(0, 0, width, height);
                graph.FillRectangle(Brushes.White, ImageSize);
            }
        }

        //public object get(string name, int argsCount)
        //{
        //    switch (name)
        //    {
        //        case "VW":
        //            return width;
        //        case "VH":
        //            return height;
        //    }
        //    ErrorHandling.pushError(new ErrorHandling.LogoException("Params " + name + " of Board is not valid!"));
        //    return null;
        //}
    }
}
